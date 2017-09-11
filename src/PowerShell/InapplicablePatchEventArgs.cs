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
    /// Contains information about inapplicable patches.
    /// </summary>
    internal sealed class InapplicablePatchEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InapplicablePatchEventArgs"/> class.
        /// </summary>
        /// <param name="patch">The path to the patch or patch XML file that is not applicable.</param>
        /// <param name="product">The ProductCode for or path to the target product.</param>
        /// <param name="exception">Exception information about why the patch is not applicable.</param>
        internal InapplicablePatchEventArgs(string patch, string product, Exception exception)
        {
            this.Patch = patch;
            this.Product = product;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the path to the patch or patch XML file.
        /// </summary>
        internal string Patch { get; private set; }

        /// <summary>
        /// Gets the ProductCode for or path to the target product.
        /// </summary>
        internal string Product { get; private set; }

        /// <summary>
        /// Gets exception information about why the patch is not applicable.
        /// </summary>
        internal Exception Exception { get; private set; }
    }
}
