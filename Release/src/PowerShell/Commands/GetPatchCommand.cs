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
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;
using System.Globalization;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
	[Cmdlet(VerbsCommon.Get, "MSIPatchInfo",
        DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public sealed class GetPatchCommand : EnumCommand<PatchInfo>
	{
		const string EVERYONE = "s-1-1-0";
		internal const string PatchCodeParameterSet = "PatchCode";
		const string ProductInfoParameterSet = "ProductInfo";

		protected override void ProcessRecord()
		{
			// Use input Product objects to enumerate patches for each Product.
			if (ParameterSetName == ProductInfoParameterSet)
			{
				WriteCommandDetail("Enumerating patches for input products.");
				foreach (ProductInfo product in this.inputObjects)
				{
					ProcessProduct(product);
				}
			}
			// Use input patch codes to get properties for each specific patch codes.
			else if (ParameterSetName == PatchCodeParameterSet)
			{
                WriteCommandDetail("Enumerating patches for input patch codes.");
				foreach (string patchCode in this.patchCodes)
				{
					// Write out patches for each PatchCode for a given product in a given context.
					WritePSObject(new PatchInfo(patchCode, productCode, userSid, context));
				}
			}
			// Enumerate patches for any other parameters besides InputObject and PatchCode.
			else
			{
				if ((context & InstallContext.Machine) != 0)
                    WriteCommandDetail("Enumerating patches for machine assigned products.");

				if ((context & InstallContext.UserManaged) != 0)
                    WriteCommandDetail(string.Format(CultureInfo.InvariantCulture, "Enumerating paches for user-managed products for '{0}'.", userSid));

				if ((context & InstallContext.UserUnmanaged) != 0)
                    WriteCommandDetail(string.Format(CultureInfo.InvariantCulture, "Enumerating patches for user-unmanaged products for '{0}'.", userSid));
				
				if ((filter & PatchStates.Applied) != 0)
                    WriteCommandDetail("Enumerating applied patches.");

				if ((filter & PatchStates.Superseded) != 0)
                    WriteCommandDetail("Enumerating superseded patches.");

				if ((filter & PatchStates.Obsoleted) != 0)
                    WriteCommandDetail("Enumerating obsoleted patches.");

				if ((filter & PatchStates.Registered) != 0)
                    WriteCommandDetail("Enumerating registered patches.");

				// Enumerate all products on the system.
				base.ProcessRecord();
			}
		}

		void ProcessProduct(ProductInfo product)
		{
			// For the product, set the other parameters for this cmdlet and process.
			this.productCode = product.ProductCode;
			this.userSid = product.UserSid;
			this.context = product.InstallContext;

			base.ProcessRecord();
		}

		string productCode;
		string userSid;
		InstallContext context = InstallContext.Machine;
		PatchStates filter = PatchStates.Applied;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_InputObject",
				ParameterSetName=ProductInfoParameterSet,
				Position=0,
				ValueFromPipeline=true)]
		[ValidateNotNullOrEmpty]
		public ProductInfo[] InputObject
		{
			get { return inputObjects; }
			set { inputObjects = value; }
		}
		ProductInfo[] inputObjects;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_PackageCode",
				ParameterSetName=PatchCodeParameterSet,
				Position=0,
				ValueFromPipeline=true,
				ValueFromPipelineByPropertyName=true)]
		[ValidateNotNullOrEmpty]
		public string[] PatchCode
		{
			get { return patchCodes; }
			set { patchCodes = value; }
		}
		string[] patchCodes;

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_ProductCode",
				ValueFromPipelineByPropertyName=true)]
		[ValidateNotNullOrEmpty]
		public string ProductCode
		{
			get { return productCode; }
			set { productCode = value; }
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_UserSid",
				ValueFromPipelineByPropertyName=true)]
		public string UserSid
		{
			get { return userSid; }
			set { userSid = value; }
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_InstallContext",
				ValueFromPipelineByPropertyName=true)]
		public InstallContext InstallContext
		{
			get { return context; }
			set { context = value; }
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_Filter",
				ValueFromPipelineByPropertyName=true)]
		public PatchStates Filter
		{
			get { return filter; }
			set { filter = value; }
		}

		[Parameter(
				HelpMessageBaseName="Microsoft.Windows.Installer.PowerShell.Properties.Resources",
				HelpMessageResourceId="GetPatch_Everyone")]
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

				ret = NativeMethods.MsiEnumPatchesEx(productCode, userSid, context,
						filter, index, pac, prc, out ctx, sid, ref cch);
				Debug(
					"Returned {10}: MsiEnumPatchesEx('{0}', '{1}', 0x{2:x8}, 0x{3:x8}, {4}, '{5}', '{6}', 0x{7:x8}, '{8}', {9})",
					productCode, userSid, (int)context, (int)filter, index, pac, prc, (int)ctx, sid, cch, ret);

				if (NativeMethods.ERROR_MORE_DATA == ret)
				{
					pac.Length = 0;
					prc.Length = 0;
					sid.Capacity = ++cch;

					ret = NativeMethods.MsiEnumPatchesEx(productCode, userSid, context,
									filter, index, pac, prc, out ctx, sid, ref cch);
					Debug(
						"Returned {10}: MsiEnumPatchesEx('{0}', '{1}', 0x{2:x8}, 0x{3:x8}, {4}, '{5}', '{6}', 0x{7:x8}, '{8}', {9})",
						productCode, userSid, (int)context, (int)filter, index, pac, prc, (int)ctx, sid, cch, ret);
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

				ret = NativeMethods.MsiEnumPatches(productCode, index, pac, msts, ref cch);
				Debug("Returned {5}: MsiEnumPatches('{0}', {1}, '{2}', '{3}', {4})",
					productCode, index, pac, msts, cch, ret);

				if (NativeMethods.ERROR_MORE_DATA == ret)
				{
					pac.Length = 0;
					msts.Capacity = ++cch;

					ret = NativeMethods.MsiEnumPatches(productCode, index, pac, msts, ref cch);
					Debug("Returned {5}: MsiEnumPatches('{0}', {1}, '{2}', '{3}', {4})",
						productCode, index, pac, msts, cch, ret);
				}

				if (NativeMethods.ERROR_SUCCESS == ret)
				{
					patch = new PatchInfo(pac.ToString(), null, null, InstallContext.None);
				}
			}

			return ret;
		}

		protected override void WritePSObject(PatchInfo obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			// Add PSPath with fully-qualified provider path.
			PSObject psobj = PSObject.AsPSObject(obj);
			Location.AddPSPath(obj.LocalPackage, psobj, this);

			WriteObject(psobj);
		}
	}
}
