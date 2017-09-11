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
    /// Source information for a patch.
    /// </summary>
    public class PatchSourceInfo : SourceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatchSourceInfo"/> class.
        /// </summary>
        /// <param name="productCode">The ProductCode to which the source is registered.</param>
        /// <param name="patchCode">The patch code to which the source is regisered.</param>
        /// <param name="userSid">The user SID of the product.</param>
        /// <param name="userContext">The user context of the product.</param>
        /// <param name="path">The registered source path.</param>
        /// <param name="order">The order in which the source path is tried.</param>
        internal PatchSourceInfo(string productCode, string patchCode, string userSid, UserContexts userContext, string path, int order)
            : base(productCode, userSid, userContext, path, order)
        {
            this.PatchCode = patchCode;
        }

        /// <summary>
        /// Gets the patch code to which the source is registered.
        /// </summary>
        public string PatchCode { get; private set; }
    }
}
