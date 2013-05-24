// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using Microsoft.Tools.WindowsInstaller.PowerShell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Determines patch applicability for given products.
    /// </summary>
    internal sealed class PatchSequencer
    {
        private static readonly string Namespace = @"http://www.microsoft.com/msi/patch_applicability.xsd";

        /// <summary>
        /// Creates a new instance of the <see cref="PatchSequencer"/> class.
        /// </summary>
        internal PatchSequencer()
        {
            this.Patches = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            this.TargetProductCodes = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Raised when an inapplicable patch is processed.
        /// </summary>
        internal event EventHandler<InapplicablePatchEventArgs> InapplicablePatch;

        /// <summary>
        /// Gets the list of all unique patch or patch XML file paths added to the sequencer.
        /// </summary>
        internal Set<string> Patches { get; private set; }

        /// <summary>
        /// Gets the list of all unique target ProductCodes for all patches or a given set.
        /// </summary>
        internal Set<string> TargetProductCodes { get; private set; }

        /// <summary>
        /// Adds the path to a patch or patch XML.
        /// </summary>
        /// <param name="path">The path to a patch or patch XML file.</param>
        /// <param name="validatePatch">If true, validates that the patch points only to a patch.</param>
        /// <returns>True if the path was added; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> does not exist or is not a file.</exception>
        internal bool Add(string path, bool validatePatch = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            else if (!File.Exists(path))
            {
                string message = string.Format(Properties.Resources.Error_InvalidFile, path);
                throw new FileNotFoundException(message, path);
            }

            bool ispatch = IsPatch(path);
            if (!validatePatch || ispatch)
            {
                this.Patches.Add(path);

                // Add the target ProductCodes to the set.
                if (ispatch)
                {
                    this.AddTargetProductCodesFromPatch(path);
                }
                else
                {
                    this.AddTargetProductCodesFromXml(path);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a list of patches that are applicable to the given ProductCode.
        /// </summary>
        /// <param name="product">The ProductCode for which applicability is determined.</param>
        /// <param name="userSid">The SID of the user for which the product is installed. The default is null.</param>
        /// <param name="context">The <see cref="UserContexts"/> into which the product is installed. This must be <see cref="UserContexts.None"/> for a package path. The default is <see cref="UserContexts.None"/>.</param>
        /// <returns>An ordered list of paths to applicable patch or patch XML files.</returns>
        /// <exception cref="ArgumentException">The parameters are not correct for the given package path or installed ProductCode (ex: cannot use <see cref="UserContexts.All"/> in any case).</exception>
        internal IList<string> GetApplicablePatches(string product, string userSid = null, UserContexts context = UserContexts.None)
        {
            var patches = this.Patches.ToArray();
            InapplicablePatchHandler handler = (patch, ex) => this.OnInapplicablePatch(new InapplicablePatchEventArgs(patch, product, ex));

            return Installer.DetermineApplicablePatches(product, patches, handler, userSid, context);
        }

        private void OnInapplicablePatch(InapplicablePatchEventArgs args)
        {
            var handler = this.InapplicablePatch;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        private void AddTargetProductCodesFromPatch(string path)
        {
            using (var patch = new PatchPackage(path))
            {
                foreach (var productCode in patch.GetTargetProductCodes())
                {
                    this.TargetProductCodes.Add(productCode);
                }
            }
        }

        private void AddTargetProductCodesFromXml(string path)
        {
            using (var file = System.IO.File.OpenRead(path))
            {
                var doc = new XPathDocument(file);
                var nav = doc.CreateNavigator();

                nav.MoveToChild("MsiPatch", Namespace);
                var itor = nav.SelectChildren("TargetProductCode", Namespace);

                while (itor.MoveNext())
                {
                    this.TargetProductCodes.Add(itor.Current.Value);
                }
            }
        }

        private static bool IsPatch(string path)
        {
            try
            {
                return FileType.Patch == PowerShell.FileInfo.GetFileTypeInternal(path);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Contains information about inapplicable patches.
    /// </summary>
    internal sealed class InapplicablePatchEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InapplicablePatchEventArgs"/> class.
        /// </summary>
        /// <param name="patch">The path to the patch or patch XML file that is not applicable.</param>
        /// <param name="product">The ProductCode for or path to the target product.</param>
        /// <param name="exception">Exception information about why the patch is not applicable.</param>
        internal InapplicablePatchEventArgs(string patch, string product, Exception exception)
        {
            this.Patch = patch;
            this.Product = product;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the path to the patch or patch XML file.
        /// </summary>
        internal string Patch { get; private set; }

        /// <summary>
        /// Gets the ProductCode for or path to the target product.
        /// </summary>
        internal string Product { get; private set; }

        /// <summary>
        /// Gets exception information about why the patch is not applicable.
        /// </summary>
        internal Exception Exception { get; private set; }
    }
}
