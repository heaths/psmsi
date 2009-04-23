// Cmdlet to get the storage class type for a file, optionally passing
// the PSObject back through the pipeline with a new NoteProperty.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Sat, 12 Jan 2008 17:09:56 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIFileType cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIFileType", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class GetFileTypeCommand : ItemCommandBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GetFileTypeCommand"/> class.
        /// </summary>
        public GetFileTypeCommand() : base()
        {
        }

        /// <summary>
        /// Processes the item enumerated by the base class.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        protected override void ProcessItem(PSObject item, string path)
        {
            string fileType = null;

            // Only process files.
            if (!this.SessionState.InvokeProvider.Item.IsContainer(path))
            {
                try
                {
                    Storage stg = Storage.OpenStorage(path);
                    Guid clsid = stg.Clsid;

                    // Set the friendly name.
                    if (clsid == NativeMethods.CLSID_MsiPackage)
                    {
                        fileType = Properties.Resources.Type_Package;
                    }
                    else if (clsid == NativeMethods.CLSID_MsiPatch)
                    {
                        fileType = Properties.Resources.Type_Patch;
                    }
                    else if (clsid == NativeMethods.CLSID_MsiTransform)
                    {
                        fileType = Properties.Resources.Type_Transform;
                    }
                }
                catch (Win32Exception ex)
                {
                    string message = ex.Message.Replace("%1", path);
                    PSInvalidOperationException psex = new PSInvalidOperationException(message, ex);

                    // Write the error record and continue.
                    this.WriteError(psex.ErrorRecord);
                }

                // Write only the file type if not passing the input through.
                if (!this.PassThru)
                {
                    this.WriteObject(fileType);
                }
            }

            // Attach NoteProperty if passing the input through.
            if (this.PassThru)
            {
                item.Properties.Add(new PSNoteProperty("WIFileType", fileType));
                this.WriteObject(item);
            }
        }
    }
}
