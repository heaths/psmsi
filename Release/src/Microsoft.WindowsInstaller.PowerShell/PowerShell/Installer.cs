// Installs Cmdlets into PowerShell.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 08:14:04 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Management;
using System.Management.Automation;
using Microsoft.WindowsInstaller.PowerShell;
using Microsoft.WindowsInstaller.Properties;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Installer class for installutil-supported installation.
    /// </summary>
    /// <remarks>
    /// The snap-in should be installed using Windows Installer or imported as a module.
    /// </remarks>
    [RunInstaller(true)]
    public sealed class WindowsInstallerSnapIn : PSSnapIn
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
        /// Gets the localiable resource name of the vendor.
        /// </summary>
        public override string VendorResource
        {
            get
            {
                return "Microsoft.WindowsInstaller.Properties.Resources,SnapIn_Vendor";
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
                return "Microsoft.WindowsInstaller.Properties.Resources,SnapIn_Description";
            }
        }

        /// <summary>
        /// Gets the list of format XML files.
        /// </summary>
        public override string[] Formats
        {
            get
            {
                string[] formats = { "WindowsInstaller.formats.ps1xml" };
                return formats;
            }
        }

        /// <summary>
        /// Gets the list of types XML files.
        /// </summary>
        public override string[] Types
        {
            get
            {
                string[] types = { "WindowsInstaller.types.ps1xml" };
                return types;
            }
        }
    }
}
