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
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;
using System.Globalization;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
	[Cmdlet(VerbsCommon.Get, "MSISource",
        DefaultParameterSetName = GetSourceCommand.ProductOrPatchCodeParameterSet)]
	public sealed class GetSourceCommand : EnumCommand<PackageSource>
	{
		internal const string ProductOrPatchCodeParameterSet = "ProductOrPatchCode";
		const string EVERYONE = "s-1-1-0";

		protected override void ProcessRecord()
		{
			if (ParameterSetName == GetProductCommand.ProductCodeParameterSet)
			{
                WriteCommandDetail("Enumerating source list for input product codes.");
				foreach (string productCode in this.productCodes)
				{
					this.code = Code.Product;
					this.productOrPatchCode = productCode;

					base.ProcessRecord();
				}
			}
			else if (ParameterSetName == GetPatchCommand.PatchCodeParameterSet)
			{
                WriteCommandDetail("Enumerating source list for input patch codes.");
				foreach (string patchCode in this.patchCodes)
				{
					this.code = Code.Patch;
					this.productOrPatchCode = patchCode;

					base.ProcessRecord();
				}
			}

			else if (ParameterSetName == ProductOrPatchCodeParameterSet)
			{
                WriteCommandDetail("Enumerating source list for input objects.");
				foreach (PSObject obj in this.inputObjects)
				{
					if (obj.BaseObject is ProductInfo)
					{
						ProductInfo info = (ProductInfo)obj.BaseObject;

						this.code = Code.Product;
						this.productOrPatchCode = info.ProductCode;
						this.userSid = info.UserSid;
						this.context = info.InstallContext;
					}
                    else if (obj.BaseObject is PatchInfo)
                    {
                        PatchInfo info = (PatchInfo)obj.BaseObject;

                        this.code = Code.Patch;
                        this.productOrPatchCode = info.PatchCode;
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

		string productOrPatchCode;
		string userSid;
		InstallContext context = InstallContext.Machine;
		Code code = Code.Product;
		SourceTypes sourceType = SourceTypes.Network;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
                Mandatory = true,
                HelpMessageBaseName = "Microsoft.Windows.Installer.PowerShell.Properties.Resources",
                HelpMessageResourceId = "GetSource_InputObject",
                ParameterSetName = ProductOrPatchCodeParameterSet,
                Position = 0,
                ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public PSObject[] InputObject
        {
            get { return this.inputObjects; }
            set { this.inputObjects = value; }
        }
        PSObject[] inputObjects;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
                Mandatory = true,
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetSource_ProductCode",
				ParameterSetName=GetProductCommand.ProductCodeParameterSet,
				Position=0,
				ValueFromPipelineByPropertyName=true)]
		[ValidateNotNullOrEmpty]
		public string[] ProductCode
		{
			get { return this.productCodes; }
			set { this.productCodes = value; }
		}
		string[] productCodes;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
                Mandatory = true,
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetSource_PatchCode",
				ParameterSetName=GetPatchCommand.PatchCodeParameterSet,
				Position=0,
				ValueFromPipelineByPropertyName=true)]
		[ValidateNotNullOrEmpty]
		public string[] PatchCode
		{
			get { return this.patchCodes; }
			set { this.patchCodes = value; }
		}
		string[] patchCodes;

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetSource_UserSid",
				ValueFromPipelineByPropertyName=true)]
		public string UserSid
		{
			get { return this.userSid; }
			set { this.userSid = value; }
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetSource_InstallContext",
				ValueFromPipelineByPropertyName=true)]
		public InstallContext InstallContext
		{
			get { return this.context; }
			set { this.context = value; }
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetSource_SourceType",
				ValueFromPipelineByPropertyName=true)]
		public SourceTypes SourceType
		{
			get { return this.sourceType; }
			set
			{
				if ((value & SourceTypes.Media) == SourceTypes.Media)
				{
					throw new ArgumentException(Properties.Resources.Argument_InvalidSourceType);
				}

				this.sourceType = value;
			}
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetSource_Everyone")]
		public SwitchParameter Everyone
		{
			get { return string.Compare(this.userSid, EVERYONE, StringComparison.OrdinalIgnoreCase) == 0; }
			set
			{
				if (value)
				{
					this.userSid = EVERYONE;
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
			StringBuilder sb = new StringBuilder(Msi.MAX_PATH);
			int cch = sb.Capacity;

			source = null;
			if (Msi.CheckVersion(3, 0, true))
			{
				ret = Msi.MsiSourceListEnumSources(this.productOrPatchCode, this.userSid, this.context,
						(int)this.code | (int)this.sourceType, index, sb, ref cch);
				Debug(
					"Returned {7}: MsiSourceListEnumSources('{0}', '{1}', 0x{2:x8}, 0x{3:x8}, {4}, '{5}', {6})",
					this.productOrPatchCode, this.userSid, (int)this.context, (int)this.code | (int)this.sourceType,
					index, sb, cch, ret);

				if (Msi.ERROR_MORE_DATA == ret)
				{
					sb.Capacity = ++cch;

					ret = Msi.MsiSourceListEnumSources(this.productOrPatchCode, this.userSid, this.context,
						(int)this.code | (int)this.sourceType, index, sb, ref cch);
					Debug(
						"Returned {7}: MsiSourceListEnumSources('{0}', '{1}', 0x{2:x8}, 0x{3:x8}, {4}, '{5}', {6})",
						this.productOrPatchCode, this.userSid, (int)this.context, (int)this.code | (int)this.sourceType,
						index, sb, cch, ret);
				}

				if (Msi.ERROR_SUCCESS == ret)
				{
                    source = new PackageSource(this.sourceType, index, sb.ToString());
				}
			}

			return ret;
		}

		protected override void WritePSObject(PackageSource obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");
			
			// Add PSPath with fully-qualified provider path.
			PSObject psobj = PSObject.AsPSObject(obj);
			Location.AddPSPath(obj.Path, psobj, this);

			WriteObject(psobj);
		}
	}
}
