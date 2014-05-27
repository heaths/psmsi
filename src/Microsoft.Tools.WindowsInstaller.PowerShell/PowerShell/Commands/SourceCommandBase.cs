// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for source list cmdlets.
    /// </summary>
    public abstract class SourceCommandBase : PSCmdlet
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SourceCommandBase"/> class.
        /// </summary>
        protected SourceCommandBase()
        {
            this.PreviousParameters = new List<Parameters>();
            this.UserContext = UserContexts.All;
        }

        /// <summary>
        /// Gets or sets the ProductCode for a product or patch to which the source is registered.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the optional patch code for a patch to which the source is registered.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string PatchCode { get; set; }

        /// <summary>
        /// Gets or sets the user SID of the product or patch.
        /// </summary>
        [Alias("User"), Sid]
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string UserSid { get; set; }

        /// <summary>
        /// Gets or sets the user context of the product or patch.
        /// </summary>
        [Alias("Context", "InstallContext")]
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public UserContexts UserContext { get; set; }

        /// <summary>
        /// Gets the list of previous parameters to work around a reentrency problem with Windows Installer.
        /// </summary>
        protected List<Parameters> PreviousParameters { get; private set; }

        /// <summary>
        /// Adds current parameters to <see cref="PreviousParameters"/> towork around a reentrency problem with Windows Installer.
        /// </summary>
        protected sealed override void ProcessRecord()
        {
            var param = new Parameters
            {
                ProductCode = this.ProductCode,
                PatchCode = this.PatchCode,
                UserSid = this.UserSid,
                UserContext = this.UserContext,
            };

            this.PreviousParameters.Add(param);
        }

        /// <summary>
        /// Child classes must override and should use the <see cref="PreviousParameters"/> for processing records.
        /// </summary>
        protected abstract override void EndProcessing();

        /// <summary>
        /// Gets the <see cref="Installation"/> class given the <see cref="Parameters"/>.
        /// </summary>
        /// <param name="param">The <see cref="Parameters"/> of the <see cref="Installation"/> to get.</param>
        /// <returns>An <see cref="Installation"/> given the <see cref="Parameters"/>.</returns>
        protected Installation GetInstallation(Parameters param)
        {
            Installation installation = null;
            if (string.IsNullOrEmpty(param.PatchCode))
            {
                installation = ProductInstallation.GetProducts(param.ProductCode, param.UserSid, param.UserContext).FirstOrDefault();
            }
            else
            {
                installation = PatchInstallation.GetPatches(param.PatchCode, param.ProductCode, param.UserSid, param.UserContext, PatchStates.All).FirstOrDefault();
            }

            return installation;
        }

        /// <summary>
        /// Writes a source information to the pipeline.
        /// </summary>
        /// <param name="installation">The <see cref="Installation"/> to which the source is registered.</param>
        protected void WriteSourceList(Installation installation)
        {
            ProductInstallation product = null;
            PatchInstallation patch = null;
            var order = 0;

            if (installation is ProductInstallation)
            {
                product = (ProductInstallation)installation;
            }
            else if (installation is PatchInstallation)
            {
                patch = (PatchInstallation)installation;
            }

            try
            {
                foreach (var source in installation.SourceList)
                {
                    SourceInfo info = null;
                    if (null != product)
                    {
                        info = new SourceInfo(product.ProductCode, product.UserSid, product.Context, source, order++);
                    }
                    else if (null != patch)
                    {
                        info = new PatchSourceInfo(patch.ProductCode, patch.PatchCode, patch.UserSid, patch.Context, source, order++);
                    }

                    if (null != info)
                    {
                        this.WriteObject(info);
                    }
                }
            }
            catch (InstallerException ex)
            {
                if (NativeMethods.ERROR_BAD_CONFIGURATION == ex.ErrorCode)
                {
                    var code = null != product ? product.ProductCode : null != patch ? patch.PatchCode : string.Empty;
                    var message = string.Format(Properties.Resources.Error_Corrupt, code);
                    var exception = new Exception(message, ex);

                    var error = new ErrorRecord(exception, "Error_Corrupt", ErrorCategory.NotInstalled, installation);
                    base.WriteError(error);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Parameters persisted to work around a Windows Installer reentrency issue.
        /// </summary>
        protected class Parameters
        {
            /// <summary>
            /// Gets or sets the ProductCode parameter.
            /// </summary>
            public string ProductCode { get; internal set; }

            /// <summary>
            /// Gets or sets the PatchCode parameter.
            /// </summary>
            public string PatchCode { get; internal set; }

            /// <summary>
            /// Gets or sets the UserSid parameter.
            /// </summary>
            public string UserSid { get; internal set; }

            /// <summary>
            /// Gets or sets the UserContext parameter.
            /// </summary>
            public UserContexts UserContext { get; internal set; }
        }
    }
}
