// Cmdlet to get or enumerator Windows Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 06:54:27 GMT
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
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.WindowsInstaller.PowerShell;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIProductInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIProductInfo", DefaultParameterSetName = ParameterSet.Product)]
    public sealed class GetProductCommand : PSCmdlet
    {
        private string[] productCode;
        private UserContexts context;
        private string userSid;

        /// <summary>
        /// Creates a new instance of the <see cref="GetProductCommand"/> class.
        /// </summary>
        public GetProductCommand()
        {
            this.productCode = null;
            this.context = UserContexts.Machine;
            this.userSid = null;
        }
        
        /// <summary>
        /// Gets or sets the ProductCodes to enumerate.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Product, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] ProductCode
        {
            get { return this.productCode; }
            set { this.productCode = value; }
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
