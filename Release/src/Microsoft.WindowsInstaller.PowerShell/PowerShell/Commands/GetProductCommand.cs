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
using System.Globalization;
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
        private static readonly string[] Empty = new string[] { null };

        private string[] productCodes;
        private string[] names;
        private UserContexts context;
        private string userSid;

        /// <summary>
        /// Creates a new instance of the <see cref="GetProductCommand"/> class.
        /// </summary>
        public GetProductCommand()
        {
            this.names = null;
            this.productCodes = null;
            this.context = UserContexts.Machine;
            this.userSid = null;
        }
        
        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode
        {
            get { return this.productCodes; }
            set { this.productCodes = value; }
        }

        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Name, Mandatory = true)]
        public string[] Name
        {
            get { return this.names; }
            set { this.names = value; }
        }

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
        public string UserSid
        {
            get { return this.userSid; }
            set { this.userSid = value; }
        }

        /// <summary>
        /// Gets or sets whether products for everyone should be enumerated.
        /// </summary>
        [Parameter]
        public SwitchParameter Everyone
        {
            get { return string.Compare(this.userSid, NativeMethods.World, true, CultureInfo.InvariantCulture) == 0; }
            set { this.userSid = value ? NativeMethods.World : null; }
        }

        /// <summary>
        /// Processes the input ProductCodes and writes a product to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Enumerate products by ProductCode or all products.
            if (this.ParameterSetName == ParameterSet.Product)
            {
                // Enumerate a set of null if no input was provided.
                if (this.productCodes == null || this.productCodes.Length == 0)
                {
                    this.productCodes = Empty;
                }

                // Return each product instance.
                foreach (string productCode in this.productCodes)
                {
                    this.WriteProducts(productCode);
                }
            }
        }

        /// <summary>
        /// Processes the input Names and writes a unique list of products to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            // Enumerate all products in context and match the names using regex.
            if (this.ParameterSetName == ParameterSet.Name)
            {
                // Create a list of compiled patterns.
                List<WildcardPattern> patterns = new List<WildcardPattern>(this.names.Length);
                foreach (string name in this.names)
                {
                    patterns.Add(new WildcardPattern(name, WildcardOptions.Compiled | WildcardOptions.IgnoreCase));
                }

                // Enumerate all products in the context and attempt a match against each pattern.
                foreach (ProductInstallation product in ProductInstallation.GetProducts(null, this.userSid, this.context))
                {
                    string productName = product.ProductName;
                    if (!string.IsNullOrEmpty(productName) && Utilities.MatchesAnyWildcardPattern(productName, patterns))
                    {
                        this.WriteProduct(product);
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates products for the given ProductCode and writes them to the pipeline.
        /// </summary>
        /// <param name="productCode">The ProductCode of products to enumerate.</param>
        private void WriteProducts(string productCode)
        {
            foreach (ProductInstallation product in ProductInstallation.GetProducts(productCode, this.userSid, this.context))
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
