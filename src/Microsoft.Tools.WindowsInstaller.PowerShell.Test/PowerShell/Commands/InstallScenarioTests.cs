// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for derivations of the <see cref="InstallCommandBase"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallScenarioTests : CommandTestBase
    {
        [TestMethod]
        [TestCategory("Impactful")]
        public void BasicInstall()
        {
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" TARGETDIR=`""$TestRunDirectory\BasicInstall`"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
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
            using (var p = TestRunspace.CreatePipeline(@"install-msiproduct ""$TestDeploymentDirectory\example.msi"" TARGETDIR=`""$TestRunDirectory\BasicInstall`"""))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output was not expected.");
                Assert.IsTrue(null == p.Error || 0 == p.Error.Count, "Errors were not expected.");
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
    }
}
