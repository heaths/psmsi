// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;

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

        /// <summary>
        /// Opens the package read-only and sets the <see cref="InstallCommandActionData.ProductCode"/> property.
        /// </summary>
        public void SetProductCode()
        {
            using (var db = new InstallPackage(this.Path, DatabaseOpenMode.ReadOnly))
            {
                if (db.Tables.Contains("Property"))
                {
                    this.ProductCode = db.Property["ProductCode"];
                }
                else
                {
                    this.ProductCode = null;
                }
            }
        }
    }
}
