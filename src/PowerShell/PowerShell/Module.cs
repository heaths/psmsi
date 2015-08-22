// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
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
