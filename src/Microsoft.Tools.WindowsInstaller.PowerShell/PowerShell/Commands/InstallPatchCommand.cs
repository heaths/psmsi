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
    /// The Install-MSIPatch cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Install, "MSIPatch", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(PatchInstallation))]
    public sealed class InstallPatchCommand : InstallPatchCommandBase<InstallPatchActionData>
    {
        /// <summary>
        /// Gets or sets whether the patched product information should be returned.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Installs a patch given the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An <see cref="InstallProductActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(InstallPatchActionData data)
        {
            Installer.ApplyMultiplePatches(data.Patches, data.ProductCode, data.CommandLine);

            if (this.PassThru)
            {
                var product = ProductInstallation.GetProducts(data.ProductCode, null, UserContexts.All).FirstOrDefault();
                if (null != product && product.IsInstalled)
                {
                    this.WriteObject(product.ToPSObject(this.SessionState.Path));
                }
            }
        }
    }
}
