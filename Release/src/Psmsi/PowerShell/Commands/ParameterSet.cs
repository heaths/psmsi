// Parameter set name constants.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Wed, 12 Mar 2008 20:59:23 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Helps avoid misspellings throughout the code.
    /// </summary>
    internal static class ParameterSet
    {
        /// <summary>
        /// Use for InputObject parameters that typically accept object[] arrays.
        /// </summary>
        internal const string InputObject = "InputObject";

        /// <summary>
        /// Use when input may be a ProductCode or a patch code.
        /// </summary>
        internal const string ProductOrPatchCode = "ProductOrPatchCode";

        /// <summary>
        /// Use when input is a ProductCode.
        /// </summary>
        internal const string ProductCode = "ProductCode";

        /// <summary>
        /// Use when input is a patch code.
        /// </summary>
        internal const string PatchCode = "PatchCode";

        /// <summary>
        /// Use when input is an UpgradeCode.
        /// </summary>
        internal const string UpgradeCode = "UpgradeCode";

        /// <summary>
        /// Use for wildcard paths.
        /// </summary>
        internal const string Path = "Path";

        /// <summary>
        /// Use for literal paths where wildcards are not supported.
        /// </summary>
        internal const string LiteralPath = "LiteralPath";
    }
}
