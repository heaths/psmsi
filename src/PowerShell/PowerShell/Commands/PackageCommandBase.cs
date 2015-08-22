﻿// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using Microsoft.Tools.WindowsInstaller.Properties;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
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