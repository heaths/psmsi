// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIRecord cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSITable", DefaultParameterSetName = "Path,Table")]
    [OutputType(typeof(Record))]
    public sealed class GetTableCommand : ItemCommandBase
    {
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
        /// Gets or sets the table name from which all <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", Mandatory = true)]
        [Parameter(ParameterSetName = "LiteralPath,Table", Mandatory = true)]
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the query from which all <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Query", Mandatory = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", Mandatory = true)]
        public string Query { get; set; }

        /// <summary>
        /// Opens the database specified by the <paramref name="item"/> and executes the specified query.
        /// </summary>
        /// <param name="item">A file item that references a package database.</param>
        protected override void ProcessItem(PSObject item)
        {
            var path = item.GetPropertyValue<string>("PSPath");
            var providerPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            using (var db = new Database(providerPath, DatabaseOpenMode.ReadOnly))
            {
                var query = this.GetQuery(db);
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
                                var copy = new Record(record, columns);

                                // Add additional properties to the Members collection; otherwise,
                                // existing adapted properties are copied along with cached values.
                                var obj = PSObject.AsPSObject(copy);
                                obj.Members.Add(new PSNoteProperty("MSIPath", providerPath));
                                obj.Members.Add(new PSNoteProperty("MSIQuery", query));

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

        private string GetQuery(Database db)
        {
            if (0 <= this.ParameterSetName.IndexOf("Query", StringComparison.InvariantCultureIgnoreCase))
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
                    string message = string.Format(Resources.Error_TableNotFound, this.Table, db.FilePath);
                    var ex = new PSArgumentException(message, "Table");

                    this.WriteError(ex.ErrorRecord);
                }
            }

            return null;
        }
    }
}
