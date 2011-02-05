// Cmdlet to get or enumerator Windows Installer products.
//
// Created: Fri, 06 Apr 2007 14:59:12 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIRelatedProductInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIRelatedProductInfo")]
    public sealed class GetRelatedProductCommand : PSCmdlet
    {
        private List<string> allUpgradeCodes = new List<string>();

        /// <summary>
        /// Gets or sets the UpgradeCode to enumerate related products.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] UpgradeCode { get; set; }

        /// <summary>
        /// Collects the input UpgradeCodes for future processing.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Enumerate each of the UpgradeCodes to get information about the products.
            if (this.UpgradeCode != null && this.UpgradeCode.Length > 0)
            {
                this.allUpgradeCodes.AddRange(this.UpgradeCode);
            }
        }

        /// <summary>
        /// Processes the input UpgradeCodes and writes a list of products to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            this.allUpgradeCodes.ForEach((upgradeCode) =>
                {
                    this.WriteProducts(upgradeCode);
                });
        }

        /// <summary>
        /// Enumerates related products and writes them to the pipeline.
        /// </summary>
        /// <param name="upgradeCode"></param>
        private void WriteProducts(string upgradeCode)
        {
            foreach (ProductInstallation product in ProductInstallation.GetRelatedProducts(upgradeCode))
            {
                this.WriteProduct(product);
            }
        }

        /// <summary>
        /// Adds properties to the <see cref="ProductInstallation"/> object and writes it to the pipeline.
        /// </summary>
        /// <param name="product">The <see cref="ProductInstallation"/> to write to the pipeline.</param>
        private void WriteProduct(ProductInstallation product)
        {
            PSObject obj = PSObject.AsPSObject(product);

            // Add the local package as the PSPath.
            string path = PathConverter.ToPSPath(this.SessionState, product.LocalPackage);
            obj.Properties.Add(new PSNoteProperty("PSPath", path));

            this.WriteObject(obj);
        }
    }
}
