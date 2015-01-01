// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Cmdlet to get the source list for a product or patch.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "MSISource")]
    public abstract class SourcePathCommandBase : SourceCommandBase
    {
        /// <summary>
        /// Gets or sets the path to add or remove from registered source.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Path, Mandatory = true, Position = 1, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal path to add or remove from registered source.
        /// </summary>
        [Alias("PSPath")]
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string[] LiteralPath
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        /// <summary>
        /// Gets or sets whether to write the remaining registered sources to the pipeline.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Adds the resolved path to the <see cref="SourceCommandBase.Parameters"/>.
        /// </summary>
        /// <param name="param">The <see cref="SourceCommandBase.Parameters"/> to update.</param>
        protected override void UpdateParameters(Parameters param)
        {
            var items = this.InvokeProvider.Item.Get(this.Path, true, ParameterSet.LiteralPath == this.ParameterSetName);

            foreach (var item in items)
            {
                var path = item.GetPropertyValue<string>("PSPath");
                path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

                param.Paths.Add(path);
            }
        }
    }
}
