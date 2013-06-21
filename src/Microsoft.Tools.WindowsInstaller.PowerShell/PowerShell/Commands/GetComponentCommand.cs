// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

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
        [ValidateNotNullOrEmpty, ValidateGuid]
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

            // Add the component key path as the PSPath.
            string path = this.SessionState.Path.GetUnresolvedPSPathFromKeyPath(component.Path);
            obj.Properties.Add(new PSNoteProperty("PSPath", path));

            // Must hide the ClientProducts property or exceptions will be thrown.
            obj.Properties.Add(new PSNoteProperty("ClientProducts", null));

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
