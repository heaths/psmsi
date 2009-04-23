// Base class for cmdlets which process items (files).
//
// Created: Sun, 01 Mar 2009 08:40:54 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for cmdlets which process items.
    /// </summary>
    public abstract class ItemCommandBase : PSCmdlet
    {
        private static readonly string[] All = new string[] { "*" };

        private string[] paths;
        private bool passThru;

        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Path, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path
        {
            get { return this.paths; }
            set { this.paths = value; }
        }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("PSPath")]
        public string[] LiteralPath
        {
            get { return this.paths; }
            set { this.paths = value; }
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

        /// <summary>
        /// Processes the input paths and writes the file hashes to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            // If no path was provided, enumerate a set of null.
            if (this.paths == null || this.paths.Length == 0)
            {
                this.paths = All;
            }

            // Get all the items.
            bool literal = this.ParameterSetName == ParameterSet.LiteralPath;
            Collection<PSObject> items = this.InvokeProvider.Item.Get(this.paths, true, literal);

            foreach (PSObject item in items)
            {
                PSPropertyInfo property = item.Properties["PSPath"];
                if (property != null && property.Value is string)
                {
                    // Get the provider path.
                    string path = PathConverter.ToProviderPath(this.SessionState, property.Value as string);
                    if (!string.IsNullOrEmpty(path))
                    {
                        // Process the item.
                        this.ProcessItem(item, path);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the item in the inheriting class.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> to process.</param>
        /// <param name="path">The provider path from the PSPath attached to the <paramref name="item"/>.</param>
        protected abstract void ProcessItem(PSObject item, string path);
    }
}
