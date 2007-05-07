// Advertised product that contains advertised-only functionality.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Sat, 10 Feb 2007 09:12:51 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Text;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer
{
	public class InstalledProductInfo : ProductInfo
	{
		internal InstalledProductInfo(string productCode) :
			this(productCode, null, InstallContext.Machine)
		{
		}

		internal InstalledProductInfo(string productCode, string userSid, InstallContext context) :
			base(productCode, userSid, context)
		{
		}

		public override ProductState ProductState
		{
			get { return ProductState.Installed; }
		}

		public string InstalledProductName
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_INSTALLEDPRODUCTNAME,
						ref installedProductName);
			}
		}
		string installedProductName = null;

		public string VersionString
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_VERSIONSTRING, ref versionString);
			}
		}
		string versionString = null;

		public string HelpLink
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_HELPLINK, ref helpLink);
			}
		}
		string helpLink = null;

		public string HelpTelephone
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_HELPTELEPHONE, ref helpTelephone);
			}
		}
		string helpTelephone = null;

		public string InstallLocation
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_INSTALLLOCATION, ref installLocation);
			}
		}
		string installLocation = null;

		public string InstallSource
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_INSTALLSOURCE, ref installSource);
			}
		}
		string installSource = null;

		public DateTime InstallDate
		{
			get
			{
				return (DateTime)GetProperty<DateTime>(Msi.INSTALLPROPERTY_INSTALLDATE, ref installDate);
			}
		}
		string installDate = null;

		public string Publisher
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PUBLISHER, ref publisher);
			}
		}
		string publisher = null;

		public string LocalPackage
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_LOCALPACKAGE, ref localPackage);
			}
		}
		string localPackage = null;

		public string UrlInfoAbout
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_URLINFOABOUT, ref urlInfoAbout);
			}
		}
		string urlInfoAbout = null;

		public string UrlUpdateInfo
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_URLUPDATEINFO, ref urlUpdateInfo);
			}
		}
		string urlUpdateInfo = null;

		public int VersionMinor
		{
			get
			{
				return (int)GetProperty<int>(Msi.INSTALLPROPERTY_VERSIONMINOR, ref versionMinor);
			}
		}
		string versionMinor = null;

		public int VersionMajor
		{
			get
			{
				return (int)GetProperty<int>(Msi.INSTALLPROPERTY_VERSIONMAJOR, ref versionMajor);
			}
		}
		string versionMajor = null;

		public string ProductID
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PRODUCTID, ref productID);
			}
		}
		string productID = null;

		public string RegCompany
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_REGCOMPANY, ref regCompany);
			}
		}
		string regCompany = null;

		public string RegOwner
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_REGOWNER, ref regOwner);
			}
		}
		string regOwner = null;

		internal override string PSPath
		{
			get
			{
				return LocalPackage;
			}
		}
	}
}

