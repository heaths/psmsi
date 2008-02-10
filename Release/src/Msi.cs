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
using System.Runtime.ConstrainedExecution;
using System.Globalization;

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

        // STATSTG.clsid values
        internal static readonly Guid CLSID_MsiPackage = new Guid(0xC1084, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        internal static readonly Guid CLSID_MsiPatch = new Guid(0xC1086, 0x0, 0x0, new byte[] {0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46});
        internal static readonly Guid CLSID_MsiTransform = new Guid(0xC1082, 0x0, 0x0, new byte[] {0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46});

        internal const string MsiPackage = "Package";
        internal const string MsiPatch = "Patch";
        internal const string MsiTransform = "Transform";

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

        //[DllImport(DLL)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiCloseHandle(
        //    [MarshalAs(UnmanagedType.U4)] int hAny);

		// Enumerator functions
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiEnumClients(
        //        string szComponent,
        //        [MarshalAs(UnmanagedType.U4)] int iProductIndex,
        //        [Out] StringBuilder lpProductBuf);
	
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiEnumComponentCosts(
        //        [MarshalAs(UnmanagedType.U4)] int hInstall,
        //        string szComponent,
        //        [MarshalAs(UnmanagedType.U4)] int dwIndex,
        //        [MarshalAs(UnmanagedType.U4)] int iState,
        //        [Out] StringBuilder lpDriveBuf,
        //        [MarshalAs(UnmanagedType.U4)] ref int pcchDriveBuf,
        //        [MarshalAs(UnmanagedType.I4)] out int piCost,
        //        [MarshalAs(UnmanagedType.I4)] out int pTempCost);
	
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiEnumComponentQualifiers(
        //        string szComponent,
        //        [MarshalAs(UnmanagedType.U4)] int iIndex,
        //        [Out] StringBuilder lpQualifierBuf,
        //        [MarshalAs(UnmanagedType.U4)] ref int pcchQualifierBuf,
        //        [Out] StringBuilder lpApplicationDataBuf,
        //        [MarshalAs(UnmanagedType.U4)] ref int pcchApplicationDataBuf);
	
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiEnumComponents(
        //        [MarshalAs(UnmanagedType.U4)] int iComponentIndex,
        //        [Out] StringBuilder lpComponentBuf);
	
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiEnumFeatures(
        //        string szProduct,
        //        [MarshalAs(UnmanagedType.U4)] int iFeatureIndex,
        //        [Out] StringBuilder lpFeatureBuf,
        //        [Out] StringBuilder lpParentBuf);
	
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
				[MarshalAs(UnmanagedType.U4)] PatchStates dwFilter,
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

        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiGetProductInfoEx(
        //        string szPatchCode,
        //        string szProductCode,
        //        string szUserSid,
        //        [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
        //        string szProperty,
        //        [Out] StringBuilder lpValue,
        //        [MarshalAs(UnmanagedType.U4)] ref int pcchValue);

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

        [DllImport(DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiGetFileHash(
            string szFilePath,
            [MarshalAs(UnmanagedType.U4)] int dwOptions,
            [MarshalAs(UnmanagedType.LPStruct), Out] FileHashInfo pHash);

        // Database functions

        // Creates a database handle by path.
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiOpenDatabase(
        //    string szDatabasePath,
        //    [MarshalAs(UnmanagedType.SysUInt)] DBOpen szPersist,
        //    out MsiHandle phDatabase);

        // Get a database handle from a product handle.
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return"), DllImport(DLL, CharSet = CharSet.Unicode)]
        //internal static extern MsiHandle MsiGetActiveDatabase(MsiHandle hInstall);

        // Action functions
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern int MsiRemovePatches(
        //        string szPatchList,
        //        string szProductCode,
        //        [MarshalAs(UnmanagedType.U4)] InstallType eUninstallType,
        //        string szPropertyList);

		// Miscellaneous functions
        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //internal static extern InstallUIHandler MsiSetExternalUI(
        //        InstallUIHandler puiHandler,
        //        [MarshalAs(UnmanagedType.U4)] InstallLogModes dwMessageFilter,
        //        IntPtr pvContext);

        //[DllImport(DLL, CharSet=CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.U4)]
        //internal static extern InstallUILevels MsiSetInternalUI(
        //        [MarshalAs(UnmanagedType.U4)] InstallUILevels dwUILevel,
        //        ref IntPtr phWnd);

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
		static int _major;
		static int _minor;

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
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.MsiRequiredVersion, major, minor));
			}

			return check;
		}

	}

    [StructLayout(LayoutKind.Sequential)]
    sealed class MsiHandle// : CriticalFinalizerObject, IDisposable
    {
        // The MSIHANDLE.
        int handle;

        const int InvalidHandleValue = 0;

        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        //internal MsiHandle()
        //{
        //    this.handle = InvalidHandleValue;
        //    GC.SuppressFinalize(this);
        //}

        //~MsiHandle()
        //{
        //    Dispose(false);
        //}

        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        //void Dispose(bool disposing)
        //{
        //    Msi.MsiCloseHandle(handle);
        //}

        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        //public void Dispose()
        //{
        //    Dispose(true);
        //}

        //public override bool Equals(object obj)
        //{
        //    MsiHandle other = obj as MsiHandle;
        //    if (null != other)
        //    {
        //        return handle == other.handle;
        //    }

        //    return base.Equals(obj);
        //}

        //public override int GetHashCode()
        //{
        //    return handle;
        //}

        //public override string ToString()
        //{
        //    return handle.ToString();
        //}
    }

	[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet=CharSet.Unicode)]
	internal delegate int InstallUIHandler(
			IntPtr pvContext,
			[MarshalAs(UnmanagedType.U4)] int iMessageType,
			string szMessage);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), Flags]
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum ProductState : int
	{
		Advertised = 1,
		Installed = 5
	}

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
	public enum PatchStates : int
	{
		Invalid = 0,
		Applied = 1,
		Superseded = 2,
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Obsoleted")]
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags"), Flags]
	public enum InstallMessages : int
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
	public enum InstallLogModes : int
	{
		FatalExit = 1 << (InstallMessages.FatalExit >> 24),
        Error = 1 << (InstallMessages.Error >> 24),
        Warning = 1 << (InstallMessages.Warning >> 24),
        User = 1 << (InstallMessages.User >> 24),
        Info = 1 << (InstallMessages.Info >> 24),
        ResolveSource = 1 << (InstallMessages.ResolveSource >> 24),
        OutOfDiskSpace = 1 << (InstallMessages.OutOfDiskSpace >> 24),
        ActionStart = 1 << (InstallMessages.ActionStart >> 24),
        ActionData = 1 << (InstallMessages.ActionData >> 24),
        CommonData = 1 << (InstallMessages.CommonData >> 24),
        PropertyDump = 1 << (InstallMessages.Progress >> 24),
        Verbose = 1 << (InstallMessages.Initialize >> 24),
        ExtraDebug = 1 << (InstallMessages.Terminate >> 24),
        LogOnlyOnError = 1 << (InstallMessages.ShowDialog >> 24),
        Progress = 1 << (InstallMessages.Progress >> 24),
        Initialize = 1 << (InstallMessages.Initialize >> 24),
        Terminate = 1 << (InstallMessages.Terminate >> 24),
        ShowDialog = 1 << (InstallMessages.ShowDialog >> 24),
        FilesInUse = 1 << (InstallMessages.FilesInUse >> 24),
        RMFilesInUse = 1 << (InstallMessages.RMFilesInUse >> 24)
	}

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
	public enum InstallUILevels : int
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
	public enum SourceTypes : int
	{
		None = 0,
		Network = 1,
		Url = 2,
		Media = 4,
		All = Network | Url | Media
	}

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum Code : int
	{
		Product = 0,
        Patch = 0x40000000
	}

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
    public enum DBOpen : int
    {
        ReadOnly = 0,
        Transact,
        Direct,
        Create,
        CreateDirect,
        PatchFile = 32
    }

	[StructLayout(LayoutKind.Sequential)]
	struct DLLVERSIONINFO
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "reserved")]
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

    [StructLayout(LayoutKind.Sequential)]
    public class FileHashInfo
    {
        [MarshalAs(UnmanagedType.U4)] int dwFileHashInfoSize;
        [MarshalAs(UnmanagedType.U4)] int dwData0;
        [MarshalAs(UnmanagedType.U4)] int dwData1;
        [MarshalAs(UnmanagedType.U4)] int dwData2;
        [MarshalAs(UnmanagedType.U4)] int dwData3;

        internal FileHashInfo()
        {
            dwFileHashInfoSize = Marshal.SizeOf(this.GetType());
        }

        public int HashPart1 { get { return dwData0; } }
        public int HashPart2 { get { return dwData1; } }
        public int HashPart3 { get { return dwData2; } }
        public int HashPart4 { get { return dwData3; } }
    }
}
