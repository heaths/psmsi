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
    /// Source information for a product.
    /// </summary>
    public class SourceInfo
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SourceInfo"/> class.
        /// </summary>
        /// <param name="productCode">The ProductCode to which the source is registered.</param>
        /// <param name="userSid">The user SID of the product.</param>
        /// <param name="userContext">The user context of the product.</param>
        /// <param name="path">The registered source path.</param>
        /// <param name="order">The order in which the source path is tried.</param>
        internal SourceInfo(string productCode, string userSid, UserContexts userContext, string path, int order)
        {
            this.ProductCode = productCode;
            this.UserSid = userSid;
            this.UserContext = userContext;
            this.Path = path;
            this.Order = order;
        }

        /// <summary>
        /// Gets the ProductCode to which the source is registered.
        /// </summary>
        public string ProductCode { get; private set; }

        /// <summary>
        /// Gets the user SID of the product.
        /// </summary>
        public string UserSid { get; private set; }

        /// <summary>
        /// Gets the user context of the product.
        /// </summary>
        public UserContexts UserContext { get; private set; }

        /// <summary>
        /// Gets the registered source path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the order in which the source path is tried.
        /// </summary>
        public int Order { get; private set; }
    }
}
