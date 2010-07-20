// Cmdlet to get or enumerator Windows Installer products.
//
// Created: Thu, 01 Feb 2007 06:54:27 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIProductInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIProductInfo", DefaultParameterSetName = ParameterSet.Product)]
    public sealed class GetProductCommand : PSCmdlet
    {
        private List<Parameters> allParameters = new List<Parameters>();
        private UserContexts context = UserContexts.Machine;

        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Sets the wildcard names to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Name, Mandatory = true)]
        public string[] Name { get; set; }

        /// <summary>
        /// Gets or sets the user context for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("Context", "InstallContext")] // Backward compatibility.
        public UserContexts UserContext
        {
            get { return this.context; }
            set
            {
                if (value == UserContexts.None)
                {
                    throw new ArgumentException(Properties.Resources.Error_InvalidContext);
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
                        foreach (ProductInstallation product in ProductInstallation.GetProducts(null, param.UserSid, param.UserContext))
                        {
                            string productName = product.ProductName;
                            if (Utility.MatchesAnyWildcardPattern(productName, patterns))
                            {
                                this.WriteProduct(product);
                            }
                        }
                    }
                });
        }

        /// <summary>
        /// Enumerates products for the given ProductCode and writes them to the pipeline.
        /// </summary>
        /// <param name="productCode">The ProductCode of products to enumerate.</param>
        /// <param name="userSid">The user's SID for products to enumerate.</param>
        /// <param name="context">The installation context for products to enumerate.</param>
        private void WriteProducts(string productCode, string userSid, UserContexts context)
        {
            foreach (ProductInstallation product in ProductInstallation.GetProducts(productCode, userSid, context))
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
