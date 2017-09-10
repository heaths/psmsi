﻿// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
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
        /// Gets or sets a value indicating whether the <see cref="Path"/> or <see cref="LiteralPath"/> must exist.
        /// </summary>
        protected virtual bool ShouldExist { get; set; }

        /// <summary>
        /// Adds the resolved path to the <see cref="SourceCommandBase.Parameters"/>.
        /// </summary>
        /// <param name="param">The <see cref="SourceCommandBase.Parameters"/> to update.</param>
        protected override void UpdateParameters(Parameters param)
        {
            IEnumerable<string> paths;
            if (this.ShouldExist)
            {
                paths = this.InvokeProvider.Item
                    .Get(this.Path, true, ParameterSet.LiteralPath == this.ParameterSetName)
                    .Select(item => item.GetPropertyValue<string>("PSPath"));
            }
            else
            {
                paths = this.Path;
            }

            foreach (var path in paths)
            {
                var providerPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
                param.Paths.Add(providerPath);
            }
        }
    }
}
