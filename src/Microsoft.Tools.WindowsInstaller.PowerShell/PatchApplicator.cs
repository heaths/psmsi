// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Applies applicable patch transforms to a database.
    /// </summary>
    /// <remarks>
    /// Calling <see cref="InstallPackage.ApplyPatch"/> fails commits so we need to apply applicable transforms directly.
    /// </remarks>
    internal sealed class PatchApplicator
    {
        /// <summary>
        /// The transforms errors to ignore.
        /// </summary>
        internal const TransformErrors IgnoreErrors = TransformErrors.AddExistingRow | TransformErrors.AddExistingTable | TransformErrors.DelMissingRow
                    | TransformErrors.DelMissingTable | TransformErrors.UpdateMissingRow;

        private InstallPackage db;
        private PatchSequencer sequencer;

        /// <summary>
        /// Creates a new instance of the <see cref="PatchApplicator"/> class.
        /// </summary>
        /// <param name="db">The <see cref="InstallPackage"/> to transform.</param>
        internal PatchApplicator(InstallPackage db)
        {
            if (null == db)
            {
                throw new ArgumentNullException("db");
            }

            this.db = db;
            this.sequencer = new PatchSequencer();
        }

        /// <summary>
        /// Raised when an inapplicable patch is processed.
        /// </summary>
        internal event EventHandler<InapplicablePatchEventArgs> InapplicablePatch
        {
            // Just forward events to the sequencer.
            add { this.sequencer.InapplicablePatch += value; }
            remove { this.sequencer.InapplicablePatch -= value; }
        }

        /// <summary>
        /// Adds a patch to the list for consideration.
        /// </summary>
        /// <param name="path">The path of a patch to add.</param>
        internal void Add(string path)
        {
            this.sequencer.Add(path, true);
        }

        /// <summary>
        /// Applies applicable transforms in order of sequenced patches.
        /// </summary>
        internal void Apply(bool throwOnError = false)
        {
            // Need to make a copy of the database since exclusivity is required.
            IEnumerable<string> applicable = null;
            using (var copy = Copy(this.db))
            {
                // Copy the items to a list so they are enumerated immediately and the temporary database can be closed.
                applicable = this.sequencer.GetApplicablePatches(copy.FilePath).Select(patch => patch.Patch).ToList();
            }

            foreach (var path in applicable)
            {
                using (var patch = new PatchPackage(path))
                {
                    var transforms = patch.GetValidTransforms(this.db);
                    foreach (var transform in transforms)
                    {
                        // GetValidTransforms does not return the patch transform so assume it too is valid.
                        foreach (var prefix in new string[] { string.Empty, "#" })
                        {
                            var temp = Path.ChangeExtension(Path.GetTempFileName(), ".mst");
                            patch.ExtractTransform(prefix + transform, temp);

                            // Apply and commit the authored transform so further transforms may apply.
                            this.db.ApplyTransform(temp, IgnoreErrors);
                            this.db.Commit();

                            // Attempt to delete the temporary transform.
                            TryDelete(temp);
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static InstallPackage Copy(InstallPackage db)
        {
            string temp = Path.ChangeExtension(Path.GetTempFileName(), ".msi");
            File.Copy(db.FilePath, temp, true);

            // Open a copy and schedule delete it when closed.
            var copy = new InstallPackage(temp, DatabaseOpenMode.ReadOnly);
            copy.DeleteOnClose(temp);

            return copy;
        }

        private static void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
            }
        }
    }
}
