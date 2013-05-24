// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Tools.WindowsInstaller.Properties;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIRecord cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSITable", DefaultParameterSetName = "Path,Table")]
    public sealed class GetTableCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [Parameter(ParameterSetName = "Path,Table", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Path,Query", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [Alias("PSPath")]
        [Parameter(ParameterSetName = "LiteralPath,Table", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,Query", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string[] LiteralPath { get; set; }

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
        /// Processes each file item in the pipeline to select table records.
        /// </summary>
        protected override void ProcessRecord()
        {
            bool isliteral = 0 <= this.ParameterSetName.IndexOf("LiteralPath", StringComparison.InvariantCultureIgnoreCase);
            var paths = isliteral ? this.LiteralPath : this.Path;

            var items = this.InvokeProvider.Item.Get(paths, true, isliteral);
            foreach (var obj in items)
            {
                string path = obj.GetPropertyValue<string>("PSPath");
                if (!string.IsNullOrEmpty(path))
                {
                    path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
                }
                else if (obj.BaseObject is System.IO.FileInfo)
                {
                    path = ((System.IO.FileInfo)obj.BaseObject).FullName;
                }

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    this.ProcessFile(path);
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

        private void ProcessFile(string path)
        {
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
    }
}
