// Native methods.
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Sat, 01 Mar 2008 22:58:00 GMT
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

using ComTypes = System.Runtime.InteropServices.ComTypes;

// Default CharSet for any DllImportAttribute that does not declare CharSet.
[module: DefaultCharSet(CharSet.Unicode)]

namespace Microsoft.Windows.Installer
{
    static class NativeMethods
    {
        #region Win32 error codes
        internal const int ERROR_SUCCESS = 0;
        internal const int ERROR_MORE_DATA = 234;
        internal const int ERROR_NO_MORE_ITEMS = 259;
        internal const int ERROR_UNKNOWN_PROPERTY = 1608;
        #endregion

        #region Storage error codes
        internal const int STG_E_FILEALREADYEXISTS = unchecked((int)0x80030050);
        #endregion

        #region Dialog return codes
        internal const int IDOK = 1;
        internal const int IDCANCEL = 2;
        internal const int IDABORT = 3;
        internal const int IDRETRY = 4;
        internal const int IDIGNORE = 5;
        internal const int IDYES = 6;
        internal const int IDNO = 7;
        #endregion

        #region Other constants
        internal const int MAX_GUID_CHARS = 38;
        internal const int MAX_FEATURE_CHARS = 38;
        internal const int MAX_PATH = 260;
        internal const string World = "s-1-1-0";
        #endregion

        #region Properties or advertised products
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
        #endregion

        #region Properties for installed products
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
        #endregion

        #region Properties for installed patches
        internal const string INSTALLPROPERTY_UNINSTALLABLE = "Uninstallable";
        internal const string INSTALLPROPERTY_PATCHSTATE = "State";
        internal const string INSTALLPROPERTY_PATCHTYPE = "PatchType";
        internal const string INSTALLPROPERTY_LUAENABLED = "LUAEnabled";
        internal const string INSTALLPROPERTY_DISPLAYNAME = "DisplayName";
        internal const string INSTALLPROPERTY_MOREINFOURL = "MoreInfoURL";
        #endregion

        #region Source list information for advertised packages
        internal const string INSTALLPROPERTY_LASTUSEDSOURCE = "LastUsedSource";
        internal const string INSTALLPROPERTY_LASTUSEDTYPE = "LastUsedType";
        internal const string INSTALLPROPERTY_MEDIAPACKAGEPATH = "MediaPackagePath";
        internal const string INSTALLPROPERTY_DISKPROMPT = "DiskPrompt";
        #endregion

        #region Interface identifiers
        const string UUID_IStorage = "0000000b-0000-0000-C000-000000000046";
        //const string UUID_IPropertySetStorage = "0000013A-0000-0000-C000-000000000046";
        //const string UUID_IPropertyStorage = "00000138-0000-0000-C000-000000000046";
        const string UUID_IEnumSTATSTG = "0000000d-0000-0000-C000-000000000046";

        internal static readonly Guid IID_IStorage = new Guid(NativeMethods.UUID_IStorage);
        //internal static readonly Guid IID_IPropertySetStorage = new Guid(NativeMethods.UUID_IPropertySetStorage);
        //internal static readonly Guid IID_IPropertyStorage = new Guid(NativeMethods.UUID_IPropertyStorage);
        #endregion

