// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
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
                throw new ArgumentNullException("path");
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
