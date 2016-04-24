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

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// The results of an operation on a product or patch package.
    /// </summary>
    public sealed class Result
    {
        private bool rebootInitiated;
        private bool rebootRequired;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        internal Result()
        {
            this.rebootInitiated = false;
            this.rebootRequired = false;
        }

        /// <summary>
        /// Gets a value indicating whether a reboot has been initiated already.
        /// </summary>
        public bool RebootInitiated
        {
            get
            {
                return this.rebootInitiated;
            }

            internal set
            {
                this.rebootInitiated |= value;
                this.rebootRequired |= value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a reboot is required or has been initiated already.
        /// </summary>
        public bool RebootRequired
        {
            get { return this.rebootRequired; }
            internal set { this.rebootRequired |= value; }
        }

        /// <summary>
        /// Combines results while preserving their semantics.
        /// </summary>
        /// <param name="x">The first <see cref="Result"/> to combine.</param>
        /// <param name="y">The second <see cref="Result"/> to combine.</param>
        /// <returns>A combined <see cref="Result"/> from <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static Result operator |(Result x, Result y)
        {
            if (null == x)
            {
                throw new ArgumentNullException("x");
            }

            if (null == y)
            {
                throw new ArgumentNullException("y");
            }

            var result = new Result();

            result.rebootInitiated = x.rebootInitiated | y.rebootInitiated;
            result.rebootRequired = result.rebootInitiated | x.rebootRequired | y.rebootRequired;

            return result;
        }
    }
}
