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
using Microsoft.Windows.Installer.PowerShell;

using ComTypes = System.Runtime.InteropServices.ComTypes;

// Default CharSet for any DllImportAttribute that does not declare CharSet.
[module: DefaultCharSet(CharSet.Unicode)]

namespace Microsoft.Windows.Installer
{
    /// <summary>
    /// Native methods, structures, enumerations, and constants.
    /// </summary>
    internal static class NativeMethods
    {
        #region Other constants
        internal static readonly string World = "s-1-1-0";
        #endregion

        #region Interface identifiers
        internal static readonly Guid IID_IStorage = new Guid(0xb, 0x0, 0x0, new byte[] { 0xc0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        #endregion

        #region Storage class identifiers
        internal static readonly Guid CLSID_MsiPackage = new Guid(0xC1084, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        internal static readonly Guid CLSID_MsiPatch = new Guid(0xC1086, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
        internal static readonly Guid CLSID_MsiTransform = new Guid(0xC1082, 0x0, 0x0, new byte[] { 0xC0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x46 });
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

        #region Storage interfaces
        [ComImport, Guid("0000000b-0000-0000-c000-000000000046")]
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
                [MarshalAs(UnmanagedType.U4)] int grfFlags);

            void Commit(
                [MarshalAs(UnmanagedType.U4)] int grfCommitFlags);

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

        [ComImport, Guid("0000000d-0000-0000-C000-000000000046")]
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
}
