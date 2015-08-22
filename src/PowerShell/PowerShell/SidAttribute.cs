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
using System.Management.Automation;
using System.Security.Principal;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Transforms user account identifiers to security identifiers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SidAttribute : ArgumentTransformationAttribute
    {
        /// <summary>
        /// Transforms user account identifiers to security identifiers.
        /// </summary>
        /// <param name="engineIntrinsics">Provides access to the APIs for managing the transformation context.</param>
        /// <param name="inputData">The parameter argument that is to be transformed.</param>
        /// <returns>The transformed object.</returns>
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            // null may be allowed, so pass it through.
            if (inputData == null)
            {
                return null;
            }

            // Convert account information to SDDL format.
            string username;
            if (LanguagePrimitives.TryConvertTo<string>(inputData, out username))
            {
                string sddl;
                if (SidAttribute.TryParseUsername(username, out sddl))
                {
                    return sddl;
                }
            }

            // Fallback to returning the original input data to further transformations.
            return inputData;
        }

        /// <summary>
        /// Tries to parse the string as a username to get the SDDL format of a SID.
        /// </summary>
        /// <param name="username">The string to parse as a username.</param>
        /// <param name="sddl">The SDDL format of a SID to return.</param>
        /// <returns>Returns true if the string was parsed as a username and an SDDL was returned; otherwise, false.</returns>
        internal static bool TryParseUsername(string username, out string sddl)
        {
            if (username.IndexOf("\\", StringComparison.Ordinal) >= 0)
            {
                try
                {
                    NTAccount account = new NTAccount(username);
                    SecurityIdentifier sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));

                    sddl = sid.ToString();
                    return true;
                }
                catch
                {
                }
            }

            // Coverstion failed.
            sddl = null;
            return false;
        }
    }
}
