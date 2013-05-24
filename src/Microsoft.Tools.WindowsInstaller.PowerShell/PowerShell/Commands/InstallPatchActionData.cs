// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The data for actions to install patches.
    /// </summary>
    public class InstallPatchActionData : InstallCommandActionData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstallPatchActionData"/> class.
        /// </summary>
        public InstallPatchActionData()
        {
            this.Patches = new List<string>();
        }

        /// <summary>
        /// Gets the list of patch paths to apply.
        /// </summary>
        public List<string> Patches { get; private set; }

        /// <summary>
        /// Gets or sets the user security identifier for the product to which patches apply.
        /// </summary>
        public string UserSid { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserContexts"/> for the product to which patches apply.
        /// </summary>
        public UserContexts UserContext { get; set; }
    }
}
