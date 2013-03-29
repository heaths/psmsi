// Cmdlet to get the storage class type for a file, optionally passing
// the PSObject back through the pipeline with a new NoteProperty.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.ComponentModel;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIFileHash cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIFileHash", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class GetFileHashCommand : ItemCommandBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GetFileHashCommand"/> class.
        /// </summary>
        public GetFileHashCommand() : base()
        {
        }

        /// <summary>
        /// Processes the item enumerated by the base class.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> to process.</param>
        /// <param name="path">The provider path from the PSPath attached to the <paramref name="item"/>.</param>
        protected override void ProcessItem(PSObject item, string path)
        {
            FileHash hash = new FileHash();

            // Only process files.
            if (!this.SessionState.InvokeProvider.Item.IsContainer(path))
            {
                int ret = NativeMethods.MsiGetFileHash(path, 0, hash);
                if (ret != NativeMethods.ERROR_SUCCESS)
                {
                    // Write the error record and continue enumerating files.
                    Win32Exception ex = new Win32Exception(ret);

                    string message = ex.Message.Replace("%1", path);
                    PSInvalidOperationException psex = new PSInvalidOperationException(message, ex);

                    this.WriteError(psex.ErrorRecord);
                }

                // Write only the hash if not passing the input through.
                if (!this.PassThru)
                {
                    this.WriteObject(hash);
                }
            }

            // Attach NoteProperties if passing the input through.
            if (this.PassThru)
            {
                item.Properties.Add(new PSNoteProperty("MSIHashPart1", hash.MSIHashPart1));
                item.Properties.Add(new PSNoteProperty("MSIHashPart2", hash.MSIHashPart2));
                item.Properties.Add(new PSNoteProperty("MSIHashPart3", hash.MSIHashPart3));
                item.Properties.Add(new PSNoteProperty("MSIHashPart4", hash.MSIHashPart4));

                this.WriteObject(item);
            }
        }
    }
}
