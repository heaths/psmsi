// Cmdlet to get or enumerator Windows Installer products.
//
// Created: Fri, 06 Apr 2007 14:59:12 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
        private string[] upgradeCodes;

        /// <summary>
        /// Gets or sets the UpgradeCode to enumerate related products.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] UpgradeCode
        {
            get { return this.upgradeCodes; }
            set { this.upgradeCodes = value; }
        }

        /// <summary>
        /// Processes the input UpgradeCodes and writes a product to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Enumerate each of the UpgradeCodes to get information about the products.
            foreach (string upgradeCode in this.upgradeCodes)
            {
                this.WriteProducts(upgradeCode);
            }
        }

        /// <summary>
        /// Enumerates related products and writes them to the pipeline.
        /// </summary>
        /// <param name="upgradeCode"></param>
        private void WriteProducts(string upgradeCode)
        {
            foreach (ProductInstallation product in ProductInstallation.GetRelatedProducts(upgradeCode))
            {
                PSObject obj = PSObject.AsPSObject(product);

                // Add the local package as the PSPath.
                string path = PathConverter.ToPSPath(this.SessionState, product.LocalPackage);
                obj.Properties.Add(new PSNoteProperty("PSPath", path));

                this.WriteObject(obj);
            }
        }
    }
}
