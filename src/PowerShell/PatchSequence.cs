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

using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Information about a sequence of applicable patches.
    /// </summary>
    public sealed class PatchSequence
    {
        /// <summary>
        /// Gets the path to the patch package.
        /// </summary>
        public string Patch { get; internal set; }

        /// <summary>
        /// Gets the sequence of the <see cref="Patch"/> as it applies to the <see cref="Product"/>.
        /// </summary>
        public int Sequence { get; internal set; }

        /// <summary>
        /// Gets the path to a product package or the ProductCode.
        /// </summary>
        public string Product { get; internal set; }

        /// <summary>
        /// Gets the user's SID to which the product is assigned.
        /// </summary>
        public string UserSid { get; internal set; }

        /// <summary>
        /// Gets the user context to which the product is assigned.
        /// </summary>
        public UserContexts UserContext { get; internal set; }

        /// <summary>
        /// Gets the ProductCode if <see cref="Product"/> is a GUID.
        /// </summary>
        public string ProductCode
        {
            get
            {
                if (Validate.IsGuid(this.Product))
                {
                    return this.Product;
                }

                return null;
            }
        }
    }
}
