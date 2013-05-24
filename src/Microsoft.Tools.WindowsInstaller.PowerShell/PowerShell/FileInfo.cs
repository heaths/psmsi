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
        /// <exception cref="PSNotSupportedException">The file is missing or is not a valid Windows Installer file.</exception>
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

            // Get the Windows Installer file type.
            var type = GetFileTypeInternal(fileInfo.FullName);
            switch (type)
            {
                case FileType.Package:
                    return Properties.Resources.Type_Package;

                case FileType.Patch:
                    return Properties.Resources.Type_Patch;

                case FileType.Transform:
                    return Properties.Resources.Type_Transform;

                default:
                    return null;
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

        /// <summary>
        /// Gets the <see cref="FileType"/> of the file referenced by the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file to check.</param>
        /// <returns>The <see cref="FileType"/> of the file referenced by the given <paramref name="path"/>.</returns>
        /// <exception cref="PSNotSupportedException">The file is missing or is not a valid Windows Installer file.</exception>
        internal static FileType GetFileTypeInternal(string path)
        {
            Storage stg = null;

            try
            {
                stg = Storage.OpenStorage(path);

                // Set the friendly name.
                Guid clsid = stg.Clsid;
                if (clsid == NativeMethods.CLSID_MsiPackage)
                {
                    return FileType.Package;
                }
                else if (clsid == NativeMethods.CLSID_MsiPatch)
                {
                    return FileType.Patch;
                }
                else if (clsid == NativeMethods.CLSID_MsiTransform)
                {
                    return FileType.Transform;
                }
                else
                {
                    return FileType.Other;
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
    }

    /// <summary>
    /// The type of Windows Installer file.
    /// </summary>
    internal enum FileType
    {
        /// <summary>
        /// An MSI package.
        /// </summary>
        Package,

        /// <summary>
        /// An MSP patch.
        /// </summary>
        Patch,

        /// <summary>
        /// An MST transform.
        /// </summary>
        Transform,

        /// <summary>
        /// Not a valid Windows Installer file.
        /// </summary>
        Other,
    }
}
