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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using Microsoft.Tools.WindowsInstaller.Properties;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for cmdlets that process Windows Installer database packages.
    /// </summary>
    public abstract class PackageCommandBase : ItemCommandBase
    {
        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        /// <remarks>
        /// Assumes that all derivative classes will require another parameter in first position and that the Path parameter be specified.
        /// </remarks>
        [Parameter(ParameterSetName = ParameterSet.Path, Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        /// <summary>
        /// Gets or sets patch packages to apply before validation.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        public virtual string[] Patch { get; set; }

        /// <summary>
        /// Gets or sets transforms to apply before validation.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        public virtual string[] Transform { get; set; }

        /// <summary>
        /// Applies any applicable transforms from <see cref="Patch"/> and <see cref="Transform"/> to the given package.
        /// </summary>
        /// <param name="db">The <see cref="InstallPackage"/> database to which applicable transforms are applied.</param>
        protected void ApplyTransforms(InstallPackage db)
        {
            // Apply transforms first since they likely apply to the unpatched product.
            if (0 < this.Transform.Count())
            {
                this.Transform = this.ResolveFiles(this.Transform).ToArray();

                foreach (string path in this.Transform)
                {
                    try
                    {
                        db.ApplyTransform(path, PatchApplicator.IgnoreErrors);
                        db.ApplyTransform(path, PatchApplicator.IgnoreErrors | TransformErrors.ViewTransform);
                    }
                    catch (InstallerException ex)
                    {
                        using (var pse = new PSInstallerException(ex))
                        {
                            if (null != pse.ErrorRecord)
                            {
                                base.WriteError(pse.ErrorRecord);
                            }
                        }
                    }
                }

                db.Commit();
            }

            // Apply applicable patch transforms.
            if (0 < this.Patch.Count())
            {
                this.Patch = this.ResolveFiles(this.Patch).ToArray();

                var applicator = new PatchApplicator(db);
                foreach (string path in this.Patch)
                {
                    applicator.Add(path);
                }

                applicator.InapplicablePatch += (source, args) =>
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_InapplicablePatch, args.Patch, args.Product);
                    base.WriteVerbose(message);
                };

                // The applicator will commit the changes.
                applicator.Apply();
            }
        }

        /// <summary>
        /// Opens a product or patch database.
        /// </summary>
        /// <param name="path">The path to the database to open.</param>
        /// <returns>A <see cref="Database"/> object that must be disposed, or null if not a product or patch database.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will dispose later")]
        protected Database OpenDatabase(string path)
        {
            var type = FileInfo.GetFileTypeInternal(path);
            if (FileType.Package == type)
            {
                var db = new InstallPackage(path, DatabaseOpenMode.ReadOnly);
                this.ApplyTransforms(db);

                return db;
            }
            else if (FileType.Patch == type)
            {
                return new PatchPackage(path);
            }
            else
            {
                var message = string.Format(Resources.Error_InvalidStorage, path);
                var ex = new PSNotSupportedException(message);
                if (null != ex.ErrorRecord)
                {
                    base.WriteError(ex.ErrorRecord);
                }

                return null;
            }
        }

        /// <summary>
        /// Resolves all <pararef name="paths"/> to file system provider-specific paths.
        /// </summary>
        /// <param name="paths">The paths to resolve.</param>
        /// <returns>File system provider paths.</returns>
        protected IEnumerable<string> ResolveFiles(IEnumerable<string> paths)
        {
            ProviderInfo provider;
            foreach (string path in paths)
            {
                foreach (string resolvedPath in this.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider))
                {
                    yield return resolvedPath;
                }
            }
        }
    }
}
