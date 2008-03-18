// Cmdlet to get or enumerator Windows Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Fri, 06 Apr 2007 14:59:12 GMT
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
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "MSIRelatedProductInfo",
        DefaultParameterSetName = ParameterSet.UpgradeCode)]
    public sealed class GetRelatedProductCommand : EnumCommand<ProductInfo>
    {
        string currentUpgradeCode;

        protected override void ProcessRecord()
        {
            if (ParameterSet.UpgradeCode == this.ParameterSetName)
            {
                foreach (string upgradeCode in this.upgradeCodes)
                {
                    this.currentUpgradeCode = upgradeCode;
                    base.ProcessRecord();
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(
                Mandatory = true,
                HelpMessageBaseName = "Microsoft.Windows.Installer.Properties.Resources",
                HelpMessageResourceId = "GetRelatedProduct_UpgradeCode",
                ParameterSetName = ParameterSet.UpgradeCode,
                Position = 0,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] UpgradeCode
        {
            get { return upgradeCodes; }
            set { upgradeCodes = value; }
        }
        string[] upgradeCodes;

        protected override int Enumerate(int index, out ProductInfo product)
        {
            int ret = 0;
            product = null;
            StringBuilder pc = new StringBuilder(NativeMethods.MAX_GUID_CHARS + 1);

            this.CallingNativeFunction("MsiEnumRelatedProducts", this.currentUpgradeCode, index);
            ret = NativeMethods.MsiEnumRelatedProducts(this.currentUpgradeCode, 0, index, pc);

            if (NativeMethods.ERROR_SUCCESS == ret)
            {
                product = ProductInfo.Create(pc.ToString());
            }

            return ret;
        }

        protected override void AddMembers(PSObject psobj)
        {
            // Add PSPath with fully-qualified provider path.
            ProductInfo obj = (ProductInfo)psobj.BaseObject;
            Location.AddPSPath(obj.PSPath, psobj, this);
        }
    }
}
