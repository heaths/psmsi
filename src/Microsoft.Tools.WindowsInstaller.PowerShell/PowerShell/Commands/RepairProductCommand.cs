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
    [Cmdlet(VerbsDiagnostic.Repair, "MSIProduct", DefaultParameterSetName = ParameterSet.Product)]
    public sealed class RepairProductCommand : InstallCommandBase<RepairProductActionData>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RepairProductCommand"/> with the default <see cref="ReinstallMode"/>.
        /// </summary>
        public RepairProductCommand()
        {
            this.ReinstallMode = RepairProductActionData.Default;
        }

        /// <summary>
        /// Gets or sets the ProductCode to repair.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ReinstallModes"/> to use for repairing the product.
        /// </summary>
        /// <value>The default value is <see cref="RepairProductActionData.Default"/>.</value>
        [Parameter(ValueFromPipelineByPropertyName = true)]
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
        /// <param name="data">An <see cref="RepairProductActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(RepairProductActionData data)
        {
            Installer.ReinstallProduct(data.ProductCode, data.ReinstallMode);
        }

        /// <summary>
        /// Sets the ProductCode and <see cref="ReinstallModes"/> and queues the <see cref="RepairProductActionData"/>.
        /// </summary>
        protected override void QueueAction()
        {
            if (ParameterSet.Product == this.ParameterSetName)
            {
                foreach (string productCode in this.ProductCode)
                {
                    var data = new RepairProductActionData()
                    {
                        ProductCode = productCode,
                        ReinstallMode = this.ReinstallMode,
                    };

                    data.ParseCommandLine(this.Properties);

                    this.Actions.Enqueue(data);
                }
            }
            else
            {
                var paths = this.InvokeProvider.Item.Get(this.Path, true, ParameterSet.LiteralPath == this.ParameterSetName);
                foreach (var path in paths)
                {
                    var data = InstallPackageActionData.CreateActionData<RepairProductActionData>(this.SessionState.Path, path);

                    data.SetProductCode();
                    data.ParseCommandLine(this.Properties);

                    this.Actions.Enqueue(data);
                }
            }
        }
    }
}
