// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Edit-MSIPackage cmdlet.
    /// </summary>
    [Cmdlet(VerbsData.Edit, "MSIPackage", DefaultParameterSetName = ParameterSet.Path)]
    public sealed class EditPackageCommand : ItemCommandBase
    {
        private string orcaPath = null;

        /// <summary>
        /// Gets or sets whether to wait for Orca to close before processing the next item.
        /// </summary>
        [Parameter]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// Ges the path to Orca if installed; otherwise, displays a warning.
        /// </summary>
        protected override void BeginProcessing()
        {
            this.orcaPath = ComponentSearcher.Find(ComponentSearcher.KnownComponent.Orca);
            if (string.IsNullOrEmpty(this.orcaPath))
            {
                this.WriteWarning(Resources.Error_OrcaAbsent);
            }
        }

        /// <summary>
        /// Attempts to open the item in Orca, if installed; otherwise, tries to invoke the "edit" verb on the package.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> representing a package to open.</param>
        protected override void ProcessItem(PSObject item)
        {
            string path = item.GetPropertyValue<string>("PSPath");
            path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            // Make sure the item is an MSI or MSP package.
            var type = FileInfo.GetFileTypeInternal(path);
            if (FileType.Package != type && FileType.Patch != type)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidStorage, path);
                var ex = new PSInvalidOperationException(message);
                this.WriteError(ex.ErrorRecord);

                return;
            }

            var info = new ProcessStartInfo()
            {
                WorkingDirectory = System.IO.Path.GetDirectoryName(path),
            };

            if (!string.IsNullOrEmpty(this.orcaPath))
            {
                // Open in Orca, if installed.
                info.FileName = this.orcaPath;
                info.Arguments = "\"" + path + "\"";
            }
            else
            {
                // Try to use the edit verb instead.
                info.FileName = path;
                info.UseShellExecute = true;
                info.Verb = "edit";
            }

            Process process = null;
            try
            {
                process = Process.Start(info);
                if (this.Wait)
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is Win32Exception)
                {
                    // Likely the "edit" verb is not supported so terminate.
                    var pse = new PSInvalidOperationException(ex.Message, ex);
                    this.ThrowTerminatingError(pse.ErrorRecord);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (null != process)
                {
                    process.Dispose();
                }
            }
        }
    }
}
