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

using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-WIComponentInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIComponentInfo", DefaultParameterSetName = ParameterSet.Component)]
    [OutputType(typeof(ComponentInstallation))]
    public sealed class GetComponentCommand : PSCmdlet
    {
        private List<Parameters> allParameters = new List<Parameters>();

        /// <summary>
        /// Gets or sets the component GUIDs to enumerate.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Component, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("ComponentId")]
        [ValidateNotNullOrEmpty]
        [ValidateGuid]
        public string[] ComponentCode { get; set; }

        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string ProductCode { get; set; }

        /// <summary>
        /// Collects input ComponentCodes and ProductCodes for future processing.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Works around re-entrancy issues.
            this.allParameters.Add(new Parameters
                {
                    ParameterSetName = this.ParameterSetName,
                    ComponentCode = this.ComponentCode,
                    ProductCode = this.ProductCode,
                });
        }

        /// <summary>
        /// Enumerates the selected components and write them to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            this.allParameters.ForEach((param) =>
                {
                    if (param.ParameterSetName == ParameterSet.Component)
                    {
                        if (param.ComponentCode != null && param.ComponentCode.Length > 0)
                        {
                            // Enumerate all clients for a component.
                            foreach (string componentCode in param.ComponentCode)
                            {
                                ComponentInstallation component = new ComponentInstallation(componentCode);
                                this.WriteSharedComponent(component);
                            }
                        }
                        else
                        {
                            // Enumerate all components.
                            foreach (ComponentInstallation component in ComponentInstallation.AllComponents)
                            {
                                this.WriteSharedComponent(component);
                            }
                        }
                    }
                    else if (param.ParameterSetName == ParameterSet.Product)
                    {
                        // Enumerate all components for the specified product.
                        foreach (string componentCode in param.ComponentCode)
                        {
                            ComponentInstallation component = new ComponentInstallation(componentCode, param.ProductCode);
                            this.WriteComponent(component);
                        }
                    }
                });
        }

        /// <summary>
        /// Enumerates clients of the component and writes each client-specific component to the pipeline.
        /// </summary>
        /// <param name="component">The shared <see cref="ComponentInstallation"/> object to write to the pipeline.</param>
        private void WriteSharedComponent(ComponentInstallation component)
        {
            foreach (ProductInstallation client in component.ClientProducts)
            {
                ComponentInstallation shared = new ComponentInstallation(component.ComponentCode, client.ProductCode);
                this.WriteComponent(shared);
            }
        }

        /// <summary>
        /// Attaches additional properties to the component and writes it to the pipeline.
        /// </summary>
        /// <param name="component">The <see cref="ComponentInstallation"/> object to write to the pipeline.</param>
        private void WriteComponent(ComponentInstallation component)
        {
            PSObject obj = PSObject.AsPSObject(component);
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
            /// Gets or sets the component GUIDs.
            /// </summary>
            internal string[] ComponentCode { get; set; }

            /// <summary>
            /// Gets or sets the ProductCode.
            /// </summary>
            internal string ProductCode { get; set; }
        }
    }
}
