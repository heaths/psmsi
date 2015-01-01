// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller.Package;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSISummaryInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSISummaryInfo", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(SummaryInfo), typeof(TransformInfo))]
    public sealed class GetSummaryInfoCommand : ItemCommandBase
    {
        /// <summary>
        /// Gets or sets whether transforms within a patch should be enumerated.
        /// </summary>
        [Parameter, Alias("Transforms")]
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
