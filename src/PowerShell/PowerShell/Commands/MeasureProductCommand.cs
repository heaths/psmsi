// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Measure-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Measure, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(PSDriveInfo))]
    public sealed class MeasureProductCommand : PackageCommandBase
    {
        private static readonly char[] PropertyQuotes = new char[] { '\'', '\"' };
        private static readonly char[] PropertySeparator = new char[] { '=' };
        private static readonly string ViewCustomActions = "SELECT `Action` FROM `CustomAction` WHERE `Type` = 51";
        private static readonly string ViewExecuteActions = "SELECT `Action` FROM `InstallExecuteSequence` ORDER BY `Sequence`";

        private InstallUIOptions previousInternalUI = InstallUIOptions.Default;
        private SpaceRequirementsCollection spaceRequirements = new SpaceRequirementsCollection();

        /// <summary>
        /// Gets or sets the target directory for the product.
        /// </summary>
        [Parameter]
        [Alias("TargetDirectory")]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the remaining arguments as strings.
        /// </summary>
        [Parameter(ValueFromRemainingArguments = true)]
        public string[] Properties { get; set; }

        /// <summary>
        /// Sets the internal user interface to be silent.
        /// </summary>
        protected override void BeginProcessing()
        {
            this.previousInternalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
            base.BeginProcessing();
        }

        /// <summary>
        /// Gathers disk cost information from a product package with optional patches and transforms sequenced.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> representing the product package to measure.</param>
        protected override void ProcessItem(PSObject item)
        {
            var path = item.GetPropertyValue<string>("PSPath");
            var providerPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            var db = base.OpenDatabase(providerPath);
            if (null != db)
            {
                using (db)
                {
                    if (!db.Tables.Contains("InstallExecuteSequence"))
                    {
                        this.WriteInvalidPackageError(providerPath);
                        return;
                    }

                    // Must not ignore machine state since resulting session cannot be used for costing.
                    using (var session = Installer.OpenPackage(db, false))
                    {
                        // Set directories and other properties.
                        if (0 < this.Properties.Count())
                        {
                            foreach (var property in this.Properties)
                            {
                                var pair = property.Split(MeasureProductCommand.PropertySeparator, 2);
                                if (2 == pair.Count())
                                {
                                    // Trim surrounding quotes from the property value.
                                    session[pair[0]] = pair[1].Trim(MeasureProductCommand.PropertyQuotes);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(this.Destination))
                        {
                            var target = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(this.Destination);
                            session["TARGETDIR"] = target;
                        }

                        // Invoke any of AppSearch and type 51 custom actions that might affect costing.
                        var actions = MeasureProductCommand.GetCustomActions(db);
                        using (var view = db.OpenView(MeasureProductCommand.ViewExecuteActions))
                        {
                            string action = null;

                            view.Execute();

                            var record = view.Fetch();
                            while (null != record)
                            {
                                using (record)
                                {
                                    action = record.GetString(1);
                                    if (null != actions && actions.Contains(action))
                                    {
                                        session.DoAction(action);
                                    }
                                    else if ("AppSearch".Equals(action, StringComparison.Ordinal))
                                    {
                                        session.DoAction(action);
                                    }
                                    else if ("CostInitialize".Equals(action, StringComparison.Ordinal))
                                    {
                                        break;
                                    }
                                }

                                record = view.Fetch();
                            }
                        }

                        try
                        {
                            // Execute the main component costing actions.
                            session.DoAction("CostInitialize");
                            session.DoAction("FileCost");
                            session.DoAction("CostFinalize");

                            // Enable default features.
                            session.SetInstallLevel(1);

                            // Documentation is incorrect and InstallValidate is necessary to complete costing.
                            session.DoAction("InstallValidate");
                        }
                        catch (InstallerException)
                        {
                            // Not a valid MSI if these actions aren't present.
                            this.WriteInvalidPackageError(providerPath);
                        }

                        // Get space requirements for all components.
                        foreach (var component in session.Components)
                        {
                            foreach (var costs in component.GetCost(InstallState.Default))
                            {
                                var spaceRequired = this.spaceRequirements.Get(costs.DriveName);

                                spaceRequired.SpaceRequired += costs.Cost;
                                spaceRequired.TemporarySpaceRequired += costs.TempCost;
                            }
                        }

                        // Get space requirements for database copy and script generation.
                        foreach (var costs in session.GetTotalCost())
                        {
                            var spaceRequired = this.spaceRequirements.Get(costs.DriveName);

                            spaceRequired.SpaceRequired += costs.Cost;
                            spaceRequired.TemporarySpaceRequired += costs.TempCost;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the internal user interface and yields the results to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            Installer.SetInternalUI(this.previousInternalUI);

            var drives = this.SessionState.Drive.GetAllForProvider("FileSystem");
            if (null != drives)
            {
                // Only show psdrives that map to their logical drive letters.
                foreach (var drive in drives.Where(d => null != d.Root && d.Root.StartsWith(d.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    var spaceRequirements = this.spaceRequirements.Get(drive.Name);

                    var spaceRequired = spaceRequirements.SpaceRequired;
                    var temporarySpaceRequired = spaceRequirements.TemporarySpaceRequired;

                    var obj = PSObject.AsPSObject(drive);
                    obj.TypeNames.Insert(0, "System.Management.Automation.PSDriveInfo#MSISpaceRequired");

                    var property = new PSNoteProperty("MSISpaceRequired", spaceRequired);
                    obj.Properties.Add(property, true);

                    property = new PSNoteProperty("MSITemporarySpaceRequired", temporarySpaceRequired);
                    obj.Properties.Add(property, true);

                    base.WriteObject(obj);
                }
            }

            base.EndProcessing();
        }

        private static Set<string> GetCustomActions(Database db)
        {
            Set<string> actions = null;
            if (db.Tables.Contains("CustomAction"))
            {
                actions = new Set<string>(StringComparer.Ordinal);
                using (var view = db.OpenView(MeasureProductCommand.ViewCustomActions))
                {
                    view.Execute();

                    var record = view.Fetch();
                    while (null != record)
                    {
                        using (record)
                        {
                            actions.Add(record.GetString(1));
                        }

                        record = view.Fetch();
                    }
                }
            }

            return actions;
        }

        private void WriteInvalidPackageError(string path)
        {
            var message = string.Format(Resources.Error_InvalidPackage, path);
            var ex = new InvalidOperationException(message);
            var error = new ErrorRecord(ex, "InvalidPackage", ErrorCategory.InvalidOperation, path);

            base.WriteError(error);
        }

        private class SpaceRequirements
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SpaceRequirements"/> class.
            /// </summary>
            /// <param name="driveLetter">The drive letter for space requirements.</param>
            internal SpaceRequirements(string driveLetter)
            {
                this.DriveLetter = driveLetter.Substring(0, 1);
                this.SpaceRequired = 0;
                this.TemporarySpaceRequired = 0;
            }

            /// <summary>
            /// Gets the drive letter for space requirements.
            /// </summary>
            internal string DriveLetter { get; private set; }

            /// <summary>
            /// Gets or sets the space required after install completes.
            /// </summary>
            internal long SpaceRequired { get; set; }

            /// <summary>
            /// Gets or sets the space required during install.
            /// </summary>
            internal long TemporarySpaceRequired { get; set; }
        }

        private class SpaceRequirementsCollection : KeyedCollection<string, SpaceRequirements>
        {
            /// <summary>
            /// Gets a new or existing <see cref="SpaceRequirements"/> object.
            /// </summary>
            /// <param name="driveLetter">The drive letter for the <see cref="SpaceRequirements"/>.</param>
            /// <returns>a new or existing <see cref="SpaceRequirements"/> object for the <paramref name="driveLetter"/>.</returns>
            internal SpaceRequirements Get(string driveLetter)
            {
                driveLetter = driveLetter.Substring(0, 1);

                if (this.Contains(driveLetter))
                {
                    return this[driveLetter];
                }
                else
                {
                    var spaceRequirements = new SpaceRequirements(driveLetter);
                    this.Add(spaceRequirements);

                    return spaceRequirements;
                }
            }

            /// <summary>
            /// Gets the key for the <see cref="SpaceRequirements"/>.
            /// </summary>
            /// <param name="item">The <see cref="SpaceRequirements"/> from which the key is obtained.</param>
            /// <returns>The key for the <see cref="SpaceRequirements"/>.</returns>
            protected override string GetKeyForItem(SpaceRequirements item)
            {
                return item.DriveLetter;
            }
        }
    }
}
