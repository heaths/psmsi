// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.IO;
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
        [Parameter(ParameterSetName = ParameterSet.Path, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("PSPath"), ValidateNotNullOrEmpty]
        public string[] LiteralPath
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        /// <summary>
        /// Processes the input paths and writes the file hashes to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            // If no path was provided, enumerate all child items.
            if (null == this.Path || 0 == this.Path.Length)
            {
                this.Path = All;
            }

            // Get all the items.
            var items = this.InvokeProvider.Item.Get(this.Path, true, this.ParameterSetName == ParameterSet.LiteralPath);
            foreach (var item in items)
            {
                // Get the provider path.
                string path = item.GetPropertyValue<string>("PSPath");
                path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

                if (File.Exists(path) || Directory.Exists(path))
                {
                    this.ProcessItem(item);
                }
                else
                {
                    string message = string.Format(Properties.Resources.Error_InvalidFile, path);
                    var ex = new NotSupportedException(message);
                    var error = new ErrorRecord(ex, "UnsupportedItemType", ErrorCategory.InvalidType, path);

                    this.WriteError(error);
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
