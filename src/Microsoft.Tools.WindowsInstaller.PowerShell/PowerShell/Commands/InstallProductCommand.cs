// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;
using System.Globalization;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Install-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Install, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(ProductInstallation))]
    public sealed class InstallProductCommand : InstallProductCommandBase<InstallProductActionData>
    {
        /// <summary>
        /// Gets or sets the target directory for the initial product install.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Path, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, ValueFromPipelineByPropertyName = true)]
        [Alias("TargetDirectory")]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets whether installed product information should be returned.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Gets a generic description of the activity performed by this cmdlet.
        /// </summary>
        protected override string Activity
        {
            get { return Resources.Action_Install; }
        }

        /// <summary>
        /// Installs a product given the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An <see cref="InstallProductActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(InstallProductActionData data)
        {
            if (!string.IsNullOrEmpty(data.TargetDirectory))
            {
                data.CommandLine += string.Format(CultureInfo.InvariantCulture, @" TARGETDIR=""{0}""", data.TargetDirectory);
            }

            if (!string.IsNullOrEmpty(data.Path))
            {
                Installer.InstallProduct(data.Path, data.CommandLine);
            }
            else if (!string.IsNullOrEmpty(data.ProductCode))
            {
                Installer.ConfigureProduct(data.ProductCode, INSTALLLEVEL_DEFAULT, InstallState.Default, data.CommandLine);
            }

            if (this.PassThru)
            {
                var product = ProductInstallation.GetProducts(data.ProductCode, null, UserContexts.All).FirstOrDefault();
                if (null != product && product.IsInstalled)
                {
                    this.WriteObject(product.ToPSObject(this.SessionState.Path));
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="InstallProductActionData"/> with additional information.
        /// </summary>
        /// <param name="data">The <see cref="InstallProductActionData"/> to update.</param>
        protected override void UpdateAction(InstallProductActionData data)
        {
            base.UpdateAction(data);

            data.TargetDirectory = this.Destination;
        }
    }
}
