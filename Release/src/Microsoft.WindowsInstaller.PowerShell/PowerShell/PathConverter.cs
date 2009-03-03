// Converts paths from one form to another.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Sun, 01 Mar 2009 09:30:02 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.IO;
using System.Management.Automation;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Converts paths from one form to another.
    /// </summary>
    internal static class PathConverter
    {
        /// <summary>
        /// Converts an unqualified path to a provider-qualified PSPath.
        /// </summary>
        /// <param name="session">The <see cref="SessionState"/> for the current execution context.</param>
        /// <param name="path">The provider path to convert.</param>
        /// <returns>A PSPath or null if passed a null path.</returns>
        internal static string ToPSPath(SessionState session, string path)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            else if (path == null)
            {
                return null;
            }

            PathIntrinsics pi = session.Path;
            if (!pi.IsProviderQualified(path))
            {
                // Get the drive info in order to qualify the path.
                string driveName = null;
                if (!pi.IsPSAbsolute(path, out driveName))
                {
                    PathInfo current = pi.CurrentFileSystemLocation;
                    driveName = current.Drive.Name;
                    path = pi.Combine(current.ProviderPath, path);
                }

                // Format the path as a PSPath using the current provider.
                PSDriveInfo drive = session.Drive.Get(driveName);
                if (drive != null)
                {
                    ProviderInfo provider = drive.Provider;
                    return string.Concat(provider.ModuleName, @"\", provider.Name, @"::", path);
                }
            }
            else
            {
                // Return the provider-qualified path.
                return path;
            }

            // If we got this far, throw an exception.
            throw new ArgumentException(string.Format(Properties.Resources.Error_CannotConvertPath, path), "path");
        }

        /// <summary>
        /// Converts a PSPath to a provider path.
        /// </summary>
        /// <param name="session">The <see cref="SessionState"/> for the current execution context.</param>
        /// <param name="path">The PSPath to convert.</param>
        /// <returns>A provider path or null if passed a null path.</returns>
        internal static string ToProviderPath(SessionState session, string path)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            else if (path == null)
            {
                return null;
            }

            // Get the provider path.
            PathIntrinsics pi = session.Path;
            return pi.GetUnresolvedProviderPathFromPSPath(path);
        }
    }
}
