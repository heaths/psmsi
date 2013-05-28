// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIFileHash cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIFileHash", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(FileHash), typeof(FileSystemInfo))]
    public sealed class GetFileHashCommand : ItemCommandBase
    {
        /// <summary>
        /// Gets or sets whether the file objects are returned.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Processes the item enumerated by the base class.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> to process.</param>
        protected override void ProcessItem(PSObject item)
        {
            if (this.PassThru)
            {
                this.WriteObject(item);
            }
            else if (null != item.Properties["MSIFileHash"])
            {
                try
                {
                    // Return the file type from ETS.
                    this.WriteObject(item.Properties["MSIFileHash"].Value);
                }
                catch (PSNotSupportedException ex)
                {
                    this.WriteError(ex.ErrorRecord);
                }
            }
        }
    }
}
