// Cmdlet to get or enumerator Windows Installer patches.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 22:08:18 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "MSIPatchInfo",
        DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
    public sealed class GetPatchCommand : EnumCommand<PatchInfo>
    {
        string currentProductCode;

        protected override void ProcessRecord()
        {
            // Enumerate patches for input product codes.
            if (ParameterSet.ProductCode == this.ParameterSetName)
            {
                foreach (string productCode in this.productCodes)
                {
                    // If patch codes where specified, just write those to the pipeline.
                    if (null != this.patchCodes && 0 < this.patchCodes.Length)
                    {
                        foreach (string patchCode in this.patchCodes)
                        {
                            // Write out patches for each patch code in a given context.
                            WritePSObject(new PatchInfo(patchCode, productCode, userSid, context));
                        }
                    }
                    else
                    {
                        this.currentProductCode = productCode;

                        // Enumerate all products on the system.
                        base.ProcessRecord();
                    }
                }
            }
            // Enumerate all patches in the given context.
            else
            {
                base.ProcessRecord();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetPatch_ProductCode",
                ParameterSetName = ParameterSet.ProductCode,
                Position = 0,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] ProductCode
        {
            get { return productCodes; }
            set { productCodes = value; }
        }
        string[] productCodes;

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetPatch_PatchCode",
                Position = 1,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] PatchCode
        {
            get { return patchCodes; }
            set { patchCodes = value; }
        }
        string[] patchCodes;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Context_UserSid",
                ValueFromPipelineByPropertyName = true)]
        public string UserSid
        {
            get { return userSid; }
            set { userSid = value; }
        }
        string userSid;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Context_InstallContext",
                ValueFromPipelineByPropertyName = true)]
        public InstallContext InstallContext
        {
            get { return context; }
            set { context = value; }
        }
        InstallContext context = InstallContext.Machine;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetPatch_Filter",
                ValueFromPipelineByPropertyName = true)]
        public PatchStates Filter
        {
            get { return filter; }
            set { filter = value; }
        }
        PatchStates filter = PatchStates.Applied;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Context_Everyone")]
        public SwitchParameter Everyone
        {
            get { return string.Compare(userSid, NativeMethods.World, StringComparison.OrdinalIgnoreCase) == 0; }
            set
            {
                if (value)
                {
                    userSid = NativeMethods.World;
                }
                else
                {
                    userSid = null;
                }
            }
        }

        protected override int Enumerate(int index, out PatchInfo patch)
        {
            int ret = 0;
            StringBuilder pac = new StringBuilder(NativeMethods.MAX_GUID_CHARS + 1);
            StringBuilder prc = new StringBuilder(NativeMethods.MAX_GUID_CHARS + 1);
            InstallContext ctx = InstallContext.None;
            int cch = 0;

            patch = null;
            if (Msi.CheckVersion(3, 0))
            {
                // Use MsiEnumPatchesEx for MSI 3.0 and newer
                StringBuilder sid = new StringBuilder(80);
                cch = sid.Capacity;

                this.CallingNativeFunction("MsiEnumPatchesEx", this.currentProductCode, this.userSid, (int)this.context, (int)this.filter, index);
                ret = NativeMethods.MsiEnumPatchesEx(this.currentProductCode, this.userSid, this.context,
                        this.filter, index, pac, prc, out ctx, sid, ref cch);

                if (NativeMethods.ERROR_MORE_DATA == ret)
                {
                    pac.Length = 0;
                    prc.Length = 0;
                    sid.Capacity = ++cch;

                    ret = NativeMethods.MsiEnumPatchesEx(this.currentProductCode, this.userSid, this.context,
                            this.filter, index, pac, prc, out ctx, sid, ref cch);
                }

                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    patch = new PatchInfo(pac.ToString(), prc.ToString(), sid.ToString(), ctx);
                }
            }
            else
            {
                // Use MsiEnumPatches for releases prior to MSI 3.0
                StringBuilder msts = new StringBuilder(80);
                cch = msts.Capacity;

                this.CallingNativeFunction("MsiEnumPatches", this.currentProductCode, index);
                ret = NativeMethods.MsiEnumPatches(this.currentProductCode, index, pac, msts, ref cch);

                if (NativeMethods.ERROR_MORE_DATA == ret)
                {
                    pac.Length = 0;
                    msts.Capacity = ++cch;

                    ret = NativeMethods.MsiEnumPatches(this.currentProductCode, index, pac, msts, ref cch);
                }

                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    patch = new PatchInfo(pac.ToString(), null, null, InstallContext.None);
                }
            }

            return ret;
        }

        protected override ErrorDetails GetErrorDetails(int returnCode)
        {
            switch (returnCode)
            {
                case NativeMethods.ERROR_BAD_CONFIGURATION:
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Error_BadProductConfiguration, this.currentProductCode);
                        ErrorDetails err = new ErrorDetails(message);
                        err.RecommendedAction = Properties.Resources.Recommend_Recache;
                        return err;
                    }
            }
            
            return base.GetErrorDetails(returnCode);
        }

        protected override void AddMembers(PSObject psobj)
        {
            // Add PSPath with fully-qualified provider path.
            PatchInfo obj = (PatchInfo)psobj.BaseObject;
            Location.AddPSPath(obj.LocalPackage, psobj, this);
        }
    }
}
