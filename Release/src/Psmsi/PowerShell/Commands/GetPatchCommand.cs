// Cmdlet to get or enumerator Windows Installer patches.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 22:08:18 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Windows.Installer.PowerShell;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIPatchInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIPatchInfo", DefaultParameterSetName = ParameterSet.Product)]
    public sealed class GetPatchCommand : PSCmdlet
    {
        private string[] productCodes;
        private string[] patchCodes;
        private PatchStates filter;
        private UserContexts context;
        private string userSid;

        /// <summary>
        /// Creates a new instance of the <see cref="GetPatchCommand"/> class.
        /// </summary>
        public GetPatchCommand()
        {
            this.productCodes = null;
            this.patchCodes = null;
            this.filter = PatchStates.Applied;
            this.context = UserContexts.Machine;
            this.userSid = null;
        }

        /// <summary>
        /// Gets or sets the ProductCodes for which patches are enumerated.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = ParameterSet.Patch, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] ProductCode
        {
            get { return this.productCodes; }
            set { this.productCodes = value; }
        }

        /// <summary>
        /// Gets or sets patch codes for which information is retrieved.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Patch, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] PatchCode
        {
            get { return this.patchCodes; }
            set { this.patchCodes = value; }
        }

        /// <summary>
        /// Gets or sets the patch states filter for enumeration.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public PatchStates Filter
        {
            get { return this.filter; }
            set { this.filter = value; }
        }

        /// <summary>
        /// Gets or sets the user context for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("Context", "InstallContext")] // Backward compatibility.
        public UserContexts UserContext
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// Gets or sets the user security identifier for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("User")]
        [Sid]
        public string UserSid
        {
            get { return this.userSid; }
            set { this.userSid = value; }
        }

        /// <summary>
        /// Gets or sets whether products for everyone should be enumerated.
        /// </summary>
        [Parameter]
        public SwitchParameter Everyone
        {
            get { return string.Compare(this.userSid, NativeMethods.World, true, CultureInfo.InvariantCulture) == 0; }
            set { this.userSid = value ? NativeMethods.World : null; }
        }
    }
}
