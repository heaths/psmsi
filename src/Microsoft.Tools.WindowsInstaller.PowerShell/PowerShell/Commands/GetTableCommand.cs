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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIRecord cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSITable", DefaultParameterSetName = "Path,Table")]
    [OutputType(typeof(Record))]
    public sealed class GetTableCommand : PackageCommandBase
    {
        private static readonly MethodInfo methodGetPath = typeof(GetTableCommand).GetMethod("GetPath", BindingFlags.Public | BindingFlags.Static);
        private static readonly MethodInfo methodGetQuery = typeof(GetTableCommand).GetMethod("GetQuery", BindingFlags.Public | BindingFlags.Static);
        private InstallUIOptions previousInternalUI = InstallUIOptions.Default;

        /// <summary>
        /// Gets the path to the package that contains this <see cref="Record"/> for use in code methods.
        /// </summary>
        /// <param name="obj">The <see cref="PSObject"/> that wraps a <see cref="Record"/>.</param>
        /// <returns>The path to the package that contains this <see cref="Record"/>.</returns>
        public static string GetPath(PSObject obj)
        {
            var record = obj.As<Record>();
            if (null != record)
            {
                return record.Path;
            }

            return null;
        }
        /// <summary>
        /// Gets the query string that returned the <see cref="Record"/> for use in code methods.
        /// </summary>
        /// <param name="obj">The <see cref="PSObject"/> that wraps a <see cref="Record"/>.</param>
        /// <returns>The query string that returned the <see cref="Record"/>.</returns>
        public static string GetQuery(PSObject obj)
        {
            var record = obj.As<Record>();
            if (null != record && null != record.Columns)
            {
                return record.Columns.QueryString;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Path,Query", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public override string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [Alias("PSPath")]
        [Parameter(ParameterSetName = "LiteralPath,Table", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override string[] LiteralPath
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ProductInstallation"/> to query.
        /// </summary>
        [Parameter(ParameterSetName = "Installation,Table", Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Installation,Query", Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public ProductInstallation[] Product { get; set; }

        /// <summary>
        /// Gets or sets the table name from which all <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", Mandatory = true)]
        [Parameter(ParameterSetName = "LiteralPath,Table", Mandatory = true)]
        [Parameter(ParameterSetName = "Installation,Table", Mandatory = true)]
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the query from which all <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Query", Mandatory = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", Mandatory = true)]
        [Parameter(ParameterSetName = "Installation,Query", Mandatory = true)]
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets patch packages to apply before <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table")]
        [Parameter(ParameterSetName = "Path,Query")]
        [Parameter(ParameterSetName = "LiteralPath,Table")]
        [Parameter(ParameterSetName = "LiteralPath,Query")]
        [ValidateNotNullOrEmpty]
        public override string[] Patch
        {
            get { return base.Patch; }
            set { base.Patch = value; }
        }

        /// <summary>
        /// Gets or sets transforms to apply before <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table")]
        [Parameter(ParameterSetName = "Path,Query")]
        [Parameter(ParameterSetName = "LiteralPath,Table")]
        [Parameter(ParameterSetName = "LiteralPath,Query")]
        [ValidateNotNullOrEmpty]
        public override string[] Transform
        {
            get { return base.Transform; }
            set { base.Transform = value; }
        }

        /// <summary>
        /// Gets or sets whether to ignore machine state when opening the <see cref="ProductInstallation"/>.
        /// </summary>
        [Parameter(ParameterSetName = "Installation,Table")]
        [Parameter(ParameterSetName = "Installation,Query")]
        public SwitchParameter IgnoreMachineState { get; set; }

        /// <summary>
        /// Sets up the user interface handlers.
        /// </summary>
        protected override void BeginProcessing()
        {
            // Set up the UI handlers.
            this.previousInternalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
            base.BeginProcessing();
        }

        /// <summary>
        /// Opens an installed <see cref="Product"/> or the database specified by the <see cref="Path"/> or <see cref="LiteralPath"/>.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.IsInstallation)
            {
                foreach (var product in this.Product)
                {
                    var session = this.OpenProduct(product);
                    if (null != session)
                    {
                        using (session)
                        {
                            this.WriteRecords(session.Database, product.LocalPackage);
                        }
                    }
                }
            }
            else
            {
                base.ProcessRecord();
            }
        }

        /// <summary>
        /// Opens the database specified by the <paramref name="item"/> and executes the specified query.
        /// </summary>
        /// <param name="item">A file item that references a package database.</param>
        protected override void ProcessItem(PSObject item)
        {
            var path = item.GetPropertyValue<string>("PSPath");
            var providerPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            using (var db = this.OpenDatabase(providerPath))
            {
                this.WriteRecords(db, providerPath);
            }
        }

        /// <summary>
        /// Restores the previous user interface handlers.
        /// </summary>
        protected override void EndProcessing()
        {
            Installer.SetInternalUI(this.previousInternalUI);
            base.EndProcessing();
        }

        private bool IsInstallation
        {
            get
            {
                return null != this.ParameterSetName
                    && 0 <= this.ParameterSetName.IndexOf(ParameterSet.Installation, StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool IsQuery
        {
            get
            {
                return null != this.ParameterSetName
                    && 0 <= this.ParameterSetName.IndexOf("Query", StringComparison.OrdinalIgnoreCase);
            }
        }

        private string GetQuery(Database db, string path)
        {
            if (this.IsQuery)
            {
                return this.Query;
            }
            else
            {
                if (db.Tables.Contains(this.Table))
                {
                    return db.Tables[this.Table].SqlSelectString;
                }
                else
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TableNotFound, this.Table, path);
                    var ex = new PSArgumentException(message, "Table");

                    this.WriteError(ex.ErrorRecord);
                }
            }

            return null;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private Database OpenDatabase(string path)
        {
            var type = FileInfo.GetFileTypeInternal(path);
            if (FileType.Package == type)
            {
                var db = new InstallPackage(path, DatabaseOpenMode.ReadOnly);
                base.ApplyTransforms(db);

                return db;
            }
            else
            {
                // Let Windows Installer thow any exceptions if not a valid package.
                return new Database(path, DatabaseOpenMode.ReadOnly);
            }
        }

        private Session OpenProduct(ProductInstallation product)
        {
            // Open the product taking machine state into account.
            var path = product.LocalPackage;
            if (!string.IsNullOrEmpty(path))
            {
                return Installer.OpenPackage(path, this.IgnoreMachineState);
            }
            else if (product.IsAdvertised && !String.IsNullOrEmpty(product.AdvertisedPackageName))
            {
                // Product is advertised and has no locally installed package.
                var message = string.Format(Properties.Resources.Error_Advertised, product.ProductCode);
                this.WriteWarning(message);
            }
            else
            {
                // Product registration is corrupt.
                var message = string.Format(Properties.Resources.Error_Corrupt, product.ProductCode);
                var ex = new Exception(message, new Win32Exception(NativeMethods.ERROR_BAD_CONFIGURATION));
                var error = new ErrorRecord(ex, "Error_Corrupt", ErrorCategory.NotInstalled, product);

                this.WriteError(error);
            }

            return null;
        }

        private void WriteRecords(Database db, string path)
        {
            var query = this.GetQuery(db, path);
            if (!string.IsNullOrEmpty(query))
            {
                using (var view = db.OpenView(query))
                {
                    view.Execute();

                    // Get column information from the view before being disposed.
                    var columns = ViewManager.GetColumns(view);

                    var record = view.Fetch();
                    while (null != record)
                    {
                        using (record)
                        {
                            // Create a locally cached copy of the record.
                            var copy = new Record(record, columns, path);

                            // Add additional properties to the Members collection; otherwise,
                            // existing adapted properties are copied along with cached values.
                            var obj = PSObject.AsPSObject(copy);
                            obj.Members.Add(new PSCodeProperty("MSIPath", methodGetPath));
                            obj.Members.Add(new PSCodeProperty("MSIQuery", methodGetQuery));

                            // Show only column properties by default.
                            var memberSet = ViewManager.GetMemberSet(view);
                            obj.Members.Add(memberSet);

                            this.WriteObject(obj);
                        }

                        record = view.Fetch();
                    }
                }
            }
        }
    }
}
