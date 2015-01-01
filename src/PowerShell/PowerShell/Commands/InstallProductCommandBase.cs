// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for product-related commands.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InstallProductCommandBase<T> : InstallCommandBase<T> where T : InstallProductActionData, new()
    {
        /// <summary>
        /// Gets or sets the ProductCode to install.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Product, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ProductInstallation"/> to install.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Installation, Mandatory = true, ValueFromPipeline = true)]
        public ProductInstallation[] Product { get; set; }

        /// <summary>
        /// Queues products for installation.
        /// </summary>
        protected override void QueueActions()
        {
            if (ParameterSet.Product == this.ParameterSetName)
            {
                foreach (string productCode in this.ProductCode)
                {
                    var data = new T()
                    {
                        ProductCode = productCode,
                    };

                    data.ParseCommandLine(this.Properties);
                    this.UpdateAction(data);

                    this.Actions.Enqueue(data);
                }
            }
            else if (ParameterSet.Installation == this.ParameterSetName)
            {
                foreach (var product in this.Product)
                {
                    var data = new T()
                    {
                        ProductCode = product.ProductCode,
                    };

                    data.ParseCommandLine(this.Properties);
                    this.UpdateAction(data);

                    this.Actions.Enqueue(data);
                }
            }
            else
            {
                var paths = this.InvokeProvider.Item.Get(this.Path, true, ParameterSet.LiteralPath == this.ParameterSetName);
                foreach (var path in paths)
                {
                    var data = InstallCommandActionData.CreateActionData<T>(this.SessionState.Path, path);

                    data.SetProductCode();
                    data.ParseCommandLine(this.Properties);
                    this.UpdateAction(data);

                    this.Actions.Enqueue(data);
                }
            }
        }
    }
}
