// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Converts paths from one form to another.
    /// </summary>
    internal static class PathConverter
    {
        /// <summary>
        /// Converts a component key path to a provider-qualified PSPath.
        /// </summary>
        /// <param name="session">The <see cref="SessionState"/> for the current execution context.</param>
        /// <param name="path">The component key path to convert.</param>
        /// <returns>A PSPath or null if passed a null path.</returns>
        internal static string FromKeyPathToPSPath(SessionState session, string path)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            else if (path == null)
            {
                // Probably no key path.
                return null;
            }

            // Detect UNC paths: string stars with two backslashes.
            if (path.StartsWith(@"\\", StringComparison.Ordinal))
            {
                return PathConverter.ToPSPath(session, path);
            }

            // Get the prefix to determine the type. Windows Installer rarely uses
            // a '?' instead of a ':' so search for both.
            int pos = path.IndexOfAny(new char[] { ':', '?'});

            // Detect file system paths: single drive letter followed by a colon.
            if (pos == 1)
            {
                // Translate the '?' if used.
                if (path[pos] == '?')
                {
                    path = string.Concat(path.Substring(0, pos), ":", path.Substring(pos + 1));
                }

                return PathConverter.ToPSPath(session, path);
            }

            // Detect registry key paths: 2 digits followed by a colon.
            else if (pos == 2)
            {
                string root = null;
                switch (path.Substring(0, pos))
                {
                    case "00":
                        root = "HKEY_CLASSES_ROOT";
                        break;

                    case "01":
                        root = "HKEY_CURRENT_USER";
                        break;

                    case "02":
                        root = "HKEY_LOCAL_MACHINE";
                        break;

                    case "03":
                        root = "HKEY_USERS";
                        break;

                    default:
                        // Not supported, but not an error.
                        return null;
                }

                // Not all the roots have drives, so we have to hard code the provider-qualified root.
                return string.Concat(@"Microsoft.PowerShell.Core\Registry::", root, path.Substring(pos + 1));
            }

            // Fallback to return null (not supported, but not an error).
            else
            {
                return null;
            }
        }

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

            // TODO: Replace all instances of containing method with callee.
            return session.Path.GetUnresolvedPSPathFromProviderPath(path);
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
