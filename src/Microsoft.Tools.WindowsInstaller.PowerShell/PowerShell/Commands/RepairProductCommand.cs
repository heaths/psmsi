// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Repair-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Repair, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class RepairProductCommand : InstallCommandBase<RepairCommandActionData>
    {
        private ReinstallModesConverter converter;

        /// <summary>
        /// Creates a new instance of the <see cref="RepairProductCommand"/> with the default <see cref="ReinstallMode"/>.
        /// </summary>
        public RepairProductCommand()
        {
            this.ReinstallMode = RepairCommandActionData.Default;
            this.converter = new ReinstallModesConverter();
        }

        /// <summary>
        /// Gets or sets the <see cref="ReinstallModes"/> to use for repairing the product.
        /// </summary>
        /// <value>The default value is <see cref="RepairCommandActionData.Default"/>.</value>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ReinstallMode]
        public ReinstallModes ReinstallMode { get; set; }

        /// <summary>
        /// Gets a generic description of the activity performed by this cmdlet.
        /// </summary>
        protected override string Activity
        {
            get { return Resources.Action_Repair; }
        }

        /// <summary>
        /// Repairs a product given the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An <see cref="RepairCommandActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(RepairCommandActionData data)
        {
            string mode = this.converter.ConvertToString(data.ReinstallMode);
            data.CommandLine += " REINSTALL=ALL REINSTALLMODE=" + mode;

            if (!string.IsNullOrEmpty(data.Path))
            {
                Installer.InstallProduct(data.Path, data.CommandLine);
            }
            else if (!string.IsNullOrEmpty(data.ProductCode))
            {
                Installer.ConfigureProduct(data.ProductCode, INSTALLLEVEL_DEFAULT, InstallState.Default, data.CommandLine);
            }
        }

        /// <summary>
        /// Updates the <see cref="RepairCommandActionData"/> to include the current <see cref="ReinstallMode"/> flags.
        /// </summary>
        /// <param name="data">The <see cref="RepairCommandActionData"/> to update.</param>
        protected override void UpdateAction(RepairCommandActionData data)
        {
            base.UpdateAction(data);

            data.ReinstallMode = this.ReinstallMode;
        }
    }
}
