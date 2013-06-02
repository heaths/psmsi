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
            var product = new ProductInstallation("{877EF582-78AF-4D84-888B-167FDC3BCC11}", null, UserContexts.All);
            if (null != product && product.IsInstalled)
            {
                Installer.ConfigureProduct(product.ProductCode, 0, InstallState.Absent, "REMOVE=ALL");
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void BasicInstall()
        {
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" TARGETDIR=`""$TestRunDirectory`"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
                Assert.IsTrue(File.Exists(Path.Combine(this.TestContext.TestRunDirectory, @"product.wxs")), "The product was not installed to the correct location.");
            }

            using (var p = TestRunspace.CreatePipeline(@"repair-msiproduct ""$TestDeploymentDirectory\example.msi"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"install-msipatch ""$TestDeploymentDirectory\example.msp"" -context userunmanaged"))
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
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" -destination ""$TestRunDirectory"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
                Assert.IsTrue(File.Exists(Path.Combine(this.TestContext.TestRunDirectory, @"product.wxs")), "The product was not installed to the correct location.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' -context all | install-msiproduct -properties A=1"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' -context all | install-msipatch ""$TestDeploymentDirectory\example.msp"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' -context all | repair-msiproduct"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' -context all | uninstall-msipatch ""$TestDeploymentDirectory\example.msp"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }

            using (var p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' -context all | uninstall-msiproduct"))
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
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" -destination ""$TestRunDirectory"" -passthru | repair-msiproduct -passthru | install-msipatch ""$TestDeploymentDirectory\example.msp"" -passthru | uninstall-msiproduct"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
            }
        }
    }
}