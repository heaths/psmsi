// Native methods for Windows Installer.
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Tue, 10 Jul 2007 04:03:23 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Microsoft.Windows.Installer
{
    sealed class Storage
    {
        internal const string DLL = "ole32.dll";

        const int ERROR_SUCCESS = 0;
        const int ERROR_NO_MORE_ITEMS = 259;

        static readonly Guid IID_IStorage = new Guid("0000000b-0000-0000-C000-000000000046");
        static readonly Guid IID_IPropertySetStorage = new Guid("0000013A-0000-0000-C000-000000000046");
        static readonly Guid IID_IPropertyStorage = new Guid("00000138-0000-0000-C000-000000000046");

        IStorage stg = null;
        STGM mode;

        Storage(IStorage stg, STGM mode)
        {
            this.stg = stg;
            this.mode = mode;
        }

        Storage(Storage parent, string childName)
        {
            this.mode = parent.mode;
            parent.stg.OpenStorage(childName, null, this.mode, null, 0, out this.stg);
        }

        /// <summary>
        /// Opens a storage file in read-only mode.
        /// </summary>
        /// <param name="path">Path to a storage file.</param>
        /// <returns>An instance of the <see cref="Storage"/> class.</returns>
        internal static Storage OpenStorage(string path)
        {
            return OpenStorage(path, true);
        }

        /// <summary>
        /// Opens a storage file.
        /// </summary>
        /// <param name="path">Path to a storage file.</param>
        /// <param name="readOnly">true to open the file read-only mode; otherwise, false to open the file in read-write mode.</param>
        /// <returns>An instance of the <see cref="Storage"/> class.</returns>
        internal static Storage OpenStorage(string path, bool readOnly)
        {
            IStorage stg = null;
            Guid iid = IID_IStorage;

            STGM mode = STGM.STGM_DIRECT;
            mode |= readOnly ? STGM.STGM_READ | STGM.STGM_SHARE_DENY_WRITE : STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE;

            int ret = StgOpenStorageEx(path, mode,
                STGFMT.STGFMT_STORAGE,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                ref iid,
                out stg);

            if (0 != ret)
            {
                throw new System.ComponentModel.Win32Exception(ret);
            }

            return new Storage(stg, mode);
        }

        Guid clsid = Guid.Empty;
        internal Guid Clsid
        {
            get
            {
                if (Guid.Empty == clsid)
                {
                    using (STATSTG stat = new STATSTG())
                    {
                        stg.Stat(stat, STATFLAG.STATFLAG_NONAME);
                        clsid = stat.clsid;
                    }
                }

                return clsid;
            }
        }

        internal IEnumerable<Storage> SubStorages
        {
            get
            {
                int ret = ERROR_SUCCESS;
				IEnumSTATSTG estats;
            	stg.EnumElements(0, IntPtr.Zero, 0, out estats);
                STATSTG[] stats = new STATSTG[1];
                int fetched = 0;

                while (0 == (ret = estats.Next(1, stats, out fetched)))
                {
                    if (1 != fetched)
                    {
                        ret = ERROR_NO_MORE_ITEMS;
                        break;
                    }
                    else if (IID_IStorage == stats[0].clsid)
                    {
                        using (stats[0])
                        {
                            yield return new Storage(this, stats[0].Name);
                        }
                    }
                }

                if (ERROR_SUCCESS != ret && ERROR_NO_MORE_ITEMS != ret)
                {
                    throw new System.ComponentModel.Win32Exception(ret);
                }
            }
        }

        [DllImport(DLL, CharSet = CharSet.Unicode)]
        static extern int StgOpenStorageEx(
            string pwcsName,
            [MarshalAs(UnmanagedType.U4)] STGM grfMode,
            [MarshalAs(UnmanagedType.U4)] STGFMT stgfmt,
            [MarshalAs(UnmanagedType.U4)] int grfAttrs,
            IntPtr pStgOptions,
            IntPtr reserved2,
            ref Guid riid,
            out IStorage ppObjectOpen);
    }

    [ComImport]
    [Guid("0000000b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IStorage
    {
        void CreateStream(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [MarshalAs(UnmanagedType.U4)] STGM grfMode,
            [MarshalAs(UnmanagedType.U4)] int reserved1,
            [MarshalAs(UnmanagedType.U4)] int reserved2,
            out ComTypes.IStream ppstm);

        void OpenStream(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            IntPtr reserved1,
            [MarshalAs(UnmanagedType.U4)] STGM grfMode,
            [MarshalAs(UnmanagedType.U4)] STGM reserved2,
            out ComTypes.IStream ppstm);

        void CreateStorage(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [MarshalAs(UnmanagedType.U4)] STGM grfMode,
            [MarshalAs(UnmanagedType.U4)] int reserved1,
            [MarshalAs(UnmanagedType.U4)] int reserved2,
            out IStorage ppstg);

        void OpenStorage(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            IStorage pstgPriority,
            [MarshalAs(UnmanagedType.U4)] STGM grfMode,
            [MarshalAs(UnmanagedType.LPWStr)] string snbExclude,
            [MarshalAs(UnmanagedType.U4)] int reserved,
            out IStorage ppstg);

        void CopyTo(
            [MarshalAs(UnmanagedType.U4)] int ciidExclude,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Guid[] rgiidExclude,
            [MarshalAs(UnmanagedType.LPWStr)] string snbExclude,
            IStorage pstgDest);

        void MoveElementTo(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            IStorage pstgDest,
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName,
            [MarshalAs(UnmanagedType.U4)] STGMOVE grfFlags);

        void Commit(
            [MarshalAs(UnmanagedType.U4)] STGC grfCommitFlags);

        void Revert();

        void EnumElements(
            [MarshalAs(UnmanagedType.U4)] int reserved1,
            IntPtr reserved2,
            [MarshalAs(UnmanagedType.U4)] int reserved3,
            out IEnumSTATSTG ppenum);

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
            [MarshalAs(UnmanagedType.U4)] STATFLAG grfStatFlag);
    }

    [ComImport]
    [Guid("0000000d-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IEnumSTATSTG
    {
        [PreserveSig]
        int Next(
            [MarshalAs(UnmanagedType.U4)] int celt,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] STATSTG[] rgelt,
            [MarshalAs(UnmanagedType.U4)] out int pceltFetched);

        [PreserveSig]
        int Skip(
            [MarshalAs(UnmanagedType.U4)] int celt);

        void Reset();

        void Clone(
            out IEnumSTATSTG ppenum);
    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    class STATSTG : IDisposable
    {
        internal IntPtr pwcsName;
        [MarshalAs(UnmanagedType.U4)] internal STGTY type;
        [MarshalAs(UnmanagedType.U8)] internal long cbSize;
        internal ComTypes.FILETIME mtime;
        internal ComTypes.FILETIME ctime;
        internal ComTypes.FILETIME atime;
        [MarshalAs(UnmanagedType.U4)] internal STGM grfMode;
        [MarshalAs(UnmanagedType.U4)] internal int grfLockSupported;
        internal Guid clsid;
        [MarshalAs(UnmanagedType.U4)] internal int grfStateBits;
        [MarshalAs(UnmanagedType.U4)] internal int reserved;

        internal string Name
        {
            get
            {
                string name = Marshal.PtrToStringUni(pwcsName);
                return name;
            }
        }

        void IDisposable.Dispose()
        {
            if (IntPtr.Zero != pwcsName)
            {
                Marshal.FreeCoTaskMem(pwcsName);
            }
        }
    }

    [Flags]
    enum STGM : int
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

    enum STGMOVE : int
    {
        STGMOVE_MOVE = 0,
        STGMOVE_COPY = 1,
        STGMOVE_SHALLOWCOPY = 2
    }

    [Flags]
    enum STGC : int
    {
        STGC_DEFAULT = 0,
        STGC_OVERWRITE = 1,
        STGC_ONLYIFCURRENT = 2,
        STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
        STGC_CONSOLIDATE = 8
    }

    enum STGFMT : int
    {
        STGFMT_STORAGE = 0,
        STGFMT_NATIVE = 1,
        STGFMT_FILE = 3,
        STGFMT_ANY = 4,
        STGFMT_DOCFILE = 5
    }

    [Flags]
    enum STATFLAG : int
    {
        STATFLAG_DEFAULT = 0,
        STATFLAG_NONAME = 1,
        STATFLAG_NOOPEN = 2
    }

    enum STGTY : int
    {
        STGTY_STORAGE = 1,
        STGTY_STREAM = 2,
        STGTY_LOCKBYTES = 3,
        STGTY_PROPERTY = 4
    }
}
