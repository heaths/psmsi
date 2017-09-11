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
using Microsoft.Tools.WindowsInstaller.Properties;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Repair-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Repair, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(ProductInstallation))]
    public sealed class RepairProductCommand : InstallProductCommandBase<RepairProductActionData>
    {
        private ReinstallModesConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepairProductCommand"/> class.
        /// </summary>
        public RepairProductCommand()
        {
            this.ReinstallMode = RepairProductActionData.Default;
            this.converter = new ReinstallModesConverter();
        }

        /// <summary>
        /// Gets or sets the <see cref="ReinstallModes"/> to use for repairing the product.
        /// </summary>
        /// <value>The default value is <see cref="RepairProductActionData.Default"/>.</value>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ReinstallMode]
        public ReinstallModes ReinstallMode { get; set; }

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
            get { return Resources.Action_Repair; }
        }

        /// <summary>
        /// Repairs a product given the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An <see cref="RepairProductActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(RepairProductActionData data)
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
        /// Updates the <see cref="RepairProductActionData"/> to include the current <see cref="ReinstallMode"/> flags.
        /// </summary>
        /// <param name="data">The <see cref="RepairProductActionData"/> to update.</param>
        protected override void UpdateAction(RepairProductActionData data)
        {
            base.UpdateAction(data);

            data.ReinstallMode = this.ReinstallMode;
        }
    }
}
