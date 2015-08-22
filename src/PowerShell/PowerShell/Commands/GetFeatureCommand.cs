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
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-WIFeatureInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIFeatureInfo", DefaultParameterSetName = ParameterSet.Product)]
    [OutputType(typeof(FeatureInstallation))]
    public sealed class GetFeatureCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the <see cref="ProductInstallation"/> for which features are enumerated.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public ProductInstallation[] Product { get; set; }

        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Feature, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the feature names to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Feature, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("Name")]
        public string[] FeatureName { get; set; }

        /// <summary>
        /// Enumerates the selected features and write them to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == ParameterSet.Product)
            {
                // Enumerate the features of each product.
                foreach (ProductInstallation product in this.Product)
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
                foreach (string featureName in this.FeatureName)
                {
                    FeatureInstallation feature = new FeatureInstallation(featureName, this.ProductCode);
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
