// Cmdlet to get the storage class type for a file, optionally passing
// the PSObject back through the pipeline with a new NoteProperty.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Sat, 12 Jan 2008 17:09:56 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.WindowsInstaller;
using Microsoft.WindowsInstaller.PowerShell;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIFileType cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIFileType", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class GetFileTypeCommand : PSCmdlet
    {
        private string[] path;
        private bool literal;
        private bool passThru;

        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Path, Position = 0,
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path
        {
            get { return this.path; }
            set
            {
                this.literal = false;
                this.path = value;
            }
        }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, Position = 0, Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("PSPath")]
        public string[] LiteralPath
        {
            get { return this.path; }
            set
            {
                this.literal = true;
                this.path = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the file objects are returned.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru
        {
            get { return this.passThru; }
            set { this.passThru = value; }
        }
    }
}
