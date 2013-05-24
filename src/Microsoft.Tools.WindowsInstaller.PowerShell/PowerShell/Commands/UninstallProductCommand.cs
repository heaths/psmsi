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
    [Cmdlet(VerbsLifecycle.Uninstall, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class UninstallProductCommand : InstallProductCommandBase<InstallProductActionData>
    {
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
            data.CommandLine += " REMOVE=ALL";

            if (!string.IsNullOrEmpty(data.Path))
            {
                Installer.InstallProduct(data.Path, data.CommandLine);
            }
            else if (!string.IsNullOrEmpty(data.ProductCode))
            {
                Installer.ConfigureProduct(data.ProductCode, INSTALLLEVEL_DEFAULT, InstallState.Default, data.CommandLine);
            }
        }
    }
}
