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
using System.IO;
using System.Management;
using System.Management.Automation;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "MSIFileType",
        DefaultParameterSetName = Location.PathParameterSet)]
    public sealed class GetFileTypeCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            foreach (string path in this.path)
            {
                // with seemingly no other way, check if the path is valid if literal
                if (this.literal && !this.SessionState.Path.IsValid(path))
                {
                    continue;
                }

                Collection<PSObject> items = this.InvokeProvider.Item.Get(path);
                foreach (PSObject item in items)
                {
                    // Get the file system path.
                    PSPropertyInfo psPathInfo = item.Properties["PSPath"];
                    string psPath = null != psPathInfo ? psPathInfo.Value as string : null;

                    string fileType = null;
                    if (null != psPath)
                    {
                        ProviderInfo provider;
                        Collection<string> fsPaths = this.GetResolvedProviderPathFromPSPath(psPath, out provider);

                        if (null != fsPaths && 0 < fsPaths.Count && File.Exists(fsPaths[0]))
                        {
                            string fsPath = fsPaths[0];
                            Guid clsid = Guid.Empty;

                            try
                            {
                                Storage stg = Storage.OpenStorage(fsPath, true);
                                clsid = stg.Clsid;
                            }
                            catch (System.ComponentModel.Win32Exception ex)
                            {
                                // non-terminating error; continue to the next file
                                string message = ex.Message.Replace("%1", fsPath);
                                PSInvalidOperationException psex = new PSInvalidOperationException(ex.Message, ex);
                                this.WriteError(psex.ErrorRecord);
                            }

                            if (Msi.CLSID_MsiPackage == clsid)
                            {
                                    fileType = Msi.MsiPackage;
                            }
                            else if (Msi.CLSID_MsiPatch == clsid)
                            {
                                    fileType = Msi.MsiPatch;
                            }
                            else if (Msi.CLSID_MsiTransform == clsid)
                            {
                                    fileType = Msi.MsiTransform;
                            }
                        }
                    }

                    if (this.passThru)
                    {
                        item.Properties.Add(new PSNoteProperty("MSIFileType", fileType));
                        this.WriteObject(item);
                    }
                    else
                    {
                        this.WriteObject(fileType);
                    }
                }
            }
        }

        string[] path = null;
        bool literal = false;
        bool passThru = false;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "Location_Path",
                ParameterSetName = Location.PathParameterSet,
                Mandatory = true,
                Position = 0,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true)]
        public string[] Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "Location_Path",
                ParameterSetName = Location.LiteralPathParameterSet,
                Mandatory = true,
                Position = 0,
                ValueFromPipeline = false,
                ValueFromPipelineByPropertyName = true),
        Alias("PSPath")]
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
    }
}
