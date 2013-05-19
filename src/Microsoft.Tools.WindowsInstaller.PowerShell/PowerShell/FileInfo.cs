// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Provides file type and hash information for ETS.
    /// </summary>
    public static class FileInfo
    {
        /// <summary>
        /// Gets the Windows Installer storage class type.
        /// </summary>
        /// <param name="obj">The wrapped <see cref="System.IO.FileInfo">FileInfo</see> to check.</param>
        /// <returns>The storage class type.</returns>
        public static string GetFileType(PSObject obj)
        {
            // Return null if not a valid FileInfo object.
            if (null == obj)
            {
                return null;
            }

            System.IO.FileInfo fileInfo = obj.BaseObject as System.IO.FileInfo;
            if (null == fileInfo)
            {
                return null;
            }

            // Get the storage class identifier.
            Storage stg = null;
            string path = fileInfo.FullName;

            try
            {
                stg = Storage.OpenStorage(path);

                // Set the friendly name.
                Guid clsid = stg.Clsid;
                if (clsid == NativeMethods.CLSID_MsiPackage)
                {
                    return Properties.Resources.Type_Package;
                }
                else if (clsid == NativeMethods.CLSID_MsiPatch)
                {
                    return Properties.Resources.Type_Patch;
                }
                else if (clsid == NativeMethods.CLSID_MsiTransform)
                {
                    return Properties.Resources.Type_Transform;
                }
                else
                {
                    return null;
                }
            }
            catch (InvalidDataException ex)
            {
                // The file is not a valid OLE storage file.
                throw new PSNotSupportedException(ex.Message, ex);
            }
            catch (Win32Exception ex)
            {
                string message = ex.Message.Replace("%1", path);
                throw new PSNotSupportedException(message, ex);
            }
            finally
            {
                IDisposable disposable = stg as IDisposable;
                if (null != stg)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the first 32 bits of the file hash.
        /// </summary>
        /// <param name="obj">The wrapped <see cref="System.IO.FileInfo">FileInfo</see> to check.</param>
        /// <returns>The first 32 bits of the file hash.</returns>
        public static FileHash GetFileHash(PSObject obj)
        {
            // Return null if not a valid FileInfo object.
            if (null == obj)
            {
                return null;
            }

            System.IO.FileInfo fileInfo = obj.BaseObject as System.IO.FileInfo;
            if (null == fileInfo)
            {
                return null;
            }

            // Get the hash of the file.
            string path = fileInfo.FullName;
            FileHash hash = new FileHash();

            int ret = NativeMethods.MsiGetFileHash(path, 0, hash);
            if (ret != NativeMethods.ERROR_SUCCESS)
            {
                // Write the error record and continue enumerating files.
                Win32Exception ex = new Win32Exception(ret);

                string message = ex.Message.Replace("%1", path);
                throw new PSNotSupportedException(message, ex);
            }
            else
            {
                return hash;
            }
        }
    }
}
