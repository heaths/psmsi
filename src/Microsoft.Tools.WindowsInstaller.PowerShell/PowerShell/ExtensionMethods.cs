// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Matches a string using any of the wildcard <paramref name="patterns"/>.
        /// </summary>
        /// <param name="source">The <see cref="String"/> to be matched.</param>
        /// <param name="patterns">A list of <see cref="WilcardPattern"/> objects to match.</param>
        /// <returns>True if the string matches any of the <paramref name="patterns"/>; otherwise, false if the string or patterns are null, empty, or not match is found.</returns>
        internal static bool Match(this string source, IList<WildcardPattern> patterns)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }
            else if (null == patterns || 0 == patterns.Count)
            {
                return false;
            }

            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(source))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an unresolved qualified path from a component key path.
        /// </summary>
        /// <param name="source">A <see cref="PathIntrinsics"/> object.</param>
        /// <param name="path">The path to convert.</param>
        /// <returns>An unresolved qualified path.</returns>
        internal static string GetUnresolvedPSPathFromKeyPath(this PathIntrinsics source, string path)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (string.IsNullOrEmpty(path))
            {
                // Probably no key path.
                return null;
            }

            // Treat UNC paths as normal FileSystem paths.
            if (path.StartsWith(@"\\", StringComparison.Ordinal))
            {
                return source.GetUnresolvedPSPathFromProviderPath(path);
            }

            // Get the path prefix to determine the path type.
            // Windows Installer will sometimes use a ? instead of : so search for both.
            int pos = path.IndexOfAny(new char[] { ':', '?' });

            // File system key paths.
            if (1 == pos)
            {
                // Translate a ? to a :.
                if ('?' == path[pos])
                {
                    path = path.Substring(0, pos) + ":" + path.Substring(1 + pos);
                }

                return source.GetUnresolvedPSPathFromProviderPath(path);
            }

            // Registry key paths.
            else if (2 == pos)
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
                        // Not an error, but not valid either.
                        return null;
                }

                // Not all the roots have drives, so we have to hard code the provider-qualified root.
                return @"Microsoft.PowerShell.Core\Registry::" + root + path.Substring(1 + pos);
            }

            // Fallback: not an error, but not valid either.
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an unresolved, provider-qualified path string.
        /// </summary>
        /// <param name="source">A <see cref="PathIntrinsics"/> object.</param>
        /// <param name="path">The path to resolve and format.</param>
        /// <returns>A resolved, provider-qualified path string.</returns>
        internal static string GetUnresolvedPSPathFromProviderPath(this PathIntrinsics source, string path)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (string.IsNullOrEmpty(path))
            {
                // Pass through.
                return null;
            }

            ProviderInfo provider;
            PSDriveInfo drive;

            path = source.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive);
            return provider.ModuleName + @"\" + provider.Name + "::" + path;
        }

        /// <summary>
        /// Returns a <see cref="PSObject"/> wrapper for a <see cref="ProductInstallation"/> object and attaches special properties.
        /// </summary>
        /// <param name="source">The <see cref="ProductInstallation"/> object to convert.</param>
        /// <param name="provider">A <see cref="PathIntrinsics"/> provider used to convert paths.</param>
        /// <returns>A <see cref="PSObject"/> wrapper with attached special properties.</returns>
        internal static PSObject ToPSObject(this ProductInstallation source, PathIntrinsics provider = null)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            var obj = PSObject.AsPSObject(source);

            // Add path information if possible.
            if (null != provider)
            {
                var path = provider.GetUnresolvedPSPathFromProviderPath(source.LocalPackage);

                var property = obj.Properties.Match("PSPath", PSMemberTypes.Properties).FirstOrDefault();
                if (null != property)
                {
                    property.Value = path;
                }
                else
                {
                    property = new PSNoteProperty("PSPath", path);
                    obj.Properties.Add(property);
                }
            }

            return obj;
        }

        /// <summary>
        /// Returns a <see cref="PSObject"/> wrapper for a <see cref="PatchInstallation"/> object and attaches special properties.
        /// </summary>
        /// <param name="source">The <see cref="PatchInstallation"/> object to convert.</param>
        /// <param name="provider">A <see cref="PathIntrinsics"/> provider used to convert paths.</param>
        /// <returns>A <see cref="PSObject"/> wrapper with attached special properties.</returns>
        internal static PSObject ToPSObject(this PatchInstallation source, PathIntrinsics provider = null)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            var obj = PSObject.AsPSObject(source);

            // Add path information if possible.
            if (null != provider)
            {
                var path = provider.GetUnresolvedPSPathFromProviderPath(source.LocalPackage);
                var property = obj.Properties.Match("PSPath", PSMemberTypes.Properties).FirstOrDefault();
                if (null != property)
                {
                    property.Value = path;
                }
                else
                {
                    property = new PSNoteProperty("PSPath", path);
                    obj.Properties.Add(property);
                }
            }

            return obj;
        }
    }
}
