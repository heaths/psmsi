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
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Windows.Installer
{
    sealed class Storage
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        NativeMethods.STGM mode;
        NativeMethods.IStorage stg;

        Storage(NativeMethods.IStorage stg, NativeMethods.STGM mode)
        {
            this.stg = stg;
            this.mode = mode;
        }

        //Storage(Storage parent, string childName)
        //{
        //    this.mode = parent.mode;
        //    parent.stg.OpenStorage(childName, null, this.mode, null, 0, out this.stg);
        //}

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
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                ref iid,
                out stg);

            if (NativeMethods.STG_E_FILEALREADYEXISTS == ret)
            {
                throw new IOException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.File_AlreadyExists, path));
            }
            else if (0 != ret)
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
                    using (NativeMethods.STATSTG stat = new NativeMethods.STATSTG())
                    {
                        stg.Stat(stat, NativeMethods.STATFLAG.STATFLAG_NONAME);
                        clsid = stat.clsid;
                    }
                }

                return clsid;
            }
        }

        //internal IEnumerable<Storage> SubStorages
        //{
        //    get
        //    {
        //        int ret = ERROR_SUCCESS;
        //        IEnumSTATSTG estats;
        //        stg.EnumElements(0, IntPtr.Zero, 0, out estats);
        //        STATSTG[] stats = new STATSTG[1];
        //        int fetched = 0;

        //        while (0 == (ret = estats.Next(1, stats, out fetched)))
        //        {
        //            if (1 != fetched)
        //            {
        //                ret = ERROR_NO_MORE_ITEMS;
        //                break;
        //            }
        //            else if (IID_IStorage == stats[0].clsid)
        //            {
        //                using (stats[0])
        //                {
        //                    yield return new Storage(this, stats[0].Name);
        //                }
        //            }
        //        }

        //        if (ERROR_SUCCESS != ret && ERROR_NO_MORE_ITEMS != ret)
        //        {
        //            throw new System.ComponentModel.Win32Exception(ret);
        //        }
        //    }
        //}
    }
}
