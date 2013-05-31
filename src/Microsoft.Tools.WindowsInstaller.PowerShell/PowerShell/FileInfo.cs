// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
        /// Gets the Windows Installer file type.
        /// </summary>
        /// <param name="obj">The wrapped <see cref="System.IO.FileInfo">FileInfo</see> to check.</param>
        /// <returns>The Windows Installer file type.</returns>
        /// <exception cref="PSNotSupportedException">An error occured when getting the file type.</exception>
        public static string GetFileType(PSObject obj)
        {
            // Make sure the path exists.
            var path = GetFilePath(obj);
            if (!File.Exists(path))
            {
                return null;
            }

            // Get the Windows Installer file type.
            var type = GetFileTypeInternal(path);
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
        /// <exception cref="PSNotSupportedException">An error occured when hashing the file.</exception>
        public static FileHash GetFileHash(PSObject obj)
        {
            var path = GetFilePath(obj);
            if (!File.Exists(path))
            {
                return null;
            }

            // Get the hash of the file.
            var hash = new FileHash();
            int ret = NativeMethods.MsiGetFileHash(path, 0, hash);

            if (NativeMethods.ERROR_SUCCESS != ret)
            {
                var ex = new Win32Exception(ret);
                var message = ex.Message.Replace("%1", path);

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
        /// <exception cref="PSNotSupportedException">An error occured when getting the file type.</exception>
        internal static FileType GetFileTypeInternal(string path)
        {
            // Make sure the file exists.
            if (!File.Exists(path))
            {
                return FileType.None;
            }

            // Get the class identity from the OLE storage file.
            Storage storage = null;

            try
            {
                storage = Storage.OpenStorage(path);
                var classId = storage.ClassIdentity;

                if (NativeMethods.CLSID_MsiPackage == classId)
                {
                    return FileType.Package;
                }
                else if (NativeMethods.CLSID_MsiPatch == classId)
                {
                    return FileType.Patch;
                }
                else if (NativeMethods.CLSID_MsiTransform == classId)
                {
                    return FileType.Transform;
                }
            }
            catch (Win32Exception ex)
            {
                // Only truly exceptional errors should be thrown.
                var message = ex.Message.Replace("%1", path);
                throw new PSNotSupportedException(message, ex);
            }
            catch
            {
                // Ignore all other exceptions.
            }
            finally
            {
                if (null != storage)
                {
                    storage.Dispose();
                }
            }

            return FileType.Other;
        }

        private static string GetFilePath(PSObject obj)
        {
            // No SessionState is passed, so make sure we're dealing with a valid file.
            if (null != obj)
            {
                var file = obj.BaseObject as System.IO.FileInfo;
                if (null != file)
                {
                    return file.FullName;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// The type of Windows Installer file.
    /// </summary>
    internal enum FileType
    {
        /// <summary>
        /// A file path was not specified or does not exist.
        /// </summary>
        None,

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
