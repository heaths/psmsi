// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The data for actions to repair a package or product.
    /// </summary>
    public class InstallProductActionData : InstallCommandActionData
    {
        /// <summary>
        /// Gets or sets the target directory where to install the product.
        /// </summary>
        public string TargetDirectory { get; set; }
    }
}
