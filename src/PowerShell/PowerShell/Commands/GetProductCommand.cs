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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIProductInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIProductInfo", DefaultParameterSetName = ParameterSet.Product)]
    [OutputType(typeof(ProductInstallation))]
    public sealed class GetProductCommand : PSCmdlet
    {
        private List<Parameters> allParameters = new List<Parameters>();
        private UserContexts context = UserContexts.All;

        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required by older PowerShell")]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the wildcard names to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required by older PowerShell")]
        [Parameter(ParameterSetName = ParameterSet.Name, Mandatory = true)]
        public string[] Name { get; set; }

        /// <summary>
        /// Gets or sets the user context for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("Context", "InstallContext")] // Backward compatibility.
        public UserContexts UserContext
        {
            get
            {
                return this.context;
            }

            set
            {
                if (value == UserContexts.None)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Error_InvalidContext, UserContexts.None);
                    throw new ArgumentException(message, "UserContext");
                }

                this.context = value;
            }
        }

        /// <summary>
        /// Gets or sets the user security identifier for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("User")]
        [Sid]
        public string UserSid { get; set; }

        /// <summary>
        /// Gets or sets whether products for everyone should be enumerated.
        /// </summary>
        [Parameter]
        public SwitchParameter Everyone
        {
            get { return string.Compare(this.UserSid, NativeMethods.World, StringComparison.OrdinalIgnoreCase) == 0; }
            set { this.UserSid = value ? NativeMethods.World : null; }
        }

        /// <summary>
        /// Collects the input ProductCodes for future processing.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.allParameters.Add(new Parameters
                {
                    ParameterSetName = this.ParameterSetName,
                    ProductCode = this.ProductCode,
                    Name = this.Name,
                    UserContext = this.UserContext,
                    UserSid = this.UserSid,
                });
        }

        /// <summary>
        /// Processes the input ProductCodes or Names and writes a list of products to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            // Works around re-entrancy issues.
            this.allParameters.ForEach((param) =>
                {
                    // Enumerate all products by ProductCodes.
                    if (param.ParameterSetName == ParameterSet.Product)
                    {
                        // Return each product instance.
                        if (param.ProductCode != null && param.ProductCode.Length > 0)
                        {
                            foreach (string productCode in param.ProductCode)
                            {
                                this.WriteProducts(productCode, param.UserSid, param.UserContext);
                            }
                        }
                        else
                        {
                            // Write all products.
                            this.WriteProducts(null, param.UserSid, param.UserContext);
                        }
                    }

                    // Enumerate all products in context and match the names using regex.
                    else if (param.ParameterSetName == ParameterSet.Name)
                    {
                        // Create a list of compiled patterns.
                        List<WildcardPattern> patterns = new List<WildcardPattern>(param.Name.Length);
                        foreach (string name in param.Name)
                        {
                            patterns.Add(new WildcardPattern(name, WildcardOptions.Compiled | WildcardOptions.IgnoreCase));
                        }

                        // Enumerate all products in the context and attempt a match against each pattern.
                        this.WriteProducts(null, param.UserSid, param.UserContext, patterns);
                    }
                });
        }

        /// <summary>
        /// Enumerates products for the given ProductCode and writes them to the pipeline.
        /// </summary>
        /// <param name="productCode">The ProductCode of products to enumerate.</param>
        /// <param name="userSid">The user's SID for products to enumerate.</param>
        /// <param name="context">The installation context for products to enumerate.</param>
        /// <param name="patterns">Optional list of <see cref="WildcardPattern"/> to match product names.</param>
        private void WriteProducts(string productCode, string userSid, UserContexts context, IList<WildcardPattern> patterns = null)
        {
            foreach (ProductInstallation product in ProductInstallation.GetProducts(productCode, userSid, context))
            {
                if (0 == patterns.Count() || product.ProductName.Match(patterns))
                {
                    this.WriteProduct(product);
                }
            }
        }

        /// <summary>
        /// Adds properties to the <see cref="ProductInstallation"/> object and writes it to the pipeline.
        /// </summary>
        /// <param name="product">The <see cref="ProductInstallation"/> to write to the pipeline.</param>
        private void WriteProduct(ProductInstallation product)
        {
            var obj = product.ToPSObject(this.SessionState.Path);
            this.WriteObject(obj);
        }

        /// <summary>
        /// Collects parameters for processing.
        /// </summary>
        private sealed class Parameters
        {
            /// <summary>
            /// Gets or sets the parameter set name.
            /// </summary>
            internal string ParameterSetName { get; set; }

            /// <summary>
            /// Gets or sets the ProductCodes.
            /// </summary>
            internal string[] ProductCode { get; set; }

            /// <summary>
            /// Gets or sets the wildcard names.
            /// </summary>
            internal string[] Name { get; set; }

            /// <summary>
            /// Gets or sets the installation context.
            /// </summary>
            internal UserContexts UserContext { get; set; }

            /// <summary>
            /// Gets or sets the user's SID.
            /// </summary>
            internal string UserSid { get; set; }
        }
    }
}
