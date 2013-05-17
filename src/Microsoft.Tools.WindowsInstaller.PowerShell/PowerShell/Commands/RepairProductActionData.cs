// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The data for actions to repair a package or product.
    /// </summary>
    public class RepairProductActionData : InstallCommandActionData
    {
        /// <summary>
        /// The default <see cref="ReinstallMode"/> (equivalent to "omus").
        /// </summary>
        public const ReinstallModes Default = ReinstallModes.FileOlderVersion | ReinstallModes.MachineData | ReinstallModes.UserData | ReinstallModes.Shortcut;

        /// <summary>
        /// Creates a new instance of the <see cref="RepairProductActionData"/> with the <see cref="Default"/> <see cref="ReinstallMode"/>.
        /// </summary>
        public RepairProductActionData()
        {
            this.ReinstallMode = RepairProductActionData.Default;
        }

        /// <summary>
        /// Gets or sets the <see cref="ReinstallModes"/> for the action.
        /// </summary>
        /// <value>The default value is <see cref="Default"/>.</value>
        public ReinstallModes ReinstallMode { get; set; }
    }
}
