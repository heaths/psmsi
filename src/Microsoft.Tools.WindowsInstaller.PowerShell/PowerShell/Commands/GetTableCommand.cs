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
            string path = item.GetPropertyValue<string>("PSPath");
            path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            // No handles are disposed or the property adapter will throw.
            var db = new Database(path, DatabaseOpenMode.ReadOnly);

            string query = this.GetQuery(db);
            if (!string.IsNullOrEmpty(query))
            {
                var view = db.OpenView(query);
                view.Execute();

                var record = view.Fetch();
                while (null != record)
                {
                    var obj = PSObject.AsPSObject(record);
                    this.WriteObject(obj);

                    record = view.Fetch();
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
