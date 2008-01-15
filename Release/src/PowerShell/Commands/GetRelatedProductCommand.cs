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
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
	[Cmdlet(VerbsCommon.Get, "MSIRelatedProductInfo",
        DefaultParameterSetName = GetRelatedProductCommand.UpgradeCodeParameterSet)]
	public sealed class GetRelatedProductCommand : EnumCommand<ProductInfo>
	{
		const string UpgradeCodeParameterSet = "UpgradeCode";
		string upgradeCode = null;

		protected override void ProcessRecord()
		{
            WriteCommandDetail("Enumerating product instances for each upgrade code.");
			foreach (string upgradeCode in this.upgradeCodes)
			{
				this.upgradeCode = upgradeCode;
				base.ProcessRecord();
			}
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetRelatedProduct_UpgradeCode",
				ParameterSetName=UpgradeCodeParameterSet,
				Position=0,
				ValueFromPipeline=true,
				ValueFromPipelineByPropertyName=true)]
		[ValidateNotNullOrEmpty]
		public string[] UpgradeCode
		{
			get { return upgradeCodes; }
			set { upgradeCodes = value; }
		}
		string[] upgradeCodes = null;

		protected override int Enumerate(int index, out ProductInfo product)
		{
			int ret = 0;
			StringBuilder pc = new StringBuilder(Msi.GUID_CHARS + 1);

			product = null;
			ret = Msi.MsiEnumRelatedProducts(this.upgradeCode, 0, index, pc);
			Debug(
				"Returned {3}: MsiEnumRelatedProducts('{0}', 0, {1}, '{2}')",
				this.upgradeCode, index, pc, ret);

			if (Msi.ERROR_SUCCESS == ret)
			{
				product = ProductInfo.Create(pc.ToString());
			}

			return ret;
		}

		protected override void WritePSObject(ProductInfo obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			// Add PSPath with fully-qualified provider path.
			PSObject psobj = PSObject.AsPSObject(obj);
			Location.AddPSPath(obj.PSPath, psobj, this);

			WriteObject(psobj);
		}
	}
}
