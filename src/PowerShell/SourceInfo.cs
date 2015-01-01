// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
