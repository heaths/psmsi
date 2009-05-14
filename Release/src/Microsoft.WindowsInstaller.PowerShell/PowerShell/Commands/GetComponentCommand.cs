// Cmdlet to get or enumerator Windows Installer products.
//
// Created: Sat, 07 Mar 2009 05:41:05 GMT
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
    /// The Get-WIComponentInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIComponentInfo", DefaultParameterSetName = ParameterSet.Component)]
    public sealed class GetComponentCommand : PSCmdlet
    {
        private string[] componentCodes;
        private string productCode;

        /// <summary>
        /// Gets or sets the component GUIDs to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), Parameter(ParameterSetName = ParameterSet.Component, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty, ValidateGuid]
        public string[] ComponentCode
        {
            get { return this.componentCodes; }
            set { this.componentCodes = value; }
        }

        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string ProductCode
        {
            get { return this.productCode; }
            set { this.productCode = value; }
        }

        /// <summary>
        /// Enumerates the selected components and write them to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == ParameterSet.Component)
            {
                if (this.componentCodes == null || this.componentCodes.Length == 0)
                {
                    // Enumerate all components.
                    foreach (ComponentInstallation component in ComponentInstallation.AllComponents)
                    {
                        this.WriteSharedComponent(component);
                    }
                }
                else
                {
                    // Enumerate all clients for a component.
                    foreach (string componentCode in this.componentCodes)
                    {
                        ComponentInstallation component = new ComponentInstallation(componentCode);
                        this.WriteSharedComponent(component);
                    }
                }
            }
            else if (this.ParameterSetName == ParameterSet.Product)
            {
                // Enumerate all components for the specified product.
                foreach (string componentCode in this.componentCodes)
                {
                    ComponentInstallation component = new ComponentInstallation(componentCode, this.productCode);
                    this.WriteComponent(component);
                }
            }
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
        /// Attachs additional properties to the component and writes it to the pipeline.
        /// </summary>
        /// <param name="component">The <see cref="ComponentInstallation"/> object to write to the pipeline.</param>
        private void WriteComponent(ComponentInstallation component)
        {
            PSObject obj = PSObject.AsPSObject(component);

            // Add the component key path as the PSPath.
            string path = PathConverter.FromKeyPathToPSPath(this.SessionState, component.Path);
            obj.Properties.Add(new PSNoteProperty("PSPath", path));

            // Must hide the ClientProducts property or exceptions will be thrown.
            obj.Properties.Add(new PSNoteProperty("ClientProducts", null));

            this.WriteObject(obj);
        }
    }
}
