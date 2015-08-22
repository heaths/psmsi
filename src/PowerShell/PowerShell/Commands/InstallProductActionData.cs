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
