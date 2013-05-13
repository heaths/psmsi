// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIRecord cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIRecord", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class GetRecordCommand : ItemCommandBase
    {
        /// <summary>
        /// Gets or sets the table names from which all <see cref="Record"/> objects are selected.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string[] Table { get; set; }

        /// <summary>
        /// Opens the MSI or MSP database and gets all <see cref="Record"/> objects.
        /// </summary>
        /// <param name="item"></param>
        protected override void ProcessItem(PSObject item)
        {
            var info = item.BaseObject as System.IO.FileInfo;
            if (null != info)
            {
                using (var db = new Database(info.FullName, DatabaseOpenMode.ReadOnly))
                {
                    foreach (string name in this.Table)
                    {
                        if (db.Tables.Contains(name))
                        {
                            // Get the proper SELECT string for the table.
                            var table = db.Tables[name];
                            using (var view = db.OpenView(table.SqlSelectString))
                            {
                                view.Execute();

                                // Get the first record.
                                var record = view.Fetch();

                                while (null != record)
                                {
                                    this.WriteObject(record);

                                    // Get the next record.
                                    record = view.Fetch();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
