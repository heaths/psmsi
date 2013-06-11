// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIPatchInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIPatchInfo", DefaultParameterSetName = ParameterSet.Patch)]
    [OutputType(typeof(PatchInstallation))]
    public sealed class GetPatchCommand : PSCmdlet
    {
        private static readonly string[] Empty = new string[] { null };

        private List<Parameters> allParameters = new List<Parameters>();
        private PatchStates filter = PatchStates.Applied;
        private UserContexts context = UserContexts.All;

        // Parameter positions below are to maintain backward call-compatibility.

        /// <summary>
        /// Gets or sets the ProductCodes for which patches are enumerated.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Patch, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode { get; set; }

        /// <summary>
        /// Gets or sets patch codes for which information is retrieved.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Patch, Position = 1, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] PatchCode { get; set; }

        /// <summary>
        /// Gets or sets the patch states filter for enumeration.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public PatchStates Filter
        {
            get { return this.filter; }
            set
            {
                if (value == PatchStates.None)
                {
                    throw new ArgumentException(Properties.Resources.Error_InvalidFilter);
                }

                this.filter = value;
            }
        }

        /// <summary>
        /// Gets or sets the user context for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("Context", "InstallContext")] // Backward compatibility.
        public UserContexts UserContext
        {
            get { return this.context; }
            set
            {
                if (value == UserContexts.None)
                {
                    string message = string.Format(Properties.Resources.Error_InvalidContext, UserContexts.None);
                    throw new ArgumentException(message, "UserContext");
                }

                this.context = value;
            }
        }

        /// <summary>
        /// Gets or sets the user security identifier for products to enumerate.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("User")]
        [Sid]
        public string UserSid { get; set; }

        /// <summary>
        /// Gets or sets whether products for everyone should be enumerated.
        /// </summary>
        [Parameter]
        public SwitchParameter Everyone
        {
            get { return string.Compare(this.UserSid, NativeMethods.World, StringComparison.OrdinalIgnoreCase) == 0; }
            set { this.UserSid = value ? NativeMethods.World : null; }
        }

        /// <summary>
        /// Collects input ProductCodes and PatchCodes for future processing.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.allParameters.Add(new Parameters
                {
                    ParameterSetName = this.ParameterSetName,
                    ProductCode = this.ProductCode != null && this.ProductCode.Length > 0 ? this.ProductCode : new string[] { null},
                    PatchCode = this.PatchCode != null && this.PatchCode.Length > 0 ? this.PatchCode : new string[] { null},
                    Filter = this.Filter,
                    UserContext = this.UserContext,
                    UserSid = this.UserSid,
                });
        }

        /// <summary>
        /// Processes the ProductCodes or PatchCodes and writes a patch to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            this.allParameters.ForEach((param) =>
                {
                    foreach (string productCode in param.ProductCode)
                    {
                        foreach (string patchCode in param.PatchCode)
                        {
                            this.WritePatches(patchCode, productCode, param.UserSid, param.UserContext, param.Filter);
                        }
                    }
                });
        }

        /// <summary>
        /// Enumerates patches for the given patch codes and ProductCodes and writes them to the pipeline.
        /// </summary>
        /// <param name="patchCode">The patch code to enumerate.</param>
        /// <param name="productCode">The ProductCode having patches to enumerate.</param>
        /// <param name="userSid">The user's SID for patches to enumerate.</param>
        /// <param name="context">The installation context for patches to enumerate.</param>
        /// <param name="filter">The patch installation state for patches to enumerate.</param>
        private void WritePatches(string patchCode, string productCode, string userSid, UserContexts context, PatchStates filter)
        {
            foreach (PatchInstallation patch in PatchInstallation.GetPatches(patchCode, productCode, userSid, context, filter))
            {
                this.WritePatch(patch);
            }
        }

        /// <summary>
        /// Adds properties to the <see cref="PatchInstallation"/> object and writes it to the pipeline.
        /// </summary>
        /// <param name="patch">The <see cref="PatchInstallation"/> to write to the pipeline.</param>
        private void WritePatch(PatchInstallation patch)
        {
            var obj = patch.ToPSObject(this.SessionState.Path);
            this.WriteObject(obj);
        }

        /// <summary>
        /// Collects parameters for processing.
        /// </summary>
        private sealed class Parameters
        {
            /// <summary>
            /// Gets or sets the parameter set name.
            /// </summary>
            internal string ParameterSetName;

            /// <summary>
            /// Gets or sets the ProductCodes.
            /// </summary>
            internal string[] ProductCode;

            /// <summary>
            /// Gets or sets the patch codes.
            /// </summary>
            internal string[] PatchCode;

            /// <summary>
            /// Gets or sets the filter.
            /// </summary>
            internal PatchStates Filter;

            /// <summary>
            /// Gets or sets the installation context.
            /// </summary>
            internal UserContexts UserContext;

            /// <summary>
            /// Gets or sets the user's SID.
            /// </summary>
            internal string UserSid;
        }
    }
}
