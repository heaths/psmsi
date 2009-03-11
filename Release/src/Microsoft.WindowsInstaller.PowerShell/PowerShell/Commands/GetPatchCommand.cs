// Cmdlet to get or enumerator Windows Installer patches.
//
// Created: Thu, 01 Feb 2007 22:08:18 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIPatchInfo cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIPatchInfo", DefaultParameterSetName = ParameterSet.Patch)]
    public sealed class GetPatchCommand : PSCmdlet
    {
        private static readonly string[] Empty = new string[] { null };

        private string[] productCodes;
        private string[] patchCodes;
        private PatchStates filter;
        private UserContexts context;
        private string userSid;

        /// <summary>
        /// Creates a new instance of the <see cref="GetPatchCommand"/> class.
        /// </summary>
        public GetPatchCommand()
        {
            this.productCodes = null;
            this.patchCodes = null;
            this.filter = PatchStates.Applied;
            this.context = UserContexts.Machine;
            this.userSid = null;
        }

        // Parameter positions below are to maintain backward call-compatibility.

        /// <summary>
        /// Gets or sets the ProductCodes for which patches are enumerated.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Patch, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] ProductCode
        {
            get { return this.productCodes; }
            set { this.productCodes = value; }
        }

        /// <summary>
        /// Gets or sets patch codes for which information is retrieved.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Parameter(ParameterSetName = ParameterSet.Patch, Position = 1, ValueFromPipelineByPropertyName = true)]
        [ValidateGuid]
        public string[] PatchCode
        {
            get { return this.patchCodes; }
            set { this.patchCodes = value; }
        }

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
                    throw new ArgumentException();
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
                    throw new ArgumentException(Properties.Resources.Error_InvalidContext);
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
        public string UserSid
        {
            get { return this.userSid; }
            set { this.userSid = value; }
        }

        /// <summary>
        /// Gets or sets whether products for everyone should be enumerated.
        /// </summary>
        [Parameter]
        public SwitchParameter Everyone
        {
            get { return string.Compare(this.userSid, NativeMethods.World, true, CultureInfo.InvariantCulture) == 0; }
            set { this.userSid = value ? NativeMethods.World : null; }
        }

        /// <summary>
        /// Processes the ProductCodes or PatchCodes and writes a patch to the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Enumerate a set of null of no input was provided.
            if (this.productCodes == null || this.productCodes.Length == 0)
            {
                this.productCodes = Empty;
            }

            if (this.patchCodes == null || this.patchCodes.Length == 0)
            {
                this.patchCodes = Empty;
            }

            // Enumerate all given patches for all given products.
            foreach (string productCode in this.productCodes)
            {
                foreach (string patchCode in this.patchCodes)
                {
                    this.WritePatches(patchCode, productCode);
                }
            }
        }

        /// <summary>
        /// Enumerates patches for the given patch codes and ProductCodes and writes them to the pipeline.
        /// </summary>
        /// <param name="patchCode">The patch code to enumerate.</param>
        /// <param name="productCode">The ProductCode having patches to enumerate.</param>
        private void WritePatches(string patchCode, string productCode)
        {
            foreach (PatchInstallation patch in PatchInstallation.GetPatches(patchCode, productCode, this.userSid, this.context, this.filter))
            {
                PSObject obj = PSObject.AsPSObject(patch);

                // Add the local package as the PSPath.
                string path = PathConverter.ToPSPath(this.SessionState, patch.LocalPackage);
                obj.Properties.Add(new PSNoteProperty("PSPath", path));

                this.WriteObject(obj);
            }
        }
    }
}
