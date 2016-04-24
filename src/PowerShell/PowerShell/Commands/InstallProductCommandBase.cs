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

using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for product-related commands.
    /// </summary>
    /// <typeparam name="T">A derivative of <see cref="InstallCommandActionData"/>.</typeparam>
    public abstract class InstallProductCommandBase<T> : InstallCommandBase<T>
        where T : InstallProductActionData, new()
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
