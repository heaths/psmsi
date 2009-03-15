// Static methods specific to the WindowsInstaller module.
//
// Created: Sat, 14 Mar 2009 22:59:16 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using Microsoft.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Static methods specific to the WindowsInstaller module.
    /// </summary>
    public static class Module
    {
        private static readonly string FeatureName = "Module";
        private static readonly string ModuleId = "{CE1F8ECF-0E25-4155-9BE1-E9DC1CADA4C2}";

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
                    NativeMethods.MsiUseFeatureEx(product.ProductCode, FeatureName, InstallMode.NoDetection, 0);
                }
            }
        }
    }
}
