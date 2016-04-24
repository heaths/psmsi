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

using System.Text.RegularExpressions;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains methods for validating input.
    /// </summary>
    internal static class Validate
    {
        // Define and compile the regular expression to validate GUIDs in a format Windows Installer understands.
        private static readonly Regex re = new Regex(
            @"\{[A-F0-9]{8}-([A-F0-9]{4}-){3}[A-F0-9]{12}\}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Returns whether or not the input <paramref name="value"/> string is a valid GUID.
        /// </summary>
        /// <param name="value">The <see cref="string"/> validate.</param>
        /// <returns>True if the input <paramref name="value"/> string is a valid GUID.</returns>
        internal static bool IsGuid(string value)
        {
            // Validate simple checks before performing a more exhaustive regex match.
            return null != value && 38 == value.Length && re.IsMatch(value);
        }
    }
}
