// Cmdlet to get or enumerator Windows Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 06:54:27 GMT
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
    [Cmdlet(VerbsCommon.Get, "MSIProductInfo",
        DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
    public sealed class GetProductCommand : EnumCommand<ProductInfo>
    {
        string currentProductCode;

        protected override void ProcessRecord()
        {
            // Enumerate all product instances with the given parameters.
            if (ParameterSet.ProductCode == this.ParameterSetName)
            {
                foreach (string productCode in this.productCodes)
                {
                    this.currentProductCode = productCode;

                    // Enumerate all products on the system.
                    base.ProcessRecord();
                }
            }
            else
            {
                // Enumerate all products with the given parameters.
                base.ProcessRecord();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetProduct_ProductCode",
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

        protected override int Enumerate(int index, out ProductInfo product)
        {
            int ret = 0;
            StringBuilder pc = new StringBuilder(NativeMethods.MAX_GUID_CHARS + 1);
            InstallContext ctx = InstallContext.None;
            int cch = 0;

            product = null;
            if (Msi.CheckVersion(3, 0))
            {
                StringBuilder sid = new StringBuilder(80);
                cch = sid.Capacity;

                this.CallingNativeFunction("MsiEnumProductsEx", this.currentProductCode, this.userSid, (int)this.context, index);
                ret = NativeMethods.MsiEnumProductsEx(this.currentProductCode, this.userSid, this.context, index, pc, out ctx, sid, ref cch);

                if (NativeMethods.ERROR_MORE_DATA == ret)
                {
                    pc.Length = 0;
                    sid.Capacity = ++cch;
                    sid.Length = 0; // Null terminate in case of junk data

                    ret = NativeMethods.MsiEnumProductsEx(this.currentProductCode, this.userSid, this.context, index, pc, out ctx, sid, ref cch);
                }

                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    product = ProductInfo.Create(pc.ToString(), sid.ToString(), ctx);
                }
            }
            else
            {
                this.CallingNativeFunction("MsiEnumProducts", index);
                ret = NativeMethods.MsiEnumProducts(index, pc);
                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    product = ProductInfo.Create(pc.ToString());
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
            ProductInfo obj = (ProductInfo)psobj.BaseObject;
            if (null != obj.PSPath)
            {
                Location.AddPSPath(obj.PSPath, psobj, this);
            }
        }
    }
}
