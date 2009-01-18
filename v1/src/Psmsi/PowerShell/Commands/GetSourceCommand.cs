// Enumerates the source list for a product or patch.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Tue, 20 Mar 2007 06:41:00 GMT
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
    [Cmdlet(VerbsCommon.Get, "MSISource",
        DefaultParameterSetName = ParameterSet.ProductOrPatchCode)]
    public sealed class GetSourceCommand : EnumCommand<PackageSource>
    {
        string currentProductOrPatchCode;
        Code code = Code.Product;

        protected override void ProcessRecord()
        {
            if (ParameterSet.ProductCode == this.ParameterSetName)
            {
                foreach (string productCode in this.productCodes)
                {
                    this.code = Code.Product;
                    this.currentProductOrPatchCode = productCode;

                    base.ProcessRecord();
                }
            }
            else if (ParameterSet.PatchCode == this.ParameterSetName)
            {
                foreach (string patchCode in this.patchCodes)
                {
                    this.code = Code.Patch;
                    this.currentProductOrPatchCode = patchCode;

                    base.ProcessRecord();
                }
            }
            else if (ParameterSet.ProductOrPatchCode == this.ParameterSetName)
            {
                foreach (PSObject obj in this.inputObjects)
                {
                    if (obj.BaseObject is ProductInfo)
                    {
                        ProductInfo info = (ProductInfo)obj.BaseObject;

                        this.code = Code.Product;
                        this.currentProductOrPatchCode = info.ProductCode;
                        this.userSid = info.UserSid;
                        this.context = info.InstallContext;
                    }
                    else if (obj.BaseObject is PatchInfo)
                    {
                        PatchInfo info = (PatchInfo)obj.BaseObject;

                        this.code = Code.Patch;
                        this.currentProductOrPatchCode = info.PatchCode;
                        this.userSid = info.UserSid;
                        this.context = info.InstallContext;
                    }
                    else
                    {
                        WriteVerbose("Skipping invalid input object.");
                    }

                    base.ProcessRecord();
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                Mandatory = true,
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetSource_InputObject",
                ParameterSetName = ParameterSet.ProductOrPatchCode,
                Position = 0,
                ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public PSObject[] InputObject
        {
            get { return this.inputObjects; }
            set { this.inputObjects = value; }
        }
        PSObject[] inputObjects;

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                Mandatory = true,
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetSource_ProductCode",
                ParameterSetName = ParameterSet.ProductCode,
                Position = 0,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] ProductCode
        {
            get { return this.productCodes; }
            set { this.productCodes = value; }
        }
        string[] productCodes;

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                Mandatory = true,
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetSource_PatchCode",
                ParameterSetName = ParameterSet.PatchCode,
                Position = 0,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] PatchCode
        {
            get { return this.patchCodes; }
            set { this.patchCodes = value; }
        }
        string[] patchCodes;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Context_UserSid",
                ValueFromPipelineByPropertyName = true)]
        public string UserSid
        {
            get { return this.userSid; }
            set { this.userSid = value; }
        }
        string userSid;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Context_InstallContext",
                ValueFromPipelineByPropertyName = true)]
        [Alias("Context")]
        public InstallContext InstallContext
        {
            get { return this.context; }
            set
            {
                if (value == InstallContext.None)
                {
                    throw new PSInvalidParameterException("InstallContext", value);
                }

                context = value;
            }
        }
        InstallContext context = InstallContext.Machine;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetSource_SourceType",
                ValueFromPipelineByPropertyName = true)]
        public SourceTypes SourceType
        {
            get { return this.sourceType; }
            set
            {
                if (value == SourceTypes.None || (value & SourceTypes.Media) == SourceTypes.Media)
                {
                    throw new PSInvalidParameterException("SourceType", value);
                }

                this.sourceType = value;
            }
        }
        SourceTypes sourceType = SourceTypes.Network;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "Context_Everyone")]
        public SwitchParameter Everyone
        {
            get { return string.Compare(this.userSid, NativeMethods.World, StringComparison.OrdinalIgnoreCase) == 0; }
            set
            {
                if (value)
                {
                    this.userSid = NativeMethods.World;
                }
                else
                {
                    this.userSid = null;
                }
            }
        }

        protected override int Enumerate(int index, out PackageSource source)
        {
            int ret = 0;
            StringBuilder sb = new StringBuilder(NativeMethods.MAX_PATH);
            int cch = sb.Capacity;

            source = null;
            if (Msi.CheckVersion(3, 0, true))
            {
                int options = (int)this.code | (int)this.sourceType;

                this.CallingNativeFunction("MsiSourceListEnumSources", this.currentProductOrPatchCode, this.userSid, (int)this.context, options, index);
                ret = NativeMethods.MsiSourceListEnumSources(this.currentProductOrPatchCode, this.userSid, this.context, options, index, sb, ref cch);

                if (NativeMethods.ERROR_MORE_DATA == ret)
                {
                    sb.Capacity = ++cch;

                    ret = NativeMethods.MsiSourceListEnumSources(this.currentProductOrPatchCode, this.userSid, this.context, options, index, sb, ref cch);
                }

                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    source = new PackageSource(this.sourceType, index, sb.ToString());
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns <see cref="ErrorDetails"/> with patch or product information depending on the current
        /// object in the pipeline.
        /// </summary>
        /// <param name="returnCode">The return code for which <see cref="ErrorDetails"/> should be retrieved.</param>
        /// <returns>If the <paramref name="returnCode"/> is handled, an <see cref="ErrorDetails"/> object
        /// with additional information; otherwise, null.</returns>
        protected override ErrorDetails GetErrorDetails(int returnCode)
        {
            if (Code.Patch == this.code)
            {
                switch (returnCode)
                {
                    case NativeMethods.ERROR_BAD_CONFIGURATION:
                        {
                            string message = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Error_BadPatchConfiguration, this.currentProductOrPatchCode);
                            ErrorDetails err = new ErrorDetails(message);
                            return err;
                        }
                }
            }
            else if (Code.Product == this.code)
            {
                switch (returnCode)
                {
                    case NativeMethods.ERROR_BAD_CONFIGURATION:
                        {
                            string message = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Error_BadProductConfiguration, this.currentProductOrPatchCode);
                            ErrorDetails err = new ErrorDetails(message);
                            err.RecommendedAction = Properties.Resources.Recommend_Recache;
                            return err;
                        }
                }
            }

            return base.GetErrorDetails(returnCode);
        }

        protected override void AddMembers(PSObject psobj)
        {
            base.AddMembers(psobj);

            // Add PSPath with fully-qualified provider path.
            PackageSource obj = (PackageSource)psobj.BaseObject;
            Location.AddPSPath(obj.Path, psobj, this);

            // Add current fields to properties for parameter binding.
            if (Code.Patch == this.code)
            {
                psobj.Properties.Add(new PSNoteProperty("PatchCode", this.currentProductOrPatchCode));
            }
            else
            {
                psobj.Properties.Add(new PSNoteProperty("ProductCode", this.currentProductOrPatchCode));
            }
            psobj.Properties.Add(new PSNoteProperty("UserSid", this.userSid));
            psobj.Properties.Add(new PSNoteProperty("InstallContext", this.context));
        }
    }
}
