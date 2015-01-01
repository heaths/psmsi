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
