// Installs Cmdlets into PowerShell.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 08:14:04 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;
using System.Management.Automation;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer.PowerShell
{
	[RunInstaller(true)]
	public class WindowsInstallerSnapIn : PSSnapIn
	{
		public override string Name { get { return "psmsi"; } }
		public override string Vendor { get { return Properties.Resources.SnapIn_Vendor; } }
		public override string VendorResource
		{
			get
			{
				return "Microsoft.Windows.Installer.Properties.Resources,SnapIn_Vendor";
			}
		}
		public override string Description { get { return Properties.Resources.SnapIn_Description; } }
		public override string DescriptionResource
		{
			get
			{
                return "Microsoft.Windows.Installer.Properties.Resources,SnapIn_Description";
			}
		}
	}
}
