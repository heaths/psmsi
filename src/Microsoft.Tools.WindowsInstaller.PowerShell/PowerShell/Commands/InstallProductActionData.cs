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
    /// The data for actions to install a package or product.
    /// </summary>
    public class InstallProductActionData : InstallPackageActionData
    {
        /// <summary>
        /// Gets or sets the ProductCode for which the action is performed.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Opens the package read-only and sets the <see cref="ProductCode"/> property.
        /// </summary>
        public void SetProductCode()
        {
            using (var db = new Database(this.Path, DatabaseOpenMode.ReadOnly))
            {
                using (var msi = Installer.OpenPackage(db, false))
                {
                    this.ProductCode = msi.GetProductProperty("ProductCode");
                }
            }
        }
    }
}
