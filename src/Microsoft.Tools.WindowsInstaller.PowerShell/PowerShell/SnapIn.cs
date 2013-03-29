// Installs Cmdlets into PowerShell.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.ComponentModel;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Installer class for installutil-supported installation.
    /// </summary>
    /// <remarks>
    /// The snap-in should be installed using Windows Installer or imported as a module.
    /// </remarks>
    [RunInstaller(true)]
    public sealed class SnapIn : PSSnapIn
    {
        /// <summary>
        /// Gets the fixed name of the snap-in.
        /// </summary>
        public override string Name { get { return "psmsi"; } }

        /// <summary>
        /// Gets the name of the vendor.
        /// </summary>
        public override string Vendor { get { return Properties.Resources.SnapIn_Vendor; } }

        /// <summary>
        /// Gets the localizable resource name of the vendor.
        /// </summary>
        public override string VendorResource
        {
            get
            {
                return "Microsoft.Tools.WindowsInstaller.Properties.Resources,SnapIn_Vendor";
            }
        }

        /// <summary>
        /// Gets the description of this snap-in.
        /// </summary>
        public override string Description { get { return Properties.Resources.SnapIn_Description; } }


        /// <summary>
        /// Gets the localizable resource name of the description of this snap-in.
        /// </summary>
        public override string DescriptionResource
        {
            get
            {
                return "Microsoft.Tools.WindowsInstaller.Properties.Resources,SnapIn_Description";
            }
        }

        /// <summary>
        /// Gets the list of format XML files.
        /// </summary>
        public override string[] Formats
        {
            get
            {
                return new string[] { "MSI.formats.ps1xml" };
            }
        }

        /// <summary>
        /// Gets the list of types XML files.
        /// </summary>
        public override string[] Types
        {
            get
            {
                return new string[] { "MSI.types.ps1xml" };
            }
        }
    }
}
