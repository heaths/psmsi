// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The data for actions to install a package.
    /// </summary>
    public class InstallCommandActionData
    {
        private long weight = 0;

        /// <summary>
        /// Gets or sets the package path for which the action is performed.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the ProductCode for which the action is performed.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the fully expanded command line to process when performing the action.
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// Gets an identifying name used for logging.
        /// </summary>
        internal virtual string LogName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Path))
                {
                    return System.IO.Path.GetFileNameWithoutExtension(this.Path);
                }
                else if (!string.IsNullOrEmpty(this.ProductCode))
                {
                    return this.ProductCode;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the weight of the package for progress reporting.
        /// </summary>
        /// <value>The calculated weight of the package, or the <see cref="PackageInfo.DefaultWeight"/> if not specified.</value>
        public long Weight
        {
            get
            {
                if (0 >= this.weight)
                {
                    return PackageInfo.DefaultWeight;
                }

                return this.weight;
            }

            set { this.weight = value; }
        }

        /// <summary>
        /// Creates an instance of an <see cref="InstallCommandActionData"/> class from the given file path.
        /// </summary>
        /// <typeparam name="T">The specific type of <see cref="InstallCommandActionData"/> to create.</typeparam>
        /// <param name="resolver">A <see cref="PathIntrinsics"/> object to resolve the file path.</param>
        /// <param name="file">A <see cref="PSObject"/> wrapping a file path.</param>
        /// <returns>An instance of an <see cref="InstallCommandActionData"/> class.</returns>
        public static T CreateActionData<T>(PathIntrinsics resolver, PSObject file) where T : InstallCommandActionData, new()
        {
            if (null == resolver)
            {
                throw new ArgumentNullException("resolver");
            }
            else if (null == file)
            {
                throw new ArgumentNullException("file");
            }

            var data = new T()
            {
                Path = resolver.GetUnresolvedProviderPathFromPSPath(file.Properties["PSPath"].Value as string),
            };

            return data;
        }

        /// <summary>
        /// Parses the arguments into the <see cref="CommandLine"/> property.
        /// </summary>
        /// <param name="args">The arguments to parse.</param>
        public void ParseCommandLine(string[] args)
        {
            if (null != args && 0 < args.Length)
            {
                this.CommandLine = string.Join(" ", args);
            }
        }

        /// <summary>
        /// Updates the <see cref="Weight"/> for progress reporting.
        /// </summary>
        public virtual void UpdateWeight()
        {
            if (!string.IsNullOrEmpty(this.Path))
            {
                this.Weight = PackageInfo.GetWeightFromPath(this.Path);
            }
            else if (!string.IsNullOrEmpty(this.ProductCode))
            {
                this.Weight = PackageInfo.GetWeightFromProductCode(this.ProductCode);
            }
        }
    }
}
