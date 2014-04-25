// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using Microsoft.Tools.WindowsInstaller.Properties;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for cmdlets that process Windows Installer database packages.
    /// </summary>
    public abstract class PackageCommandBase : ItemCommandBase
    {
        /// <summary>
        /// Gets or sets patch packages to apply before validation.
        /// </summary>
        [Parameter, ValidateNotNullOrEmpty]
        public virtual string[] Patch { get; set; }

        /// <summary>
        /// Gets or sets transforms to apply before validation.
        /// </summary>
        [Parameter, ValidateNotNullOrEmpty]
        public virtual string[] Transform { get; set; }

        /// <summary>
        /// Applies any applicable transforms from <see cref="Patch"/> and <see cref="Transform"/> to the given package.
        /// </summary>
        /// <param name="db">The <see cref="InstallPackage"/> database to which applicable transforms are applied.</param>
        protected void ApplyTransforms(InstallPackage db)
        {
            // Apply transforms first since they likely apply to the unpatched product.
            if (null != this.Transform)
            {
                foreach (string path in this.ResolveFiles(this.Transform))
                {
                    try
                    {
                        db.ApplyTransform(path, PatchApplicator.IgnoreErrors);
                        db.ApplyTransform(path, PatchApplicator.IgnoreErrors | TransformErrors.ViewTransform);
                    }
                    catch (InstallerException ex)
                    {
                        using (var psex = new PSInstallerException(ex))
                        {
                            if (null != psex.ErrorRecord)
                            {
                                this.WriteError(psex.ErrorRecord);
                            }
                        }
                    }
                }

                db.Commit();
            }

            // Apply applicable patch transforms.
            if (null != this.Patch)
            {
                var applicator = new PatchApplicator(db);
                foreach (string path in this.ResolveFiles(this.Patch))
                {
                    applicator.Add(path);
                }

                applicator.InapplicablePatch += (source, args) =>
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_InapplicablePatch, args.Patch, args.Product);
                    this.WriteVerbose(message);
                };

                // The applicator will commit the changes.
                applicator.Apply();
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
