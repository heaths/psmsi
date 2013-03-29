// Native methods for Windows Installer.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Lightweight wrapper around OLE structure storages.
    /// </summary>
    internal sealed class Storage : IDisposable
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
        /// <returns>An instance of the <see cref="Storage"/> class.</returns>
        /// <exception cref="System.IO.InvalidDataException">The file format is not supported.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Windows errors returned by OLE storage.</exception>
        internal static Storage OpenStorage(string path)
        {
            NativeMethods.IStorage stg = null;
            Guid iid = NativeMethods.IID_IStorage;

            int ret = NativeMethods.StgOpenStorageEx(path,
                NativeMethods.STGM.STGM_READ | NativeMethods.STGM.STGM_SHARE_DENY_WRITE,
                NativeMethods.STGFMT.STGFMT_STORAGE,
                0, IntPtr.Zero, IntPtr.Zero,
                ref iid, out stg);

            if (NativeMethods.STG_E_FILEALREADYEXISTS == ret)
            {
                // 0x80030050 is a rather odd error string, so return something more appropriate.
                throw new System.IO.InvalidDataException(
                    string.Format(Properties.Resources.Error_InvalidStorage, path),
                    new System.ComponentModel.Win32Exception(ret)
                    );
            }
            else if (0 != ret)
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

        /// <summary>
        /// Disposes the object immediately.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.stg = null;
            GC.SuppressFinalize(this);
        }
    }
}