        #region Class identifiers
        internal static readonly Guid CLSID_MsiPackage = new Guid(0xC1084, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        internal static readonly Guid CLSID_MsiPatch = new Guid(0xC1086, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        internal static readonly Guid CLSID_MsiTransform = new Guid(0xC1082, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        #endregion

        #region DllGetVersion functions
        [DllImport(Msi.DLL, EntryPoint = "DllGetVersion")]
        internal static extern int MsiDllGetVersion(NativeMethods.DLLVERSIONINFO pdvi);
        #endregion

        #region INSTALLUI_HANDLER callback function
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate int InstallUIHandler(
                IntPtr pvContext,
                [MarshalAs(UnmanagedType.U4)] int iMessageType,
                string szMessage);
        #endregion

        #region Handle functions
        [DllImport(Msi.DLL), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiCloseHandle(
            [MarshalAs(UnmanagedType.U4)] MsiHandle hAny);
        #endregion

        #region Patch enumeration functions
        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiEnumPatches(
                string szProduct,
                [MarshalAs(UnmanagedType.U4)] int iPatchIndex,
                [Out] StringBuilder lpPatchBuf,
                [Out] StringBuilder lpTransformsBuf,
                [MarshalAs(UnmanagedType.U4)] ref int pcchTransformsBuf);

        [DllImport(Msi.DLL)]
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
        #endregion

        #region Product enumeration functions
        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiEnumProducts(
                [MarshalAs(UnmanagedType.U4)] int iProductIndex,
                [Out] StringBuilder lpProductBuf);

        [DllImport(Msi.DLL)]
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

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiEnumRelatedProducts(
                string lpUpgradeCode,
                [MarshalAs(UnmanagedType.U4)] int dwReserved,
                [MarshalAs(UnmanagedType.U4)] int iProductIndex,
                [Out] StringBuilder lpProductBuf);
        #endregion

        #region Patch information functions
        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiGetPatchInfo(
                string szPatch,
                string szAttribute,
                [Out] StringBuilder lpValueBuf,
                [MarshalAs(UnmanagedType.U4)] ref int pcchValueBuf);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiGetPatchInfoEx(
                string szPatchCode,
                string szProductCode,
                string szUserSid,
                [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
                string szProperty,
                [Out] StringBuilder lpValue,
                [MarshalAs(UnmanagedType.U4)] ref int pcchValue);
        #endregion

        #region Product information functions
        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiGetProductInfo(
                string szProduct,
                string szProperty,
                [Out] StringBuilder lpValueBuf,
                [MarshalAs(UnmanagedType.U4)] ref int pcchValueBuf);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiGetProductInfoEx(
                string szProductCode,
                string szUserSid,
                [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
                string szProperty,
                [Out] StringBuilder lpValue,
                [MarshalAs(UnmanagedType.U4)] ref int pcchValue);
        #endregion

        #region File information functions
        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiGetFileHash(
            string szFilePath,
            [MarshalAs(UnmanagedType.U4)] int dwOptions,
            [MarshalAs(UnmanagedType.LPStruct), Out] FileHashInfo pHash);
        #endregion

        #region Source list functions
        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiSourceListEnumSources(
            string szProductOrPatchCode,
            string szUserSid,
            [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
            [MarshalAs(UnmanagedType.U4)] int dwOptions,
            [MarshalAs(UnmanagedType.U4)] int dwIndex,
            [Out] StringBuilder szSource,
            [MarshalAs(UnmanagedType.U4)] ref int pcchSource);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiSourceListAddSource(
            string szProduct,
            string szUserName, // Format: DOMAIN|MACHINE\USERNAME
            [MarshalAs(UnmanagedType.U4)] int dwReserved,
            string szSource);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiSourceListAddSourceEx(
            string szProductCodeOrPatchCode,
            string szUserSid,
            [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
            [MarshalAs(UnmanagedType.U4)] int dwOptions,
            string szSource,
            [MarshalAs(UnmanagedType.U4)] int dwIndex);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiSourceListClearAll(
            string szProduct,
            string szUserName, // Format: DOMAIN|MACHINE\USERNAME
            [MarshalAs(UnmanagedType.U4)] int dwReserved);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiSourceListClearAllEx(
            string szProductCodeOrPatchCode,
            string szUserSid,
            [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
            [MarshalAs(UnmanagedType.U4)] int dwOptions);

        [DllImport(Msi.DLL)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern int MsiSourceListClearSource(
            string szProductCodeOrPatchCode,
            string szUserSid,
            [MarshalAs(UnmanagedType.U4)] InstallContext dwContext,
            [MarshalAs(UnmanagedType.U4)] int dwOptions,
            string szSource);
        #endregion

        #region Open storage functions
        [DllImport("ole32.dll")]
        internal static extern int StgOpenStorageEx(
            string pwcsName,
            [MarshalAs(UnmanagedType.U4)] NativeMethods.STGM grfMode,
            [MarshalAs(UnmanagedType.U4)] NativeMethods.STGFMT stgfmt,
            [MarshalAs(UnmanagedType.U4)] int grfAttrs,
            IntPtr pStgOptions,
            IntPtr reserved2,
            ref Guid riid,
            out NativeMethods.IStorage ppObjectOpen);
        #endregion

        #region Other structures
        [StructLayout(LayoutKind.Sequential)]
        internal class DLLVERSIONINFO
        {
            internal DLLVERSIONINFO()
            {
                this.cbSize = Marshal.SizeOf(this);
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
        #endregion

        #region Storage interfaces
        [ComImport, Guid(NativeMethods.UUID_IStorage)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IStorage
        {
            void CreateStream(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGM grfMode,
                [MarshalAs(UnmanagedType.U4)] int reserved1,
                [MarshalAs(UnmanagedType.U4)] int reserved2,
                out ComTypes.IStream ppstm);

            void OpenStream(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                IntPtr reserved1,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGM grfMode,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGM reserved2,
                out ComTypes.IStream ppstm);

            void CreateStorage(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGM grfMode,
                [MarshalAs(UnmanagedType.U4)] int reserved1,
                [MarshalAs(UnmanagedType.U4)] int reserved2,
                out NativeMethods.IStorage ppstg);

            void OpenStorage(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                IStorage pstgPriority,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGM grfMode,
                [MarshalAs(UnmanagedType.LPWStr)] string snbExclude,
                [MarshalAs(UnmanagedType.U4)] int reserved,
                out NativeMethods.IStorage ppstg);

            void CopyTo(
                [MarshalAs(UnmanagedType.U4)] int ciidExclude,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Guid[] rgiidExclude,
                [MarshalAs(UnmanagedType.LPWStr)] string snbExclude,
                NativeMethods.IStorage pstgDest);

            void MoveElementTo(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                NativeMethods.IStorage pstgDest,
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGMOVE grfFlags);

            void Commit(
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STGC grfCommitFlags);

            void Revert();

            void EnumElements(
                [MarshalAs(UnmanagedType.U4)] int reserved1,
                IntPtr reserved2,
                [MarshalAs(UnmanagedType.U4)] int reserved3,
                out NativeMethods.IEnumSTATSTG ppenum);

            void DestroyElement(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

            void RenameElement(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsOldName,
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName);

            void SetElementTimes(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                [In] ref ComTypes.FILETIME pctime,
                [In] ref ComTypes.FILETIME patime,
                [In] ref ComTypes.FILETIME pmtime);

            void SetClass(
                [In] ref Guid clsid);

            void SetStateBits(
                [MarshalAs(UnmanagedType.U4)] int grfStateBits,
                [MarshalAs(UnmanagedType.U4)] int grfMask);

            void Stat(
                [Out] STATSTG pstatstg,
                [MarshalAs(UnmanagedType.U4)] NativeMethods.STATFLAG grfStatFlag);
        }

        [ComImport, Guid(NativeMethods.UUID_IEnumSTATSTG)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IEnumSTATSTG
        {
            [PreserveSig]
            int Next(
                [MarshalAs(UnmanagedType.U4)] int celt,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] NativeMethods.STATSTG[] rgelt,
                [MarshalAs(UnmanagedType.U4)] out int pceltFetched);

            [PreserveSig]
            int Skip(
                [MarshalAs(UnmanagedType.U4)] int celt);

            void Reset();

            void Clone(
                out NativeMethods.IEnumSTATSTG ppenum);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class STATSTG : IDisposable
        {
            internal IntPtr pwcsName;
            [MarshalAs(UnmanagedType.U4)] internal NativeMethods.STGTY type;
            [MarshalAs(UnmanagedType.U8)] internal long cbSize;
            internal ComTypes.FILETIME mtime;
            internal ComTypes.FILETIME ctime;
            internal ComTypes.FILETIME atime;
            [MarshalAs(UnmanagedType.U4)] internal NativeMethods.STGM grfMode;
            [MarshalAs(UnmanagedType.U4)] internal int grfLockSupported;
            internal Guid clsid;
            [MarshalAs(UnmanagedType.U4)] internal int grfStateBits;
            [MarshalAs(UnmanagedType.U4)] internal int reserved;

            //internal string Name
            //{
            //    get
            //    {
            //        string name = Marshal.PtrToStringUni(pwcsName);
            //        return name;
            //    }
            //}

            void IDisposable.Dispose()
            {
                if (IntPtr.Zero != pwcsName)
                {
                    Marshal.FreeCoTaskMem(pwcsName);
                }

                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Storage enumerations
        [Flags]
        internal enum STGM : int
        {
            STGM_DIRECT = 0x00000000,
            STGM_TRANSACTED = 0x00010000,
            STGM_SIMPLE = 0x08000000,

            STGM_READ = 0x00000000,
            STGM_WRITE = 0x00000001,
            STGM_READWRITE = 0x00000002,

            STGM_SHARE_DENY_NONE = 0x00000040,
            STGM_SHARE_DENY_READ = 0x00000030,
            STGM_SHARE_DENY_WRITE = 0x00000020,
            STGM_SHARE_EXCLUSIVE = 0x00000010,

            STGM_PRIORITY = 0x00040000,
            STGM_DELETEONRELEASE = 0x04000000,
            STGM_NOSCRATCH = 0x00100000,

            STGM_CREATE = 0x00001000,
            STGM_CONVERT = 0x00010000,
            STGM_FAILIFTHERE = 0x00000000,

            STGM_NOSNAPSHOT = 0x00200000,
            STGM_DIRECT_SWMR = 0x00400000
        }

        internal enum STGMOVE : int
        {
            STGMOVE_MOVE = 0,
            STGMOVE_COPY = 1,
            STGMOVE_SHALLOWCOPY = 2
        }

        [Flags]
        internal enum STGC : int
        {
            STGC_DEFAULT = 0,
            STGC_OVERWRITE = 1,
            STGC_ONLYIFCURRENT = 2,
            STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
            STGC_CONSOLIDATE = 8
        }

        internal enum STGFMT : int
        {
            STGFMT_STORAGE = 0,
            STGFMT_NATIVE = 1,
            STGFMT_FILE = 3,
            STGFMT_ANY = 4,
            STGFMT_DOCFILE = 5
        }

        [Flags]
        internal enum STATFLAG : int
        {
            STATFLAG_DEFAULT = 0,
            STATFLAG_NONAME = 1,
            STATFLAG_NOOPEN = 2
        }

        internal enum STGTY : int
        {
            STGTY_STORAGE = 1,
            STGTY_STREAM = 2,
            STGTY_LOCKBYTES = 3,
            STGTY_PROPERTY = 4
        }
        #endregion
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class MsiHandle : IDisposable
    {
        int handle; // MSIHANDLE is an int.

        MsiHandle()
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Dispose()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                int ret = NativeMethods.MsiCloseHandle(this);
                Debug.Assert(ret != 0);
            }

            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj)
        {
            MsiHandle other = obj as MsiHandle;
            if (null != other)
            {
                return this.handle == other.handle;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.handle;
        }

        public override string ToString()
        {
            return this.handle.ToString(CultureInfo.CurrentCulture);
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
            this.dwFileHashInfoSize = Marshal.SizeOf(this);
        }

        // Use Nullable<Int32> so that directories display correct with format cmdlets
        public int? HashPart1 { get { return 0 == this.dwData0 ? null : new int?(this.dwData0); } }
        public int? HashPart2 { get { return 0 == this.dwData1 ? null : new int?(this.dwData1); } }
        public int? HashPart3 { get { return 0 == this.dwData2 ? null : new int?(this.dwData2); } }
        public int? HashPart4 { get { return 0 == this.dwData3 ? null : new int?(this.dwData3); } }
    }
}
