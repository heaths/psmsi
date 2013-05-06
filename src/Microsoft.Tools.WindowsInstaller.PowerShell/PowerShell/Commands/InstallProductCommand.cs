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
    /// The Install-MSIProduct cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Install, "MSIProduct", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class InstallProductCommand : InstallCommandBase<InstallPackageActionData>
    {
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
        /// <param name="data">An <see cref="InstallPackageActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(InstallPackageActionData data)
        {
            Installer.InstallProduct(data.Path, data.CommandLine);
        }
    }
}
