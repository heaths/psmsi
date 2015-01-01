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
    /// The Uninstall-MSIPatch cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Uninstall, "MSIPatch", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class UninstallPatchCommand : InstallPatchCommandBase<InstallPatchActionData>
    {
        /// <summary>
        /// Gets the <see cref="RestorePointType"/> of the current operation.
        /// </summary>
        internal override RestorePointType Operation
        {
            get { return RestorePointType.ApplicationUninstall; }
        }

        /// <summary>
        /// Installs a patch given the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An <see cref="InstallProductActionData"/> with information about the package to install.</param>
        protected override void ExecuteAction(InstallPatchActionData data)
        {
            Installer.RemovePatches(data.Patches, data.ProductCode, data.CommandLine);
        }
    }
}
