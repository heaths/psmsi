// Represents both advertised and installed Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Wed, 31 Jan 2007 08:13:32 GMT
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
	public abstract class ProductInfo
	{
		string productCode, userSid;
		InstallContext context;

		protected ProductInfo(string productCode, string userSid, InstallContext context)
		{
			// Must at least have a ProductCode.
			if (string.IsNullOrEmpty(productCode))
			{
				throw new ArgumentNullException("productCode");
			}

			// Validate InstallContext and UserSid combinations.
			if (((InstallContext.UserManaged | InstallContext.UserUnmanaged) & context) != 0
				&& string.IsNullOrEmpty(userSid))
			{
				throw new ArgumentException(Resources.Argument_InvalidContextAndSid);
			}

			this.productCode = productCode;
			this.userSid = string.IsNullOrEmpty(userSid) ||
				context == InstallContext.Machine ? null : userSid;
			this.context = context;
		}

		internal static ProductInfo Create(string productCode)
		{
			return Create(productCode, null, InstallContext.Machine);
		}

		internal static ProductInfo Create(string productCode, string userSid, InstallContext context)
		{
			string sid = string.IsNullOrEmpty(userSid) || context == InstallContext.Machine ? null : userSid;

			// Determine if the product is advertised or installed.
			string value = GetProductProperty(productCode, sid, context, Msi.INSTALLPROPERTY_PRODUCTSTATE);
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(ProductState));
			ProductState state = (ProductState)converter.ConvertFromString(value);

			// Return appropriate product derivative or throw an exception.
			if (state == ProductState.Advertised)
			{
				// Common properties, but additional functionality.
				return new AdvertisedProductInfo(productCode, sid, context);
			}
			else if (state == ProductState.Installed)
			{
				// More properties available with installed products.
				return new InstalledProductInfo(productCode, sid, context);
			}
			else throw new NotSupportedException();
		}

		// Common properties used to create this instance.
		public string ProductCode { get { return productCode; } }
		public string UserSid { get { return userSid; } }
		public InstallContext InstallContext { get { return context; } }
		public abstract ProductState ProductState { get; }

		// Common properties to advertised and installed products.
		public string PackageName
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PACKAGENAME, ref packageName);
			}
		}
		string packageName;

		public string Transforms
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_TRANSFORMS, ref transforms);
			}
		}
		string transforms;

		public string Language
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_LANGUAGE, ref language);
			}
		}
		string language;

		public string ProductName
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PRODUCTNAME, ref productName);
			}
		}
		string productName;

		public AssignmentType AssignmentType
		{
			get
			{
				return (AssignmentType)GetProperty<AssignmentType>(Msi.INSTALLPROPERTY_ASSIGNMENTTYPE,
						ref assignmentType);
			}
		}
		string assignmentType;

		public InstanceType InstanceType
		{
			get
			{
				return (InstanceType)GetProperty<InstanceType>(Msi.INSTALLPROPERTY_INSTANCETYPE,
						ref instanceType);
			}
		}
		string instanceType;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LUA")]
        public bool AuthorizedLUAApp
		{
			get
			{
				return (bool)GetProperty<bool>(Msi.INSTALLPROPERTY_AUTHORIZED_LUA_APP,
						ref authorizedLUAApp);
			}
		}
		string authorizedLUAApp;

		public string PackageCode
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PACKAGECODE, ref packageCode);
			}
		}
		string packageCode;

		public string Version
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_VERSION, ref version);
			}
		}
		string version;

		public string ProductIcon
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_PRODUCTICON, ref productIcon);
			}
		}
		string productIcon;

		public string LastUsedSource
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_LASTUSEDSOURCE, ref lastUsedSource);
			}
		}
		string lastUsedSource;

		public string LastUsedType
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_LASTUSEDTYPE, ref lastUsedType);
			}
		}
		string lastUsedType;

		public string MediaPackagePath
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_MEDIAPACKAGEPATH, ref mediaPackagePath);
			}
		}
		string mediaPackagePath;

		public string DiskPrompt
		{
			get
			{
				return (string)GetProperty<string>(Msi.INSTALLPROPERTY_DISKPROMPT, ref diskPrompt);
			}
		}
		string diskPrompt;

        internal virtual string PSPath
        {
            get
            {
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected object GetProperty<T>(string property, ref string field)
		{
			// If field is not yet assigned, get product property.
			if (string.IsNullOrEmpty(field))
			{
				if (string.IsNullOrEmpty(property)) throw new ArgumentNullException("property");
				field = GetProductProperty(productCode, userSid, context, property);
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

		static string GetProductProperty(string productCode, string userSid, InstallContext context, string property)
		{
			int ret = 0;
			StringBuilder sb = new StringBuilder(80);
			int cch = sb.Capacity;

			if (Msi.CheckVersion(3, 0))
			{
				// Use MsiGetProductInfoEx for MSI versions 3.0 and newer.
				ret = Msi.MsiGetProductInfoEx(productCode, userSid, context, property, sb, ref cch);
				if (Msi.ERROR_MORE_DATA == ret)
				{
					sb.Capacity = ++cch;
					ret = Msi.MsiGetProductInfoEx(productCode, userSid, context, property, sb, ref cch);
				}
			}
			else
			{
				// Use MsiGetProductInfo for MSI versions prior to 3.0.
				ret = Msi.MsiGetProductInfo(productCode, property, null, ref cch);
				if (Msi.ERROR_MORE_DATA == ret)
				{
					sb.Capacity = ++cch;
					ret = Msi.MsiGetProductInfo(productCode, property, sb, ref cch);
				}
			}

			if (Msi.ERROR_SUCCESS == ret)
			{
				return sb.ToString();
			}
			else if (Msi.ERROR_UNKNOWN_PROPERTY == ret)
			{
				// MsiGetProductInfo returns ERROR_UNKNOWN_PROPERTY if the product
				// is advertised but not installed. In any event, treat this as non-fatal.
				return null;
			}

			// Getting this far means an unexpected error occured.
			throw new Win32Exception(ret);
		}

	}
}

