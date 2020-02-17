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

using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller.Package;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSISummaryInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSISummaryInfo", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(SummaryInfo), typeof(TransformInfo))]
    public sealed class GetSummaryInfoCommand : ItemCommandBase
    {
        /// <inheritdoc/>
        [Parameter(ParameterSetName = ParameterSet.Path, Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public override string[] Path { get; set; }

        /// <summary>
        /// Gets or sets whether transforms within a patch should be enumerated.
        /// </summary>
        [Parameter]
        [Alias("Transforms")]
        public SwitchParameter IncludeTransforms { get; set; }

        /// <summary>
        /// Writes a <see cref="SummaryInfo"/> object for each supported file type to the pipeline.
        /// </summary>
        /// <param name="item">A <see cref="PSObject"/> representing the file system object to process.</param>
        protected override void ProcessItem(PSObject item)
        {
            var path = item.GetPropertyValue<string>("PSPath");
            var providerPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            // Make sure the file exists and is a patch.
            var type = FileInfo.GetFileTypeInternal(providerPath);
            if (FileType.Package == type || FileType.Patch == type || FileType.Transform == type)
            {
                using (var info = new Deployment.WindowsInstaller.SummaryInfo(providerPath, false))
                {
                    var copy = new SummaryInfo(info);
                    var obj = PSObject.AsPSObject(copy);

                    // Add the class type as the first type name.
                    var name = typeof(SummaryInfo).FullName + "#" + type;
                    obj.TypeNames.Insert(0, name);

                    // Attach the original PSPath and write to the pipeline.
                    obj.SetPropertyValue("PSPath", path);
                    this.WriteObject(obj);
                }
            }

            // Enumerate transforms in the patch.
            if (FileType.Patch == type && this.IncludeTransforms)
            {
                using (var patch = new PatchPackage(providerPath))
                {
                    foreach (var transform in patch.GetTransformsInfo(true))
                    {
                        var obj = PSObject.AsPSObject(transform);

                        // Attach the original patch path and write to the pipeline.
                        obj.SetPropertyValue("Patch", providerPath);
                        this.WriteObject(obj);
                    }
                }
            }
        }
    }
}
