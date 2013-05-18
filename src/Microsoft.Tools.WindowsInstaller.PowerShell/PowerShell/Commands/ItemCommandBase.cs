// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for cmdlets which process items.
    /// </summary>
    public abstract class ItemCommandBase : PSCmdlet
    {
        private static readonly string[] All = new string[] { "*" };

        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Path, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("PSPath")]
        public string[] LiteralPath
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        /// <summary>
        /// Gets or sets whether the file objects are returned.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Processes the input paths and writes the file hashes to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            // If no path was provided, enumerate all child items.
            if (this.Path == null || this.Path.Length == 0)
            {
                this.Path = All;
            }

            // Get all the items.
            bool literal = this.ParameterSetName == ParameterSet.LiteralPath;
            Collection<PSObject> items = this.InvokeProvider.Item.Get(this.Path, true, literal);

            foreach (PSObject item in items)
            {
                PSPropertyInfo property = item.Properties["PSPath"];
                if (property != null && property.Value is string)
                {
                    // Get the provider path.
                    string path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(property.Value as string);
                    if (System.IO.File.Exists(path) || System.IO.Directory.Exists(path))
                    {
                        // Process the item.
                        this.ProcessItem(item);
                    }
                    else
                    {
                        string message = string.Format(Properties.Resources.Error_InvalidFile, path);
                        PSNotSupportedException ex = new PSNotSupportedException(message);

                        this.WriteError(ex.ErrorRecord);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the item in the inheriting class.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> to process.</param>
        protected abstract void ProcessItem(PSObject item);
    }
}
