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
    /// The Uninstall-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Uninstall, "MSIProduct", DefaultParameterSetName = ParameterSet.Product)]
    public sealed class UninstallProductCommand : InstallCommandBase<InstallProductActionData>
    {
        /// <summary>
        /// Gets or sets the ProductCode to uninstall.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets a generic description of the activity performed by this cmdlet.
        /// </summary>
        protected override string Activity
        {
            get { return Resources.Action_Uninstall; }
        }

        /// <summary>
        /// Uninstalls a product given the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An <see cref="InstallProductActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(InstallProductActionData data)
        {
            const int INSTALLLEVEL_DEFAULT = 0;

            Installer.ConfigureProduct(data.ProductCode, INSTALLLEVEL_DEFAULT, InstallState.Absent, data.CommandLine);
        }

        /// <summary>
        /// Sets the package path or ProductCode and queues the <see cref="InstallProductActionData"/>.
        /// </summary>
        protected override void QueueAction()
        {
            if (ParameterSet.Product == this.ParameterSetName)
            {
                foreach (string productCode in this.ProductCode)
                {
                    var data = new InstallProductActionData()
                    {
                        ProductCode = productCode,
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
