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
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;
using System.Globalization;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "MSIProductInfo",
        DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
    public sealed class GetProductCommand : EnumCommand<ProductInfo>
    {
        const string EVERYONE = "s-1-1-0";
        internal const string ProductCodeParameterSet = "ProductCode";
        const string ProductInfoParameterSet = "ProductInfo";

        protected override void ProcessRecord()
        {
            // Create Product objects for each given ProductCode.
            if (ParameterSetName == ProductCodeParameterSet)
            {
                WriteCommandDetail("Creating products for each input ProductCode.");
                foreach (string _productCode in this.productCodes)
                {
                    WritePSObject(ProductInfo.Create(_productCode, userSid, context));
                }
            }
            // Enumerate instances of each given Product based on each Product's ProductCode.
            else if (ParameterSetName == ProductInfoParameterSet)
            {
                WriteCommandDetail("Enumerating product instances for input products.");
                foreach (ProductInfo product in this.inputObjects)
                {
                    ProcessProduct(product);
                }
            }
            // Enumerate all products with the given parameters.
            else
            {
                if ((context & InstallContext.Machine) != 0)
                    WriteCommandDetail("Enumerating machine assigned products.");

                if ((context & InstallContext.UserManaged) != 0)
                    WriteCommandDetail(string.Format(CultureInfo.InvariantCulture, "Enumerating user-managed products for '{0}'.", userSid));

                if ((context & InstallContext.UserUnmanaged) != 0)
                    WriteCommandDetail(string.Format(CultureInfo.InvariantCulture, "Enumerating user-unmanaged products for '{0}'.", userSid));

                // Enumerate all products on the system.
                base.ProcessRecord();
            }
        }

        void ProcessProduct(ProductInfo product)
        {
            // Set parameters using input product.
            this.productCode = product.ProductCode;
            this.userSid = product.UserSid;
            this.context = product.InstallContext;

            base.ProcessRecord();
        }

        string productCode;
        string userSid;
        InstallContext context = InstallContext.Machine;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "GetProduct_InputObject",
                ParameterSetName = ProductInfoParameterSet,
                Position = 0,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public ProductInfo[] InputObject
        {
            get { return inputObjects; }
            set { inputObjects = value; }
        }
        ProductInfo[] inputObjects;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "GetProduct_ProductCode",
                ParameterSetName = ProductCodeParameterSet,
                Position = 0,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] ProductCode
        {
            get { return productCodes; }
            set { productCodes = value; }
        }
        string[] productCodes;

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "GetProduct_UserSid",
                ValueFromPipelineByPropertyName = true)]
        public string UserSid
        {
            get { return userSid; }
            set { userSid = value; }
        }

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "GetProduct_InstallContext",
                ValueFromPipelineByPropertyName = true)]
        public InstallContext InstallContext
        {
            get { return context; }
            set { context = value; }
        }

        [Parameter(
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "GetProduct_Everyone")]
        public SwitchParameter Everyone
        {
            get { return string.Compare(userSid, EVERYONE, StringComparison.OrdinalIgnoreCase) == 0; }
            set
            {
                if (value)
                {
                    userSid = EVERYONE;
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

                ret = NativeMethods.MsiEnumProductsEx(productCode, userSid, context, index, pc, out ctx, sid, ref cch);
                Debug(
                    "Returned {8}: MsiEnumProductsEx('{0}', '{1}', 0x{2:x8}, {3}, '{4}', 0x{5:x8}, '{6}', {7})",
                    productCode, userSid, (int)context, index, pc, (int)ctx, sid, cch, ret);

                if (NativeMethods.ERROR_MORE_DATA == ret)
                {
                    pc.Length = 0;
                    sid.Capacity = ++cch;
                    sid.Length = 0; // Null terminate in case of junk data

                    ret = NativeMethods.MsiEnumProductsEx(productCode, userSid, context, index, pc, out ctx, sid, ref cch);
                    Debug(
                        "Returned {8}: MsiEnumProductsEx('{0}', '{1}', 0x{2:x8}, {3}, '{4}', 0x{5:x8}, '{6}', {7})",
                        productCode, userSid, (int)context, index, pc, (int)ctx, sid, cch, ret);
                }

                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    product = ProductInfo.Create(pc.ToString(), sid.ToString(), ctx);
                }
            }
            else
            {
                ret = NativeMethods.MsiEnumProducts(index, pc);
                Debug("Returned {2}: MsiEnumProducts({0}, '{1}')", index, pc, ret);

                if (NativeMethods.ERROR_SUCCESS == ret)
                {
                    product = ProductInfo.Create(pc.ToString());
                }
            }

            return ret;
        }

        protected override void WritePSObject(ProductInfo obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            PSObject psobj = PSObject.AsPSObject(obj);

            // Add PSPath with fully-qualified provider path.
            if (null != obj.PSPath)
            {
                Location.AddPSPath(obj.PSPath, psobj, this);
            }

            WriteObject(psobj);
        }
    }
}
