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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management;
using System.Management.Automation;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "MSIFileHash",
        DefaultParameterSetName = ParameterSet.Path)]
    public sealed class GetFileHashCommand : CommandBase
    {
        protected override void ProcessRecord()
        {
            foreach (string _path in this.path)
            {
                // with seemingly no other way, check if the path is valid if literal
                if (this.literal && !this.SessionState.Path.IsValid(_path))
                {
                    continue;
                }

                Collection<PSObject> items = this.InvokeProvider.Item.Get(_path);
                foreach (PSObject item in items)
                {
                    FileHashInfo hashInfo = new FileHashInfo();
                    string fsPath = Location.GetOneResolvedProviderPathFromPSObject(item, this);

                    if (null != fsPath && File.Exists(fsPath))
                    {
                        this.CallingNativeFunction("MsiGetFileHash", fsPath, 0);
                        int ret = NativeMethods.MsiGetFileHash(fsPath, 0, hashInfo);
                        if (NativeMethods.ERROR_SUCCESS != ret)
                        {
                            // write the Win32 error message
                            System.ComponentModel.Win32Exception ex = new System.ComponentModel.Win32Exception(ret);
                            PSInvalidOperationException psex = new PSInvalidOperationException(ex.Message, ex);
                            this.WriteError(psex.ErrorRecord);
                        }
                    }

                    // pass everything through so format cmdlets work properly
                    if (this.passThru)
                    {
                        item.Properties.Add(new PSNoteProperty("MSIHashPart1", hashInfo.HashPart1));
                        item.Properties.Add(new PSNoteProperty("MSIHashPart2", hashInfo.HashPart2));
                        item.Properties.Add(new PSNoteProperty("MSIHashPart3", hashInfo.HashPart3));
                        item.Properties.Add(new PSNoteProperty("MSIHashPart4", hashInfo.HashPart4));

                        this.WriteObject(item);
                    }
                    else if (!this.Stopping)
                    {
                        this.WriteObject(hashInfo);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Location_Path",
                ParameterSetName = ParameterSet.Path,
                Mandatory = true,
                Position = 0,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true)]
        public string[] Path
        {
            get { return this.path; }
            set
            {
                this.literal = false;
                this.path = value;
            }
        }
        bool literal;
        string[] path;

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Location_LiteralPath",
                ParameterSetName = ParameterSet.LiteralPath,
                Mandatory = true,
                Position = 0,
                ValueFromPipelineByPropertyName = true)]
        [Alias("PSPath")]
        public string[] LiteralPath
        {
            get { return this.path; }
            set
            {
                this.literal = true;
                this.path = value;
            }
        }

        [Parameter]
        public SwitchParameter PassThru
        {
            get { return passThru; }
            set { passThru = value; }
        }
        bool passThru;
    }
}
