// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for derivations of the <see cref="InstallCommandBase"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallScenarioTests : CommandTestBase
    {
        [TestCleanup]
        [TestCategory("Impactful")]
        public void Cleanup()
        {
            var product = new ProductInstallation("{5F45B2AD-6B18-40EF-AA07-083876579689}", null, UserContexts.All);
            if (null != product && product.IsInstalled)
            {
                Installer.ConfigureProduct(product.ProductCode, 0, InstallState.Absent, "REMOVE=ALL");
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void BasicInstall()
        {
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" TARGETDIR=`""$TestRunDirectory\BasicInstall`"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
                Assert.IsTrue(File.Exists(Path.Combine(this.TestContext.TestRunDirectory, @"BasicInstall\product.wxs")), "The product was not installed to the correct location.");
            }

            using (var p = TestRunspace.CreatePipeline(@"repair-msiproduct ""$TestDeploymentDirectory\example.msi"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"uninstall-msiproduct ""$TestDeploymentDirectory\example.msi"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void PipelineInstall()
        {
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" -destination ""$TestRunDirectory\PipelineInstall"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
                Assert.IsTrue(File.Exists(Path.Combine(this.TestContext.TestRunDirectory, @"PipelineInstall\product.wxs")), "The product was not installed to the correct location.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{5F45B2AD-6B18-40EF-AA07-083876579689}' -context all | install-msiproduct -properties A=1"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{5F45B2AD-6B18-40EF-AA07-083876579689}' -context all | repair-msiproduct"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{5F45B2AD-6B18-40EF-AA07-083876579689}' -context all | uninstall-msiproduct"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void PassThruInstall()
        {
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" -destination ""$TestRunDirectory\PassThruInstall"" -passthru | repair-msiproduct -passthru | uninstall-msiproduct"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }
        }
    }
}