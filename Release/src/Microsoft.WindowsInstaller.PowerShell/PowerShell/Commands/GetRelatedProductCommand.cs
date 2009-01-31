// Cmdlet to get or enumerator Windows Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Fri, 06 Apr 2007 14:59:12 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Management.Automation;
using System.Text;
using Microsoft.WindowsInstaller;
using Microsoft.WindowsInstaller.PowerShell;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIRelatedProductInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIRelatedProductInfo")]
    public sealed class GetRelatedProductCommand : PSCmdlet
    {
        private string[] upgradeCodes;

        /// <summary>
        /// Gets or sets the UpgradeCode to enumerate related products.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] UpgradeCode
        {
            get { return this.upgradeCodes; }
            set { this.upgradeCodes = value; }
        }
    }
}
