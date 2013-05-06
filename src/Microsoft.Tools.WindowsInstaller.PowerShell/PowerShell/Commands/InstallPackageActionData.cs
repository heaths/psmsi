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
    public class InstallPackageActionData
    {
        /// <summary>
        /// Gets or sets the package path for which the action is performed.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the fully expanded command line to process when performing the action.
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// Creates an instance of an <see cref="InstallPackageActionData"/> class from the given file path.
        /// </summary>
        /// <typeparam name="T">The specific type of <see cref="InstallPackageActionData"/> to create.</typeparam>
        /// <param name="resolver">A <see cref="PathIntrinsics"/> object to resolve the file path.</param>
        /// <param name="file">A <see cref="PSObject"/> wrapping a file path.</param>
        /// <returns>An instance of an <see cref="InstallPackageActionData"/> class.</returns>
        public static T CreateActionData<T>(PathIntrinsics resolver, PSObject file) where T : InstallPackageActionData, new()
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
    }
}
