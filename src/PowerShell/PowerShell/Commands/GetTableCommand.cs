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
using System.ComponentModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSITable cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSITable", DefaultParameterSetName = "Path,Table")]
    [OutputType(typeof(Record), typeof(TableInfo))]
    public sealed class GetTableCommand : PackageCommandBase
    {
        private InstallUIOptions previousInternalUI = InstallUIOptions.Default;

        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Path,Query", Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [Alias("PSPath")]
        [Parameter(ParameterSetName = "LiteralPath,Table", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override string[] LiteralPath
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ProductInstallation"/> to query.
        /// </summary>
        [Parameter(ParameterSetName = "Installation,Table", Position = 1, Mandatory = true, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Installation,Query", Position = 1, Mandatory = true, ValueFromPipeline = true)]
        public ProductInstallation[] Product { get; set; }

        // TODO: Next major release Table should be the first parameter like Get-WMIObject -Class.

        /// <summary>
        /// Gets or sets the table name from which all <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Table", Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Installation,Table", Position = 0, ValueFromPipelineByPropertyName = true)]
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
        [Parameter(ParameterSetName = "Path,Table", ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Path,Query", ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Table", ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", ValueFromPipelineByPropertyName = true)]
        public override string[] Patch
        {
            get { return base.Patch; }
            set { base.Patch = value; }
        }

        /// <summary>
        /// Gets or sets transforms to apply before <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Path,Query", ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Table", ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", ValueFromPipelineByPropertyName = true)]
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

            var db = base.OpenDatabase(providerPath);
            if (null != db)
            {
                using (db)
                {
                    this.WriteRecords(db, providerPath);
                }
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
            else if (!string.IsNullOrEmpty(this.Table))
            {
                if (db.Tables.Contains(this.Table))
                {
                    return db.Tables[this.Table].SqlSelectString;
                }
                else
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TableNotFound, this.Table, path);
                    throw new PSArgumentException(message, "Table");
                }
            }

            return null;
        }

        private Session OpenProduct(ProductInstallation product)
        {
            // Open the product taking machine state into account.
            var path = product.LocalPackage;
            if (!string.IsNullOrEmpty(path))
            {
                return Installer.OpenPackage(path, this.IgnoreMachineState);
            }
            else if (product.IsAdvertised && !string.IsNullOrEmpty(product.AdvertisedPackageName))
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
            TransformView transform = null;
            if (db.Tables.Contains(TransformView.TableName))
            {
                transform = new TransformView(db);
            }

            string query = null;
            try
            {
                query = this.GetQuery(db, path);
            }
            catch (PSArgumentException ex)
            {
                base.WriteError(ex.ErrorRecord);
                return;
            }

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
                            var copy = new Record(record, columns, transform, path);
                            var obj = PSObject.AsPSObject(copy);

                            // Show only column properties by default.
                            var memberSet = ViewManager.GetMemberSet(view);
                            obj.Members.Add(memberSet, true);

                            this.WriteObject(obj);
                        }

                        record = view.Fetch();
                    }
                }
            }
            else
            {
                foreach (var table in db.Tables)
                {
                    if (db.IsTablePersistent(table.Name))
                    {
                        var info = new TableInfo(table.Name, path, transform, this.Patch, this.Transform);
                        var obj = PSObject.AsPSObject(info);

                        this.WriteObject(obj);
                    }
                }
            }
        }
    }
}
