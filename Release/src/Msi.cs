// Native methods for Windows Installer.
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Wed, 31 Jan 2007 07:11:59 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Windows.Installer
{
	static class Msi
	{
		internal const int ERROR_SUCCESS = 0;
		internal const int ERROR_MORE_DATA = 234;
		internal const int ERROR_NO_MORE_ITEMS = 259;
		internal const int ERROR_UNKNOWN_PROPERTY = 1608;

		internal const int IDOK = 1;
		internal const int IDCANCEL = 2;
		internal const int IDABORT = 3;
		internal const int IDRETRY = 4;
		internal const int IDIGNORE = 5;
		internal const int IDYES = 6;
		internal const int IDNO = 7;
	
		internal const int GUID_CHARS = 38;
		internal const int MAX_FEATURE_CHARS = 38;
		internal const int MAX_PATH = 260;
	
		internal const string DLL = "msi.dll";

		// Properties or advertised products
		internal const string INSTALLPROPERTY_PACKAGENAME = "PackageName";
		internal const string INSTALLPROPERTY_TRANSFORMS = "Transforms";
		internal const string INSTALLPROPERTY_LANGUAGE = "Language";
		internal const string INSTALLPROPERTY_PRODUCTNAME = "ProductName";
		internal const string INSTALLPROPERTY_ASSIGNMENTTYPE = "AssignmentType";
		internal const string INSTALLPROPERTY_INSTANCETYPE = "InstanceType";
		internal const string INSTALLPROPERTY_AUTHORIZED_LUA_APP = "AuthorizedLUAApp";
		internal const string INSTALLPROPERTY_PACKAGECODE = "PackageCode";
		internal const string INSTALLPROPERTY_VERSION = "Version";
		internal const string INSTALLPROPERTY_PRODUCTICON = "ProductIcon";

		// Properties for installed products
		internal const string INSTALLPROPERTY_INSTALLEDPRODUCTNAME = "InstalledProductName";
		internal const string INSTALLPROPERTY_VERSIONSTRING = "VersionString";
		internal const string INSTALLPROPERTY_HELPLINK = "HelpLink";
		internal const string INSTALLPROPERTY_HELPTELEPHONE = "HelpTelephone";
		internal const string INSTALLPROPERTY_INSTALLLOCATION = "InstallLocation";
		internal const string INSTALLPROPERTY_INSTALLSOURCE = "InstallSource";
		internal const string INSTALLPROPERTY_INSTALLDATE = "InstallDate";
		internal const string INSTALLPROPERTY_PUBLISHER = "Publisher";
		internal const string INSTALLPROPERTY_LOCALPACKAGE = "LocalPackage";
		internal const string INSTALLPROPERTY_URLINFOABOUT = "URLInfoAbout";
		internal const string INSTALLPROPERTY_URLUPDATEINFO = "URLUpdateInfo";
		internal const string INSTALLPROPERTY_VERSIONMINOR = "VersionMinor";
		internal const string INSTALLPROPERTY_VERSIONMAJOR = "VersionMajor";
		internal const string INSTALLPROPERTY_PRODUCTID = "ProductID";
		internal const string INSTALLPROPERTY_REGCOMPANY = "RegCompany";
		internal const string INSTALLPROPERTY_REGOWNER = "RegOwner";
		internal const string INSTALLPROPERTY_PRODUCTSTATE = "State";

		// Properties for installed patches
		internal const string INSTALLPROPERTY_UNINSTALLABLE = "Uninstallable";
		internal const string INSTALLPROPERTY_PATCHSTATE = "State";
		internal const string INSTALLPROPERTY_PATCHTYPE = "PatchType";
		internal const string INSTALLPROPERTY_LUAENABLED = "LUAEnabled";
		internal const string INSTALLPROPERTY_DISPLAYNAME = "DisplayName";
		internal const string INSTALLPROPERTY_MOREINFOURL = "MoreInfoURL";
		//internal const string INSTALLPROPERTY_LOCALPACKAGE = "LocalPackage";


		// Source list information for advertised packages
		internal const string INSTALLPROPERTY_LASTUSEDSOURCE = "LastUsedSource";
		internal const string INSTALLPROPERTY_LASTUSEDTYPE = "LastUsedType";
		internal const string INSTALLPROPERTY_MEDIAPACKAGEPATH = "MediaPackagePath";
		internal const string INSTALLPROPERTY_DISKPROMPT = "DiskPrompt";

		// Version callback function
		[DllImport(DLL)]
		internal static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);
	
		// Enumerator functions
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumClients(
				string szComponent,
				[MarshalAs(UnmanagedType.U4)] int iProductIndex,
				[Out] StringBuilder lpProductBuf);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumComponentCosts(
				[MarshalAs(UnmanagedType.U4)] int hInstall,
				string szComponent,
				[MarshalAs(UnmanagedType.U4)] int dwIndex,
				[MarshalAs(UnmanagedType.U4)] int iState,
				[Out] StringBuilder lpDriveBuf,
				[MarshalAs(UnmanagedType.U4)] ref int pcchDriveBuf,
				[MarshalAs(UnmanagedType.I4)] out int piCost,
				[MarshalAs(UnmanagedType.I4)] out int pTempCost);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumComponentQualifiers(
				string szComponent,
				[MarshalAs(UnmanagedType.U4)] int iIndex,
				[Out] StringBuilder lpQualifierBuf,
				[MarshalAs(UnmanagedType.U4)] ref int pcchQualifierBuf,
				[Out] StringBuilder lpApplicationDataBuf,
				[MarshalAs(UnmanagedType.U4)] ref int pcchApplicationDataBuf);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumComponents(
				[MarshalAs(UnmanagedType.U4)] int iComponentIndex,
				[Out] StringBuilder lpComponentBuf);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumFeatures(
				string szProduct,
				[MarshalAs(UnmanagedType.U4)] int iFeatureIndex,
				[Out] StringBuilder lpFeatureBuf,
				[Out] StringBuilder lpParentBuf);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumPatches(
				string szProduct,
				[MarshalAs(UnmanagedType.U4)] int iPatchIndex,
				[Out] StringBuilder lpPatchBuf,
				[Out] StringBuilder lpTransformsBuf,
				[MarshalAs(UnmanagedType.U4)] ref int pcchTransformsBuf);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumPatchesEx(
				string szProductcode,
				string szUserSid,
				[MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
				[MarshalAs(UnmanagedType.U4)] PatchState dwFilter,
				[MarshalAs(UnmanagedType.U4)] int dwIndex,
				[Out] StringBuilder szPatchCode,
				[Out] StringBuilder szTargetProductCode,
				[MarshalAs(UnmanagedType.U4)] out InstallContext pdwTargetProductContext,
				[Out] StringBuilder szTargetUserSid,
				[MarshalAs(UnmanagedType.U4)] ref int pcchTargetUserSid);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumProducts(
				[MarshalAs(UnmanagedType.U4)] int iProductIndex,
				[Out] StringBuilder lpProductBuf);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumProductsEx(
				string szProductCode,
				string szUserSid,
				[MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
				[MarshalAs(UnmanagedType.U4)] int dwIndex,
				[Out] StringBuilder szInstalledProductCode,
				[MarshalAs(UnmanagedType.U4)] out InstallContext pdwInstalledContext,
				[Out] StringBuilder szSid,
				[MarshalAs(UnmanagedType.U4)] ref int pcchSid);
	
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiEnumRelatedProducts(
				string lpUpgradeCode,
				[MarshalAs(UnmanagedType.U4)] int dwReserved,
				[MarshalAs(UnmanagedType.U4)] int iProductIndex,
				[Out] StringBuilder lpProductBuf);

		// Query functions
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiGetPatchInfo(
				string szPatch,
				string szAttribute,
				[Out] StringBuilder lpValueBuf,
				[MarshalAs(UnmanagedType.U4)] ref int pcchValueBuf);

		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiGetPatchInfoEx(
				string szPatchCode,
				string szProductCode,
				string szUserSid,
				[MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
				string szProperty,
				[Out] StringBuilder lpValue,
				[MarshalAs(UnmanagedType.U4)] ref int pcchValue);

		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiGetProductInfoEx(
				string szPatchCode,
				string szProductCode,
				string szUserSid,
				[MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
				string szProperty,
				[Out] StringBuilder lpValue,
				[MarshalAs(UnmanagedType.U4)] ref int pcchValue);

		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiGetProductInfo(
				string szProduct,
				string szProperty,
				[Out] StringBuilder lpValueBuf,
				[MarshalAs(UnmanagedType.U4)] ref int pcchValueBuf);

		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiGetProductInfoEx(
				string szProductCode,
				string szUserSid,
				[MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
				string szProperty,
				[Out] StringBuilder lpValue,
				[MarshalAs(UnmanagedType.U4)] ref int pcchValue);

		// Action functions
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiRemovePatches(
				string szPatchList,
				string szProductCode,
				[MarshalAs(UnmanagedType.U4)] InstallType eUninstallType,
				string szPropertyList);

		// Miscellaneous functions
		[DllImport(DLL, CharSet=CharSet.Unicode)]
		internal static extern InstallUIHandler MsiSetExternalUI(
				InstallUIHandler puiHandler,
				[MarshalAs(UnmanagedType.U4)] InstallLogMode dwMessageFilter,
				IntPtr pvContext);

		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern InstallUILevel MsiSetInternalUI(
				[MarshalAs(UnmanagedType.U4)] InstallUILevel dwUILevel,
				ref IntPtr phWnd);

		[DllImport(DLL, CharSet=CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		internal static extern int MsiSourceListEnumSources(
				string szProductOrPatchCode,
				string szUserSid,
				[MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
				[MarshalAs(UnmanagedType.U4)] int dwOptions,
				[MarshalAs(UnmanagedType.U4)] int dwIndex,
				[Out] StringBuilder szSource,
				[MarshalAs(UnmanagedType.U4)] ref int pcchSource);

		// Wrapper methods
		static int _major = 0;
		static int _minor = 0;

		internal static bool CheckVersion(int major, int minor)
		{
			return CheckVersion(major, minor, false);
		}

		internal static bool CheckVersion(int major, int minor, bool throwOtherwise)
		{
			if (_major == 0)
			{
				DLLVERSIONINFO dvi = new DLLVERSIONINFO(0);
				if (ERROR_SUCCESS == DllGetVersion(ref dvi))
				{
					_major = dvi.dwMajorVersion;
					_minor = dvi.dwMinorVersion;
				}
			}

			bool check = _major > major ||
				(_major == major && _minor >= minor);

			if (throwOtherwise && !check)
			{
				throw new NotSupportedException(string.Format(Properties.Resources.MsiRequiredVersion, major, minor));
			}

			return check;
		}

	}

	[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet=CharSet.Unicode)]
	internal delegate int InstallUIHandler(
			IntPtr pvContext,
			[MarshalAs(UnmanagedType.U4)] int iMessageType,
			string szMessage);
	
	[Flags]
	public enum InstallContext : int
	{
		None = 0,
		UserManaged = 1,
		UserUnmanaged = 2,
		Machine = 4,
		All = UserManaged | UserUnmanaged | Machine
	}

	public enum AssignmentType : int
	{
		User = 0,
		Machine = 1
	}

	public enum InstanceType : int
	{
		None = 0,
		Instance = 1
	}

	public enum ProductState : int
	{
		Advertised = 1,
		Installed = 5
	}
	
	[Flags]
	public enum PatchState : int
	{
		Invalid = 0,
		Applied = 1,
		Superseded = 2,
		Obsoleted = 4,
		Registered = 8,
		All = Applied | Superseded | Obsoleted | Registered
	}

	public enum InstallType : int
	{
		Default = 0,
		Network = 1,
		SingleInstance = 2
	}

	[Flags]
	public enum InstallMessage : int
	{
		FatalExit = 0,
		Error = 0x01000000,
		Warning = 0x02000000,
		User = 0x03000000,
		Info = 0x04000000,
		FilesInUse = 0x05000000,
		ResolveSource = 0x06000000,
		OutOfDiskSpace = 0x07000000,
		ActionStart = 0x08000000,
		ActionData = 0x09000000,
		Progress = 0x0a000000,
		CommonData = 0x0b000000,
		Initialize = 0x0c000000,
		Terminate = 0x0d000000,
		ShowDialog = 0x0e000000,
		RMFilesInUse = 0x19000000
	}

	[Flags]
	public enum InstallLogMode : int
	{
		FatalExit = 1 << (InstallMessage.FatalExit >> 24),
		Error = 1 << (InstallMessage.Error >> 24),
		Warning = 1 << (InstallMessage.Warning >> 24),
		User = 1 << (InstallMessage.User >> 24),
		Info = 1 << (InstallMessage.Info >> 24),
		ResolveSource = 1 << (InstallMessage.ResolveSource >> 24),
		OutOfDiskSpace = 1 << (InstallMessage.OutOfDiskSpace >> 24),
		ActionStart = 1 << (InstallMessage.ActionStart >> 24),
		ActionData = 1 << (InstallMessage.ActionData >> 24),
		CommonData = 1 << (InstallMessage.CommonData >> 24),
		PropertyDump = 1 << (InstallMessage.Progress >> 24),
		Verbose = 1 << (InstallMessage.Initialize >> 24),
		ExtraDebug = 1 << (InstallMessage.Terminate >> 24),
		LogOnlyOnError = 1 << (InstallMessage.ShowDialog >> 24),
		Progress = 1 << (InstallMessage.Progress >> 24),
		Initialize = 1 << (InstallMessage.Initialize >> 24),
		Terminate = 1 << (InstallMessage.Terminate >> 24),
		ShowDialog = 1 << (InstallMessage.ShowDialog >> 24),
		FilesInUse = 1 << (InstallMessage.FilesInUse >> 24),
		RMFilesInUse = 1 << (InstallMessage.RMFilesInUse >> 24)
	}

	[Flags]
	public enum InstallUILevel : int
	{
		NoChange = 0,
		Default = 1,
		None = 2,
		Basic = 3,
		Reduced = 4,
		Full = 5,
		EndDialog = 0x80,
		ProgressOnly = 0x40,
		HideCancel = 0x20,
		SourceResOnly = 0x100
	}

	[Flags]
	public enum SourceType : int
	{
		Unknown = 0,
		Network = 1,
		URL = 2,
		Media = 4,
		All = Network | URL | Media
	}

	public enum Code : int
	{
		Product = 0,
		Patch = 0x40000000
	}

	[StructLayout(LayoutKind.Sequential)]
	struct DLLVERSIONINFO
	{
		internal DLLVERSIONINFO(byte reserved)
		{
			cbSize = Marshal.SizeOf(typeof(DLLVERSIONINFO));
			dwMajorVersion = 0;
			dwMinorVersion = 0;
			dwBuildNumber = 0;
			dwPlatformID = Environment.OSVersion.Platform;
		}

		[MarshalAs(UnmanagedType.U4)] internal int cbSize;
		[MarshalAs(UnmanagedType.U4)] internal int dwMajorVersion;
		[MarshalAs(UnmanagedType.U4)] internal int dwMinorVersion;
		[MarshalAs(UnmanagedType.U4)] internal int dwBuildNumber;
		[MarshalAs(UnmanagedType.U4)] internal PlatformID dwPlatformID;

		public static implicit operator Version(DLLVERSIONINFO dvi)
		{
			return new Version(dvi.dwMajorVersion, dvi.dwMinorVersion,
					dvi.dwBuildNumber);
		}
	}
}
