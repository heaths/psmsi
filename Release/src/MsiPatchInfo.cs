// Represents installed patches.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 18:39:11 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Text;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;
using Microsoft.Windows.Installer.PowerShell.Commands;

namespace Microsoft.Windows.Installer
{
	public class PatchInfo
	{
		string patchCode, productCode, userSid, transforms = null;
		InstallContext context;

		internal PatchInfo(string patchCode, string transforms) :
			this(patchCode, null, null, InstallContext.None)
		{
			this.transforms = transforms;
		}

		internal PatchInfo(string patchCode, string productCode, string userSid, InstallContext context)
		{
			// Must at least have a PatchCode.
			if (string.IsNullOrEmpty(patchCode))
			{
				throw new ArgumentNullException("productCode");
			}

			// Validate InstallContext and UserSid combinations.
			if (((InstallContext.UserManaged | InstallContext.UserUnmanaged) & context) != 0
				&& string.IsNullOrEmpty(userSid))
			{
				throw new ArgumentException(Resources.Argument_InvalidContextAndSid);
			}

			this.patchCode = patchCode;
			this.productCode = string.IsNullOrEmpty(productCode) ? null : productCode;
			this.userSid = string.IsNullOrEmpty(userSid) ||
				context == InstallContext.Machine ? null : userSid;
			this.context = context;
		}

		public string PatchCode { get { return patchCode; } }
		public string ProductCode { get { return productCode; } }
		public string UserSid { get { return userSid; } }
		public InstallContext InstallContext { get { return context; } }

		public bool Uninstallable
		{
			get
			{
				return (bool)GetProperty<bool>(Msi.INSTALLPROPERTY_UNINSTALLABLE, ref uninstallable);
			}
		}
		string uninstallable = null;

		public PatchState PatchState
		{
			get
			{
				return (PatchState)GetProperty<PatchState>(Msi.INSTALLPROPERTY_PATCHSTATE, ref patchState);
			}
		}
		string patchState = null;

		public bool LuaEnabled
		{
			get
			{
				return (bool)GetProperty<bool>(Msi.INSTALLPROPERTY_LUAENABLED, ref luaEnabled);
			}
		}
		string luaEnabled = null;

		public string DisplayName
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_DISPLAYNAME, ref displayName);
			}
		}
		string displayName = null;

		public string MoreInfoUrl
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_MOREINFOURL, ref moreInfoUrl);
			}
		}
		string moreInfoUrl = null;

		public string LocalPackage
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_LOCALPACKAGE, ref localPackage);
			}
		}
		string localPackage = null;

		protected object GetProperty<T>(string property, ref string field)
		{
			// If field is not yet assigned, get product property.
			if (string.IsNullOrEmpty(field))
			{
				if (string.IsNullOrEmpty(property)) throw new ArgumentNullException("property");
				field = GetPatchProperty(property);
			}

			// Based on type T, convert non-null or empty string to T.
			if (!string.IsNullOrEmpty(field))
			{
				Type t = typeof(T);
				if ( t == typeof(bool))
				{
					return string.CompareOrdinal(field.Trim(), "0") != 0;
				}
				else if (t == typeof(DateTime))
				{
					// Dates in yyyyMMdd format.
					return DateTime.ParseExact(field, "yyyyMMdd", null);
				}
				else
				{
					//Everything else, use a TypeConverter.
					TypeConverter converter = TypeDescriptor.GetConverter(t);
					return converter.ConvertFromString(field);
				}
			}

			return default(T);
		}

		string GetPatchProperty(string property)
		{
			int ret = 0;
			StringBuilder sb = new StringBuilder(80);
			int cch = sb.Capacity;

			// Use older MsiGetPatchInfo if no ProductCode is specified.
			if (string.IsNullOrEmpty(productCode) || Msi.CheckVersion(3, 0))
			{
				// Use MsiGetPatchInfoEx for MSI versions 3.0 and newer.
				ret = Msi.MsiGetPatchInfoEx(patchCode, productCode, userSid, context, property, sb, ref cch);
				if (Msi.ERROR_MORE_DATA == ret)
				{
					sb.Capacity = ++cch;
					ret = Msi.MsiGetPatchInfoEx(patchCode, productCode, userSid, context, property, sb, ref cch);
				}
			}
			else
			{
				// Use MsiGetPatchInfo for MSI versions prior to 3.0 or if no ProductCode is specified.
				ret = Msi.MsiGetPatchInfo(patchCode, property, sb, ref cch);
				if (Msi.ERROR_MORE_DATA == ret)
				{
					sb.Capacity = ++cch;
					ret = Msi.MsiGetPatchInfo(patchCode, property, sb, ref cch);
				}
			}

			if (Msi.ERROR_SUCCESS == ret)
			{
				return sb.ToString();
			}

			// Getting this far means an unexpected error occured.
			throw new Win32Exception(ret);
		}
	}
}

