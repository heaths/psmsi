// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for patch-related commands.
    /// </summary>
    public abstract class InstallPatchCommandBase<T> : InstallCommandBase<T> where T : InstallPatchActionData, new()
    {
        /// <summary>
        /// Creates a new instance of the class and sets default property values.
        /// </summary>
        protected InstallPatchCommandBase()
        {
            // Most products are installed per-machine.
            this.UserContext = UserContexts.Machine;
        }

        /// <summary>
        /// Gets or sets the <see cref="ProductInstallation"/> to install.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Installation, Mandatory = true, ValueFromPipeline = true)]
        public PatchInstallation[] Patch { get; set; }

        /// <summary>
        /// Gets or sets the ProductCode to which patches apply.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the user context for products to which patches apply.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("Context", "InstallContext")]
        public UserContexts UserContext { get; set; }

        /// <summary>
        /// Gets or sets the user security identifier for products to which patches apply.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("User"), Sid]
        public string UserSid { get; set; }

        /// <summary>
        /// Gets an activity string similar to "Configuring".
        /// </summary>
        protected override string Activity
        {
            get { return Resources.Action_Configure; }
        }

        /// <summary>
        /// Queues applicable patches for any given products, or installed products for all patches.
        /// </summary>
        protected override void QueueActions()
        {
            // Validate property values.
            if (UserContexts.All == this.UserContext)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidContext, UserContexts.All);
                throw new ArgumentException(message, "UserContext");
            }

            var sequencer = new PatchSequencer();
            sequencer.InapplicablePatch += (source, args) =>
                {
                    // Log verbose information that the patch does not apply.
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_InapplicablePatch, args.Patch, args.Product);
                    this.WriteVerbose(message);

                    // Attempt to log why the patch does not apply to the debug stream.
                    if (null != args.Exception)
                    {
                        message = args.Exception.Message;
                        if (!string.IsNullOrEmpty(message))
                        {
                            this.WriteDebug(message);
                        }
                    }
                };

            if (this.ParameterSetName == ParameterSet.Installation)
            {
                // Add the patch information to the sequencer.
                foreach (var patch in this.Patch)
                {
                    string path = patch.LocalPackage;
                    if (!string.IsNullOrEmpty(path))
                    {
                        sequencer.Add(path, true);
                    }
                }
            }
            else
            {
                // Enumerate through the patch files and add them to the sequencer.
                var files = this.InvokeProvider.Item.Get(this.Path, true, ParameterSet.LiteralPath == this.ParameterSetName);
                foreach (var file in files)
                {
                    string path = file.GetPropertyValue<string>("PSPath");
                    path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

                    if (!string.IsNullOrEmpty(path))
                    {
                        sequencer.Add(path, true);
                    }
                }
            }

            // Use the given list of ProductCodes, or all harvested target ProductCodes.
            var targetProductCodes = new List<string>();
            if (null != this.ProductCode && 0 < this.ProductCode.Length)
            {
                targetProductCodes.AddRange(this.ProductCode);
            }
            else
            {
                targetProductCodes.AddRange(sequencer.TargetProductCodes);
            }

            // Enumerate through the ProductCodes and sequence the patch actions in a separate thread.
            foreach (var productCode in targetProductCodes)
            {
                var result = sequencer.BeginGetApplicablePatches(productCode, this.UserSid, this.UserContext);
                using (result.AsyncWaitHandle)
                {
                    result.AsyncWaitHandle.WaitOne();
                }

                // Select just the path to the patch package.
                var patches = sequencer.EndGetApplicablePatches(result).Select(patch => patch.Patch);
                if (null != patches)
                {
                    // Queue an action for each product with all applicable patches.
                    var applicable = patches.ToList();
                    if (null != applicable && 0 < applicable.Count)
                    {
                        var data = new T()
                        {
                            ProductCode = productCode,
                            UserSid = this.UserSid,
                            UserContext = this.UserContext,
                        };

                        foreach (string path in applicable)
                        {
                            data.Patches.Add(path);
                        }

                        data.ParseCommandLine(this.Properties);
                        this.UpdateAction(data);

                        this.Actions.Enqueue(data);
                    }
                }
            }
        }
    }
}
