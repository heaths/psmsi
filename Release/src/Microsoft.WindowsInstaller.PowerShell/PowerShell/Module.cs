// Static methods specific to the WindowsInstaller module.
//
// Created: Sat, 14 Mar 2009 22:59:16 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Static methods specific to the WindowsInstaller module.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Module", Justification = "This assembly is not designed for others' consumption.")]
    public static class Module
    {
        private const string FeatureName = "Module";
        private const string ModuleId = "{9D8E88E9-8E05-4FC7-AFC7-87759D1D417E}";

        /// <summary>
        /// Increments the use count and sets the last used date if the Module installer was installed.
        /// </summary>
        public static void Use()
        {
            // Enumerate all clients for the primary component if installed.
            ComponentInstallation comp = new ComponentInstallation(ModuleId);
            foreach (ProductInstallation product in comp.ClientProducts)
            {
                // If the feature is installed locally for this client, increment usage.
                FeatureInstallation feature = new FeatureInstallation(FeatureName, product.ProductCode);
                if (feature.State == InstallState.Local)
                {
					Installer.UseFeature(product.ProductCode, FeatureName, InstallMode.NoDetection);
                }
            }
        }
    }
}
