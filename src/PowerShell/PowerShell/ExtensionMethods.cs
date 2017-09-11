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
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        private static readonly string DirectorySeparator = @"\";
        private static readonly string DriveSeparator = ":";
        private static readonly char[] DriveSeparators = new char[] { ':', '?' };
        private static readonly string ProviderSeparator = "::";
        private static readonly string RegistryProvider = @"Microsoft.PowerShell.Core\Registry::";

        internal static T As<T>(this PSObject obj)
            where T : class
        {
            if (null != obj)
            {
                return obj.BaseObject as T;
            }

            return null;
        }

        /// <summary>
        /// Matches a string using any of the wildcard <paramref name="patterns"/>.
        /// </summary>
        /// <param name="source">The <see cref="string"/> to be matched.</param>
        /// <param name="patterns">A list of <see cref="WildcardPattern"/> objects to match.</param>
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
            if (path.StartsWith(DirectorySeparator, StringComparison.Ordinal))
            {
                return source.GetUnresolvedPSPathFromProviderPath(path);
            }

            // Get the path prefix to determine the path type.
            // Windows Installer will sometimes use a ? instead of : so search for both.
            var pos = path.IndexOfAny(DriveSeparators);

            // File system key paths.
            if (1 == pos)
            {
                // Translate a ? to a :.
                if ('?' == path[pos])
                {
                    path = path.Substring(0, pos) + DriveSeparator + path.Substring(1 + pos);
                }

                return source.GetUnresolvedPSPathFromProviderPath(path);
            }

            // Registry key paths.
            else if (2 == pos)
            {
                // Map the key path based on the current process's bitness.
                var view = RegistryView.GetInstance();
                path = view.MapKeyPath(path);

                if (!string.IsNullOrEmpty(path))
                {
                    // Strip the trailing backslash (for registry keys) or full registry value
                    // since the Registry provider cannot represent registry values in a PSPath.
                    pos = path.LastIndexOf(DirectorySeparator, StringComparison.Ordinal);
                    path = path.Substring(0, pos);

                    // Not all the roots have drives, so we have to hard code the provider-qualified root.
                    return RegistryProvider + path;
                }
            }

            // Fallback: not an error, but not valid either.
            return null;
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
            return provider.ModuleName + DirectorySeparator + provider.Name + ProviderSeparator + path;
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
                obj.SetPropertyValue<string>("PSPath", path);
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
                obj.SetPropertyValue<string>("PSPath", path);
            }

            return obj;
        }

        /// <summary>
        /// Gets the named property value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property value to get.</typeparam>
        /// <param name="source">The <see cref="PSObject"/> that may contain the named property.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The named property value of type <typeparamref name="T"/>,
        /// or the default value for <typeparamref name="T"/> if the property is not found or cannot be converted..</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null or empty.</exception>
        /// <exception cref="PSInvalidCastException">The property value type cannot be converted to type <typeparamref name="T"/>.</exception>
        internal static T GetPropertyValue<T>(this PSObject source, string propertyName)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            var property = source.Properties.Match(propertyName, PSMemberTypes.Properties).FirstOrDefault();
            if (null != property)
            {
                if (property.Value is T)
                {
                    return (T)property.Value;
                }
                else
                {
                    return (T)LanguagePrimitives.ConvertTo(property.Value, typeof(T), CultureInfo.InvariantCulture);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Sets or adds the named property value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property value to set.</typeparam>
        /// <param name="source">The <see cref="PSObject"/> that may contain the named property.</param>
        /// <param name="propertyName">The name of the property to set; if not found, the a <see cref="PSNoteProperty"/> is added.</param>
        /// <param name="propertyValue">The value of the property to set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null or empty.</exception>
        /// <exception cref="PSInvalidCastException">The property value type cannot be converted to type <typeparamref name="T"/>.</exception>
        internal static void SetPropertyValue<T>(this PSObject source, string propertyName, T propertyValue)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            var property = source.Properties.Match(propertyName, PSMemberTypes.Properties).FirstOrDefault();
            if (null != property)
            {
                if (property.Value is T)
                {
                    property.Value = propertyValue;
                }
                else if (null != property.Value)
                {
                    property.Value = LanguagePrimitives.ConvertTo(propertyValue, property.Value.GetType(), CultureInfo.InvariantCulture);
                }
                else
                {
                    // Best effort may throw.
                    property.Value = propertyValue;
                }
            }
            else
            {
                property = new PSNoteProperty(propertyName, propertyValue);
                source.Properties.Add(property);
            }
        }
    }
}
