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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for derivations of the <see cref="InstallCommandBase"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallScenarioTests : TestBase
    {
        private static readonly string ExampleProductCode = "{877EF582-78AF-4D84-888B-167FDC3BCC11}";
        private static readonly string ExamplePatchCode = "{FF63D787-26E2-49CA-8FAA-28B5106ABD3A}";

        [TestCleanup]
        [TestCategory("Impactful")]
        public void Cleanup()
        {
            var products = ProductInstallation.GetProducts(ExampleProductCode, null, UserContexts.All);
            if (products.Any(p => p.IsAdvertised || p.IsInstalled))
            {
                var previous = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    foreach (var product in products)
                    {
                        Installer.ConfigureProduct(product.ProductCode, 0, InstallState.Absent, null);
                    }
                }
                finally
                {
                    Installer.SetInternalUI(previous);
                }
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void BasicInstall()
        {
            using (var p = CreatePipeline(@"install-msiproduct example.msi TARGETDIR=`""$TestRunDirectory`"""))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
                Assert.IsTrue(File.Exists(Path.Combine(this.TestContext.TestRunDirectory, @"product.wxs")));
            }

            using (var p = CreatePipeline(@"repair-msiproduct example.msi"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            using (var p = CreatePipeline(@"install-msipatch example.msp -context userunmanaged"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            using (var p = CreatePipeline(@"uninstall-msiproduct example.msi"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void PipelineInstall()
        {
            using (var p = CreatePipeline(@"install-msiproduct example.msi -destination ""$TestRunDirectory"""))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
                Assert.IsTrue(File.Exists(Path.Combine(this.TestContext.TestRunDirectory, @"product.wxs")));
            }

            var command = string.Format(@"get-msiproductinfo '{0}' -context all | install-msiproduct -properties A=1", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            command = string.Format(@"get-msiproductinfo '{0}' -context all | install-msipatch example.msp", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            command = string.Format(@"get-msiproductinfo '{0}' -context all | repair-msiproduct", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            command = string.Format(@"get-msiproductinfo '{0}' -context all | uninstall-msipatch example.msp", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            command = string.Format(@"get-msiproductinfo '{0}' -context all | uninstall-msiproduct", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        public void PassThruInstall()
        {
            using (var p = CreatePipeline(@"install-msiproduct example.msi -destination ""$TestRunDirectory"" -passthru | repair-msiproduct -passthru | install-msipatch example.msp -passthru | uninstall-msiproduct"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        [WorkItem(14460)]
        public void UninstallPatchFromPipeline()
        {
            using (var p = CreatePipeline(@"install-msiproduct example.msi -destination ""$TestRunDirectory"" -passthru | install-msipatch example.msp"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            var command = string.Format(@"get-msipatchinfo '{0}' | uninstall-msipatch", ExamplePatchCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }

            command = string.Format(@"get-msiproductinfo '{0}' | uninstall-msiproduct", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        [WorkItem(14471)]
        public void InstallAdvertisedFeatures()
        {
            using (var p = CreatePipeline(@"install-msiproduct example.msi ADVERTISE=ALL TARGETDIR=`""$TestRunDirectory`"""))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);

                var feature = new FeatureInstallation("TEST", ExampleProductCode);
                Assert.AreEqual<InstallState>(InstallState.Advertised, feature.State);
            }

            // Need to pass the target directory since we're not rooted under well-known folders.
            var command = string.Format(@"install-msiadvertisedfeature -productcode '{0}' -force -properties TARGETDIR=`""$TestRunDirectory`""", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);

                var feature = new FeatureInstallation("TEST", ExampleProductCode);
                Assert.AreEqual<InstallState>(InstallState.Local, feature.State);
            }

            using (var p = CreatePipeline(@"uninstall-msiproduct example.msi"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }
        }

        [TestMethod]
        [TestCategory("Impactful")]
        [WorkItem(14471)]
        public void InstallAdvertisedFeaturesThroughPipeline()
        {
            using (var p = CreatePipeline(@"install-msiproduct example.msi ADVERTISE=ALL TARGETDIR=`""$TestRunDirectory`"""))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);

                var feature = new FeatureInstallation("TEST", ExampleProductCode);
                Assert.AreEqual<InstallState>(InstallState.Advertised, feature.State);
            }

            // Need to pass the target directory since we're not rooted under well-known folders.
            var command = string.Format(@"get-msiproductinfo '{0}' | install-msiadvertisedfeature TEST -force -properties TARGETDIR=`""$TestRunDirectory`""", ExampleProductCode);
            using (var p = CreatePipeline(command))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);

                var feature = new FeatureInstallation("TEST", ExampleProductCode);
                Assert.AreEqual<InstallState>(InstallState.Local, feature.State);
            }

            using (var p = CreatePipeline(@"uninstall-msiproduct example.msi"))
            {
                var output = p.Invoke();
                Assert.AreEqual(0, output.Count);
                Assert.AreEqual(0, p.Error.Count);
            }
        }
    }
}