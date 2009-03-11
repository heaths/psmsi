// Native methods for Windows Installer.
//
// Created: Tue, 10 Jul 2007 04:03:23 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;

namespace Microsoft.WindowsInstaller
{
    /// <summary>
    /// Lightweight wrapper around OLE structure storages.
    /// </summary>
    internal sealed class Storage
    {
        private NativeMethods.IStorage stg;
        private Guid clsid;

        private Storage(NativeMethods.IStorage stg)
        {
            this.stg = stg;
            this.clsid = Guid.Empty;
        }

        /// <summary>
        /// Opens a storage file.
        /// </summary>
        /// <param name="path">Path to a storage file.</param>
        /// <param name="readOnly">true to open the file read-only mode; otherwise, false to open the file in read-write mode.</param>
        /// <returns>An instance of the <see cref="Storage"/> class.</returns>
        internal static Storage OpenStorage(string path, bool readOnly)
        {
            NativeMethods.IStorage stg = null;
            Guid iid = NativeMethods.IID_IStorage;

            NativeMethods.STGM mode = NativeMethods.STGM.STGM_DIRECT;
            mode |= readOnly ? NativeMethods.STGM.STGM_READ | NativeMethods.STGM.STGM_SHARE_DENY_WRITE
                : NativeMethods.STGM.STGM_READWRITE | NativeMethods.STGM.STGM_SHARE_EXCLUSIVE;

            int ret = NativeMethods.StgOpenStorageEx(path, mode,
                NativeMethods.STGFMT.STGFMT_STORAGE,
                0, IntPtr.Zero, IntPtr.Zero,
                ref iid, out stg);

            if (0 != ret)
            {
                throw new System.ComponentModel.Win32Exception(ret);
            }
            else
            {
                return new Storage(stg);
            }
        }

        /// <summary>
        /// Gets the storage class identifier.
        /// </summary>
        /// <value>The storage class identifier.</value>
        internal Guid Clsid
        {
            get
            {
                if (Guid.Empty == clsid)
                {
                    using (NativeMethods.STATSTG stat = new NativeMethods.STATSTG())
                    {
                        stg.Stat(stat, NativeMethods.STATFLAG.STATFLAG_NONAME);
                        clsid = stat.clsid;
                    }
                }

                return clsid;
            }
        }
    }
}
