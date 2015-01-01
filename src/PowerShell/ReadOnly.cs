// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Conveys whether the package should be opened as read-only.
    /// </summary>
    public enum ReadOnly
    {
        /// <summary>
        /// No restriction.
        /// </summary>
        Unrestricted,

        /// <summary>
        /// Read-only recommended.
        /// </summary>
        Recommended = 2,

        /// <summary>
        /// Read-only enforced.
        /// </summary>
        Enforced = 4,
    }
}
