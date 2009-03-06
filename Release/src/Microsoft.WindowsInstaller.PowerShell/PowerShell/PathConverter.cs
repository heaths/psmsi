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
using System.Collections.ObjectModel;
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
        /// Converts a path to a provider-qualified PSPath.
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

            ProviderInfo provider = null;
            PSDriveInfo drive = null;

            // Get the unresolved path information.
            string unresolvedPath = session.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive);

            // Return the fully-qualified PSPath.
            return string.Concat(provider.ModuleName, @"\", provider.Name, "::", unresolvedPath);
        }

        /// <summary>
        /// Converts a PSPath to an unqualified provider path.
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

            ProviderInfo provider = null;
            PSDriveInfo drive = null;

            // Get the unresolved provider path and let the APIs return errors.
            return session.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive);
        }
    }
}
