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

using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Equality comparer for a <see cref="char"/>.
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
        /// <param name="x">The first <see cref="char"/> to compare.</param>
        /// <param name="y">The second <see cref="char"/> to compare.</param>
        /// <returns>True if <paramref name="x"/> equals <paramref name="y"/>.</returns>
        public bool Equals(char x, char y)
        {
            if (this.ignoreCase)
            {
                x = char.ToLowerInvariant(x);
                y = char.ToLowerInvariant(y);
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Gets the hash code of the given <see cref="char"/>.
        /// </summary>
        /// <param name="obj">The <see cref="char"/> from which to get the hash code.</param>
        /// <returns>The hash code of the given <see cref="char"/>.</returns>
        public int GetHashCode(char obj)
        {
            if (this.ignoreCase)
            {
                obj = char.ToLowerInvariant(obj);
            }

            return obj.GetHashCode();
        }
    }
}
