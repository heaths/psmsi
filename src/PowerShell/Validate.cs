// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Text.RegularExpressions;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains methods for validating input.
    /// </summary>
    internal static class Validate
    {
        // Define and compile the regular expression to validate GUIDs in a format Windows Installer understands.
        private static readonly Regex re = new Regex(@"\{[A-F0-9]{8}-([A-F0-9]{4}-){3}[A-F0-9]{12}\}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Returns whether or not the input <paramref name="value"/> string is a valid GUID.
        /// </summary>
        /// <param name="value">The <see cref="String"/> validate.</param>
        /// <returns>True if the input <paramref name="value"/> string is a valid GUID.</returns>
        internal static bool IsGuid(string value)
        {
            // Validate simple checks before performing a more exhaustive regex match.
            return null != value && 38 == value.Length && re.IsMatch(value);
        }
    }
}
