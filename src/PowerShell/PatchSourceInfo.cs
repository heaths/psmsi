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
    /// Source information for a patch.
    /// </summary>
    public class PatchSourceInfo : SourceInfo
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SourceInfo"/> class.
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
