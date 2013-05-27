// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Test-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class TestProductCommand : ItemCommandBase
    {
        private InstallUIOptions previousInternalUI = InstallUIOptions.Default;
        private ExternalUIRecordHandler previousExternalUI = null;

        // Used by nested classes.
        private Queue<Data> Output = new Queue<Data>();
        private string CurrentPath = null;

        /// <summary>
        /// Gets or sets additional ICE .cub files to use for validation.
        /// </summary>
        [Parameter, Alias("Cube"), ValidateNotNullOrEmpty]
        public string[] AdditionalCube { get; set; }

        /// <summary>
        /// Gets or sets whether to include the default ICE .cub file installed by Orca or MsiVal2.
        /// </summary>
        [Parameter]
        public SwitchParameter NoDefault { get; set; }

        /// <summary>
        /// Gets or sets the wilcard patterns of ICEs to include. By default, all ICEs are included.
        /// </summary>
        [Parameter, ValidateNotNullOrEmpty]
        public string[] Include { get; set; }

        /// <summary>
        /// Gets or sets the wilcard patterns of ICEs to exclude. By default, all ICEs are included.
        /// </summary>
        [Parameter, ValidateNotNullOrEmpty]
        public string[] Exclude { get; set; }

        /// <summary>
        /// Gets or sets patch packages to apply before validation.
        /// </summary>
        [Parameter, ValidateNotNullOrEmpty]
        public string[] Patch { get; set; }

        /// <summary>
        /// Gets or sets transforms to apply before validation.
        /// </summary>
        [Parameter, ValidateNotNullOrEmpty]
        public string[] Transform { get; set; }

        /// <summary>
        /// Gets whether the standard Verbose parameter was set.
        /// </summary>
        private bool IsVerbose
        {
            get
            {
                var bound = this.MyInvocation.BoundParameters;
                return bound.ContainsKey("Verbose") && (bool)(SwitchParameter)bound["Verbose"];
            }
        }

        /// <summary>
        /// Sets up the user interface handlers.
        /// </summary>
        protected override void BeginProcessing()
        {
            // Set up the UI handlers.
            this.previousInternalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
            this.previousExternalUI = Installer.SetExternalUI(this.OnMessage, InstallLogModes.FatalExit | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.User);

            base.BeginProcessing();
        }

        /// <summary>
        /// Merges ICE cubes into the database <paramref name="item"/> and executes selected ICEs.
        /// </summary>
        /// <param name="item">The database to validate.</param>
        protected override void ProcessItem(PSObject item)
        {
            // Get the item path and set the current context.
            string path = item.GetPropertyValue<string>("PSPath");
            path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
            this.CurrentPath = path;

            // Copy the database to a writable location and open.
            string copy = this.Copy(path);
            using (var db = new InstallPackage(copy, DatabaseOpenMode.Direct))
            {
                // Apply any patches or transforms before otherwise modifying.
                this.ApplyTransforms(db);

                // Copy the ProductCode and drop the Property table to avoid opening an installed product.
                bool hasProperty = db.IsTablePersistent("Property");
                string productCode = null;

                if (hasProperty)
                {
                    productCode = db.ExecutePropertyQuery("ProductCode");
                }

                // Merge the ICE cubes and fix up the database if needed.
                this.MergeCubes(db);
                if (!hasProperty)
                {
                    db.Execute("DROP TABLE `Property`");
                }

                var included = new List<WildcardPattern>();
                if (null != this.Include)
                {
                    Array.ForEach(this.Include, pattern => included.Add(new WildcardPattern(pattern)));
                }

                var excluded = new List<WildcardPattern>();
                if (null != this.Exclude)
                {
                    Array.ForEach(this.Exclude, pattern => excluded.Add(new WildcardPattern(pattern)));
                }

                // Get all the ICE actions in the database that are not excluded.
                var actions = new List<string>();
                foreach (var action in db.ExecuteStringQuery("SELECT `Action` FROM `_ICESequence` ORDER BY `Sequence`"))
                {
                    if (!action.Match(excluded))
                    {
                        actions.Add(action);
                    }
                }

                // Remove any actions not explicitly included.
                if (0 < included.Count)
                {
                    for (int i = actions.Count - 1; 0 <= i; --i)
                    {
                        if (!actions[i].Match(included))
                        {
                            actions.RemoveAt(i);
                        }
                    }
                }

                // Open a session with the database.
                using (var session = Installer.OpenPackage(db, false))
                {
                    // Put the original ProductCode back.
                    if (!string.IsNullOrEmpty(productCode))
                    {
                        db.Execute("DELETE FROM `Property` WHERE `Property` = 'ProductCode'");
                        db.Execute("INSERT INTO `Property` (`Property`, `Value`) VALUES ('ProductCode', '{0}')", productCode);
                    }

                    // Now execute all the remaining actions in order.
                    foreach (string action in actions)
                    {
                        try
                        {
                            session.DoAction(action);
                            this.Flush();
                        }
                        catch (InstallerException ex)
                        {
                            var psex = new PSInstallerException(ex);
                            if (null != psex.ErrorRecord)
                            {
                                this.WriteError(psex.ErrorRecord);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restores the previous user interface handlers.
        /// </summary>
        protected override void EndProcessing()
        {
            Installer.SetInternalUI(this.previousInternalUI);
            Installer.SetExternalUI(this.previousExternalUI, InstallLogModes.None);

            base.EndProcessing();
        }

        private static string GetDefaultCubePath()
        {
            // Try to find darice.cub.
            var component = new ComponentInstallation("{D865CA5E-9B46-B345-B3A6-43C5EAF209E0}");
            if (InstallState.Local == component.State)
            {
                return component.Path;
            }

            return null;
        }

        private void ApplyTransforms(InstallPackage db)
        {
            // Apply transforms first since they likely apply to the unpatched product.
            if (null != this.Transform)
            {
                foreach (string path in this.GetFiles(this.Transform))
                {
                    try
                    {
                        db.ApplyTransform(path, PatchApplicator.IgnoreErrors);
                    }
                    catch (InstallerException ex)
                    {
                        var psex = new PSInstallerException(ex);
                        if (null != psex.ErrorRecord)
                        {
                            this.WriteError(psex.ErrorRecord);
                        }
                    }
                }

                db.Commit();
            }

            // Apply applicable patch transforms.
            if (null != this.Patch)
            {
                var applicator = new PatchApplicator(db);
                foreach (string path in this.GetFiles(this.Patch))
                {
                    applicator.Add(path);
                }

                applicator.InapplicablePatch += (source, args) =>
                    {
                        string message = string.Format(Resources.Error_InapplicablePatch, args.Patch, args.Product);
                        this.WriteVerbose(message);
                    };

                // The applicator will commit the changes.
                applicator.Apply();
            }
        }

        private string Copy(string path)
        {
            string temp = System.IO.Path.GetTempPath();
            string name = System.IO.Path.GetFileName(path);
            string copy = System.IO.Path.Combine(temp, name);

            // Copy and overwrite the file into the TEMP directory.
            this.WriteVerbose(string.Format(Resources.Action_Copy, path, copy));
            File.Copy(path, copy, true);

            // Unset the read-only attribute.
            var attributes = File.GetAttributes(copy);
            File.SetAttributes(copy, attributes & ~System.IO.FileAttributes.ReadOnly);

            return copy;
        }

        private IEnumerable<string> GetFiles(IEnumerable<string> paths)
        {
            ProviderInfo provider;
            foreach (string path in paths)
            {
                foreach (string resolvedPath in this.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider))
                {
                    yield return resolvedPath;
                }
            }
        }

        private void MergeCube(Database db, string path)
        {
            using (var cube = new Database(path, DatabaseOpenMode.ReadOnly))
            {
                try
                {
                    this.WriteVerbose(string.Format(Resources.Action_Merge, path, db.FilePath));
                    db.Merge(cube, "MergeConflicts");
                }
                catch
                {
                }
            }
        }

        private void MergeCubes(InstallPackage db)
        {
            if (!this.NoDefault)
            {
                string darice = GetDefaultCubePath();
                if (!string.IsNullOrEmpty(darice))
                {
                    this.MergeCube(db, darice);
                }
                else
                {
                    this.WriteWarning(Resources.Error_DefaultCubNotFound);
                }
            }

            if (null != this.AdditionalCube)
            {
                foreach (string cube in this.GetFiles(this.AdditionalCube))
                {
                    this.MergeCube(db, cube);
                }

                db.Commit();
            }
        }

        private MessageResult OnMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            switch (messageType)
            {
                case InstallMessage.FatalExit:
                case InstallMessage.Error:
                    return this.OnError(messageRecord);

                case InstallMessage.Warning:
                    return this.OnWarning(messageRecord);

                case InstallMessage.User:
                    return this.OnInformation(messageRecord);

                default:
                    return MessageResult.None;
            }
        }

        private MessageResult OnError(Record record)
        {
            if (null != record)
            {
                using (var ex = new PSInstallerException(record))
                {
                    if (null != ex.ErrorRecord)
                    {
                        var data = new Data(DataType.Error, ex.ErrorRecord);
                        this.Output.Enqueue(data);
                    }
                }
            }

            return MessageResult.OK;
        }

        private MessageResult OnWarning(Record record)
        {
            if (null != record)
            {
                string message = record.ToString();

                var data = new Data(DataType.Warning, message);
                this.Output.Enqueue(data);
            }

            return MessageResult.OK;
        }

        private MessageResult OnInformation(Record record)
        {
            if (null != record)
            {
                string message = record.ToString();
                if (!string.IsNullOrEmpty(message))
                {
                    var ice = new IceMessage(message);
                    var obj = PSObject.AsPSObject(ice);

                    if (!string.IsNullOrEmpty(this.CurrentPath))
                    {
                        string path = this.SessionState.Path.GetUnresolvedPSPathFromProviderPath(this.CurrentPath);
                        obj.SetPropertyValue<string>("PSPath", path);
                    }

                    var data = new Data(DataType.Information, obj);
                    this.Output.Enqueue(data);
                }
            }

            return MessageResult.OK;
        }

        private void Flush()
        {
            // Since the session runs in a separate thread, data enqueued in an output queue
            // and must be dequeued in the pipeline execution thread.
            while (0 < this.Output.Count)
            {
                var data = this.Output.Dequeue();
                switch (data.Type)
                {
                    case DataType.Error:
                        this.WriteError((ErrorRecord)data.Output);
                        break;

                    case DataType.Warning:
                        this.WriteWarning((string)data.Output);
                        break;

                    case DataType.Information:
                        var obj = data.Output as PSObject;
                        if (null != obj && obj.BaseObject is IceMessage)
                        {
                            this.WriteIceMessage(obj);
                        }
                        else if (null != data.Output)
                        {
                            this.WriteVerbose(data.Output.ToString());
                        }
                        break;
                }
            }
        }

        private void WriteIceMessage(PSObject obj)
        {
            var ice = obj.BaseObject as IceMessage;
            if (null != ice)
            {
                if (IceMessageType.Information == ice.Type)
                {
                    if (this.IsVerbose)
                    {
                        this.WriteObject(ice);
                    }
                }
                else
                {
                    this.WriteObject(ice);
                }
            }
        }

        private class Data
        {
            internal Data(DataType type, object output)
            {
                this.Type = type;
                this.Output = output;
            }

            internal DataType Type { get; private set; }
            internal object Output { get; private set; }
        }

        private enum DataType
        {
            Error,
            Warning,
            Information,
        }
    }
}
