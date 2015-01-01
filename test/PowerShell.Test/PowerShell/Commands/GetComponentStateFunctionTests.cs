// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the Get-MSIComponentState function.
    /// </summary>
    [TestClass]
    public sealed class GetComponentStateFunctionTests : TestBase
    {
        private static readonly string[] TestComponentCodes = new string[] { "{69CE8679-2CD6-4711-8133-D778D2A47967}", "{B88B6441-D16B-4308-B03A-A4BBC0F8F022}" };
        private static readonly string[] TestComponentIds = new string[] { "File", "Registry" };

        [TestMethod]
        public void GetProductInstallationComponentState()
        {
            // Since the product is not actually installed, looked up the component key path in the deployment directory.
            var properties = base.DefaultRegistryProperties;
            properties["TestRunDirectory"] = this.TestContext.DeploymentDirectory;

            using (var p = CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msicomponentstate"))
            {
                using (OverrideRegistry(properties: properties))
                {
                    var output = p.Invoke();
                    Assert.AreEqual<int>(2, output.Count);

                    foreach (var item in output)
                    {
                        Assert.AreEqual<string>("Microsoft.Deployment.WindowsInstaller.ComponentInstallation#State", item.TypeNames[0]);
                        Assert.AreEqual<InstallState>(InstallState.Local, item.GetPropertyValue<InstallState>("State"));
                        Assert.IsTrue(item.GetPropertyValue<bool>("IsInstalled"));
                        CollectionAssert.Contains(TestComponentCodes, item.GetPropertyValue<string>("ComponentCode"));
                        CollectionAssert.Contains(TestComponentIds, item.GetPropertyValue<string>("Component"));
                    }
                }
            }
        }

        [TestMethod]
        public void GetProductCodeComponentState()
        {
            // Since the product is not actually installed, looked up the component key path in the deployment directory.
            var properties = base.DefaultRegistryProperties;
            properties["TestRunDirectory"] = this.TestContext.DeploymentDirectory;

            using (var p = CreatePipeline(@"get-msicomponentstate '{877EF582-78AF-4D84-888B-167FDC3BCC11}'"))
            {
                using (OverrideRegistry(properties: properties))
                {
                    var output = p.Invoke();
                    Assert.AreEqual<int>(2, output.Count);

                    foreach (var item in output)
                    {
                        Assert.AreEqual<string>("Microsoft.Deployment.WindowsInstaller.ComponentInstallation#State", item.TypeNames[0]);
                        Assert.AreEqual<InstallState>(InstallState.Local, item.GetPropertyValue<InstallState>("State"));
                        Assert.IsTrue(item.GetPropertyValue<bool>("IsInstalled"));
                        CollectionAssert.Contains(TestComponentCodes, item.GetPropertyValue<string>("ComponentCode"));
                        CollectionAssert.Contains(TestComponentIds, item.GetPropertyValue<string>("Component"));
                    }
                }
            }
        }
    }
}
