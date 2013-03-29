// Converts username parameter values to SIDs.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics.CodeAnalysis;
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
        /// Creates a new instance of the <see cref="SidAttribute"/> class.
        /// </summary>
        public SidAttribute()
        {
        }

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
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Return false on any failure like a typical TryParse.")]
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
                catch (Exception)
                {
                }
            }

            // Coverstion failed.
            sddl = null;
            return false;
        }
    }
}
