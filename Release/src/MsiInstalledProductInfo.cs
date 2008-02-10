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
		string installedProductName;

		public string VersionString
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_VERSIONSTRING, ref versionString);
			}
		}
		string versionString;

		public string HelpLink
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_HELPLINK, ref helpLink);
			}
		}
		string helpLink;

		public string HelpTelephone
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_HELPTELEPHONE, ref helpTelephone);
			}
		}
		string helpTelephone;

		public string InstallLocation
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_INSTALLLOCATION, ref installLocation);
			}
		}
		string installLocation;

		public string InstallSource
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_INSTALLSOURCE, ref installSource);
			}
		}
		string installSource;

		public DateTime InstallDate
		{
			get
			{
				return (DateTime)GetProperty<DateTime>(Msi.INSTALLPROPERTY_INSTALLDATE, ref installDate);
			}
		}
		string installDate;

		public string Publisher
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PUBLISHER, ref publisher);
			}
		}
		string publisher;

		public string LocalPackage
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_LOCALPACKAGE, ref localPackage);
			}
		}
		string localPackage;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string UrlInfoAbout
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_URLINFOABOUT, ref urlInfoAbout);
			}
		}
		string urlInfoAbout;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string UrlUpdateInfo
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_URLUPDATEINFO, ref urlUpdateInfo);
			}
		}
		string urlUpdateInfo;

		public int VersionMinor
		{
			get
			{
				return (int)GetProperty<int>(Msi.INSTALLPROPERTY_VERSIONMINOR, ref versionMinor);
			}
		}
		string versionMinor;

		public int VersionMajor
		{
			get
			{
				return (int)GetProperty<int>(Msi.INSTALLPROPERTY_VERSIONMAJOR, ref versionMajor);
			}
		}
		string versionMajor;

		public string ProductId
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PRODUCTID, ref productId);
			}
		}
		string productId;

		public string RegCompany
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_REGCOMPANY, ref regCompany);
			}
		}
		string regCompany;

		public string RegOwner
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_REGOWNER, ref regOwner);
			}
		}
		string regOwner;

		internal override string PSPath
		{
			get
			{
				return LocalPackage;
			}
		}
	}
}

