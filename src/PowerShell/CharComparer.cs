// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Equality comparer for a <see cref="Char"/>.
    /// </summary>
    internal sealed class CharComparer : IEqualityComparer<char>
    {
        /// <summary>
        /// Gets a culture-invariant, case-insensitive comparer.
        /// </summary>
        internal static readonly CharComparer InvariantCultureIgnoreCase = new CharComparer(true);

        private readonly bool ignoreCase;

        private CharComparer(bool ignoreCase)
        {
            this.ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Gets whether <paramref name="x"/> and <paramref name="y"/> are equal.
        /// </summary>
        /// <param name="x">The first <see cref="Char"/> to compare.</param>
        /// <param name="y">The second <see cref="Char"/> to compare.</param>
        /// <returns>True if <paramref name="x"/> equals <paramref name="y"/>.</returns>
        public bool Equals(char x, char y)
        {
            if (this.ignoreCase)
            {
                x = Char.ToLowerInvariant(x);
                y = Char.ToLowerInvariant(y);
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Gets the hash code of the given <see cref="Char"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Char"/> from which to get the hash code.</param>
        /// <returns>The hash code of the given <see cref="Char"/>.</returns>
        public int GetHashCode(char obj)
        {
            if (this.ignoreCase)
            {
                obj = Char.ToLowerInvariant(obj);
            }

            return obj.GetHashCode();
        }
    }
}
