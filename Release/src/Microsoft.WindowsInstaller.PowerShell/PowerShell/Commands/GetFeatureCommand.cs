// Cmdlet to get or enumerator Windows Installer products.
//
// Created: Tue, 10 Mar 2009 06:14:57 GMT
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
    /// The Get-WIFeatureInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "WIFeatureInfo", DefaultParameterSetName = ParameterSet.Product)]
    public sealed class GetFeatureCommand : PSCmdlet
    {
        private ProductInstallation[] products;
        private string productCode;
        private string[] featureNames;

        /// <summary>
        /// Gets or sets the <see cref="ProductInstallation"/> for which features are enumerated.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(ParameterSetName = ParameterSet.Product, Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public ProductInstallation[] Product
        {
            get { return this.products; }
            set { this.products = value; }
        }

        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Feature, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string ProductCode
        {
            get { return this.productCode; }
            set { this.productCode = value; }
        }

        /// <summary>
        /// Gets or sets the feature names to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(ParameterSetName = ParameterSet.Feature, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("Name")]
        public string[] FeatureName
        {
            get { return this.featureNames; }
            set { this.featureNames = value; }
        }

        /// <summary>
        /// Enumerates the selected features and write them to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == ParameterSet.Product)
            {
                // Enumerate the features of each product.
                foreach (ProductInstallation product in this.products)
                {
                    foreach (FeatureInstallation feature in product.Features)
                    {
                        this.WriteFeature(feature);
                    }
                }
            }
            else if (this.ParameterSetName == ParameterSet.Feature)
            {
                // Enumerate all the features of a specific product.
                foreach (string featureName in this.featureNames)
                {
                    FeatureInstallation feature = new FeatureInstallation(featureName, this.productCode);
                    this.WriteFeature(feature);
                }
            }
        }

        /// <summary>
        /// Writes the feature to the pipeline.
        /// </summary>
        /// <param name="feature">The <see cref="FeatureInstallation"/> to write to the pipeline.</param>
        private void WriteFeature(FeatureInstallation feature)
        {
            PSObject obj = PSObject.AsPSObject(feature);
            this.WriteObject(obj);
        }
    }
}
