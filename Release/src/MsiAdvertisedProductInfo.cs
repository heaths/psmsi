// Advertised product that contains advertised-only functionality.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Sat, 10 Feb 2007 09:10:56 GMT
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
using System.IO;
using System.Resources;
using System.Text;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer
{
	public class AdvertisedProductInfo : ProductInfo
	{
		internal AdvertisedProductInfo(string productCode) :
			this(productCode, null, InstallContext.Machine)
		{
		}

		internal AdvertisedProductInfo(string productCode, string userSid, InstallContext context) :
			base(productCode, userSid, context)
		{
		}

		public override ProductState ProductState
		{
			get { return ProductState.Advertised; }
		}

		internal override string PSPath
		{
			get
			{
				PackageSource source = new PackageSource(LastUsedSource);
				return Path.Combine(source.Path, PackageName);
			}
		}
	}
}

