// Other utility methods for the project.
//
// Created: Thu, 26 Mar 2009 23:37:19 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Utility methods for the project.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Determines if the <paramref name="value"/> matches any of the <paramref name="patterns"/>.
        /// </summary>
        /// <param name="value">The <see cref="String"/> to match.</param>
        /// <param name="patterns">The list of <see cref="WildcardPattern">WildcardPatterns</see> that contain the expressions to match.</param>
        /// <returns></returns>
        internal static bool MatchesAnyWildcardPattern(string value, IList<WildcardPattern> patterns)
        {
            if (patterns == null || patterns.Count == 0)
            {
                throw new ArgumentNullException("patterns");
            }

            // Iterate over the patterns (faster than enumeration).
            for (int i = 0; i < patterns.Count; i++)
            {
                if (patterns[i].IsMatch(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}