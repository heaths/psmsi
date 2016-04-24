// The MIT License (MIT)
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

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIPatchSequence cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIPatchSequence", DefaultParameterSetName = "Path,PackagePath")]
    [OutputType(typeof(PatchSequence))]
    public sealed class GetPatchSequenceCommand : ItemCommandBase
    {
        private PatchSequencer sequencer = new PatchSequencer();

        /// <summary>
        /// Gets or sets the paths to patch packages to sequence.
        /// </summary>
        [Parameter(ParameterSetName = "Path,PackagePath", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Path,ProductCode", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public override string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal paths to patch packages to sequence.
        /// </summary>
        [Parameter(ParameterSetName = "LiteralPath,PackagePath", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "LiteralPath,ProductCode", Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("PSPath")]
        public override string[] LiteralPath
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        /// <summary>
        /// Gets or sets the paths to packages for which patches are sequenced.
        /// </summary>
        [Parameter(ParameterSetName = "Path,PackagePath", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "LiteralPath,PackagePath", Mandatory = true, Position = 1)]
        public string[] PackagePath { get; set; }

        /// <summary>
        /// Gets or sets the ProductCodes for which patches are sequenced.
        /// </summary>
        [Parameter(ParameterSetName = "Path,ProductCode", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "LiteralPath,ProductCode", Mandatory = true, Position = 1)]
        [ValidateGuid]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the user context for the ProductCodes for which patches are sequenced.
        /// </summary>
        [Parameter(ParameterSetName = "Path,ProductCode")]
        [Parameter(ParameterSetName = "LiteralPath,ProductCode")]
        [Alias("Context", "InstallContext")]
        public UserContexts UserContext { get; set; }

        /// <summary>
        /// Gets or sets the user security identifier for the ProductCodes for which patches are sequenced.
        /// </summary>
        [Parameter(ParameterSetName = "Path,ProductCode")]
        [Parameter(ParameterSetName = "LiteralPath,ProductCode")]
        [Alias("User")]
        [Sid]
        public string UserSid { get; set; }

        private bool IsProductCode
        {
            get
            {
                return null != this.ParameterSetName
                    && 0 <= this.ParameterSetName.IndexOf("ProductCode", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Adds each patch enumerated to the <see cref="PatchSequencer"/> to sequence for each product in <see cref="EndProcessing"/>.
        /// </summary>
        /// <param name="item">A <see cref="PSObject"/> representing the path to a patch package.</param>
        protected override void ProcessItem(PSObject item)
        {
            string path = item.GetPropertyValue<string>("PSPath");
            path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            this.sequencer.Add(path);
        }

        /// <summary>
        /// Sequences and returns the ordered list of <see cref="PatchSequence"/> objects for all applicable patches.
        /// </summary>
        protected override void EndProcessing()
        {
            IEnumerable<PatchSequence> patches = null;
            if (this.IsProductCode)
            {
                // If not specified, check all contexts for the ProductCode.
                if (UserContexts.None == this.UserContext)
                {
                    this.UserContext = UserContexts.All;
                }

                foreach (string productCode in this.ProductCode)
                {
                    var product = ProductInstallation.GetProducts(productCode, this.UserSid, this.UserContext).FirstOrDefault();
                    if (null != product)
                    {
                        patches = this.sequencer.GetApplicablePatches(productCode, product.UserSid, product.Context);
                        this.WritePatchSequence(patches);
                    }
                }
            }
            else
            {
                ProviderInfo provider;
                foreach (string packagePath in this.PackagePath)
                {
                    var paths = this.SessionState.Path.GetResolvedProviderPathFromPSPath(packagePath, out provider);
                    foreach (string path in paths)
                    {
                        patches = this.sequencer.GetApplicablePatches(path);
                        this.WritePatchSequence(patches);
                    }
                }
            }
        }

        private void WritePatchSequence(IEnumerable<PatchSequence> patches)
        {
            if (null != patches)
            {
                foreach (var patch in patches)
                {
                    string path = this.SessionState.Path.GetUnresolvedPSPathFromProviderPath(patch.Patch);

                    var obj = PSObject.AsPSObject(patch);
                    obj.SetPropertyValue<string>("PSPath", path);

                    this.WriteObject(obj);
                }
            }
        }
    }
}
