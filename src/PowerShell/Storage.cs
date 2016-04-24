// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Lightweight wrapper around OLE structure storages.
    /// </summary>
    internal sealed class Storage : IDisposable
    {
        private NativeMethods.IStorage storage;
        private Guid classId;

        private Storage(NativeMethods.IStorage stg)
        {
            this.storage = stg;
            this.classId = Guid.Empty;
        }

        /// <summary>
        /// Opens an OLE storage file.
        /// </summary>
        /// <param name="path">Path to a storage file.</param>
        /// <returns>An instance of the <see cref="Storage"/> class.</returns>
        /// <exception cref="FileNotFoundException">The file path does not exist.</exception>
        /// <exception cref="InvalidDataException">The file is not an OLE storage file..</exception>
        /// <exception cref="Win32Exception">Windows errors returned by OLE storage.</exception>
        internal static Storage OpenStorage(string path)
        {
            // Make sure the file exists.
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            // Try to open the file as an OLE storage file.
            NativeMethods.IStorage stg = null;
            Guid iid = NativeMethods.IID_IStorage;

            int ret = NativeMethods.StgOpenStorageEx(
                path,
                NativeMethods.STGM.STGM_READ | NativeMethods.STGM.STGM_SHARE_DENY_WRITE,
                NativeMethods.STGFMT.STGFMT_STORAGE,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                ref iid,
                out stg);

            if (NativeMethods.STG_E_FILEALREADYEXISTS == ret)
            {
                // 0x80030050 is a rather odd error string, so return something more appropriate.
                var message = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Error_InvalidStorage, path);
                throw new InvalidDataException(message, new Win32Exception(ret));
            }
            else if (0 != ret)
            {
                throw new Win32Exception(ret);
            }
            else
            {
                return new Storage(stg);
            }
        }

        /// <summary>
        /// Gets the storage class identity.
        /// </summary>
        internal Guid ClassIdentity
        {
            get
            {
                if (Guid.Empty == this.classId)
                {
                    using (NativeMethods.STATSTG stat = new NativeMethods.STATSTG())
                    {
                        this.storage.Stat(stat, NativeMethods.STATFLAG.STATFLAG_NONAME);
                        this.classId = stat.clsid;
                    }
                }

                return this.classId;
            }
        }

        /// <summary>
        /// Closes the OLE storage file.
        /// </summary>
        public void Dispose()
        {
            this.storage = null;
            GC.SuppressFinalize(this);
        }
    }
}
