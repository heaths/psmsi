// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="GetPatchSequenceCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class GetPatchSequenceCommandTests : CommandTestBase
    {
        [TestMethod]
        public void SequencePatchesForPackage()
        {
            // No parameter names also tests that the default parameter set is correct.
            using (var p = this.TestRunspace.CreatePipeline(@"get-msipatchsequence ""$TestDeploymentDirectory\example.msp"" ""$TestDeploymentDirectory\example.msi"""))
            {
                var items = p.Invoke();

                Assert.IsNotNull(items, "No output was returned.");
                Assert.AreEqual<int>(1, items.Count, "The number of output items is incorrect.");

                var patch = items[0].BaseObject as PatchSequence;
                Assert.IsNotNull(patch, "The output type is incorrect.");
                Assert.AreEqual(Path.Combine(this.TestContext.DeploymentDirectory, "Example.msp"), patch.Patch, true, "The Patch property is incorrect.");
                Assert.AreEqual<int>(0, patch.Sequence, "The Sequence property is incorrect.");
                Assert.AreEqual(Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi"), patch.Product, true, "The Product property is incorrect.");
                Assert.IsNull(patch.ProductCode, "The ProductCode property is incorrect.");
            }
        }

        [Ignore, TestMethod]
        public void SequencePatchesForProduct()
        {
            // TODO: Need to replace registered products and patches in Registry.xml with Data\Example.* to make this work.

            // Tests piping to the cmdlet.
            using (var p = this.TestRunspace.CreatePipeline(@"get-msipatchinfo -product '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' | get-msipatchsequence -product '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}'"))
            {
                using (var reg = new MockRegistry())
                {
                    reg.Import(Path.Combine(this.TestContext.DeploymentDirectory, "Registry.xml"));

                    var items = p.Invoke();

                    Assert.IsNotNull(items, "No output was returned.");
                    Assert.AreEqual<int>(1, items.Count, "The number of output items is incorrect.");

                    var patch = items[0].BaseObject as PatchSequence;
                    Assert.IsNotNull(patch, "The output type is incorrect.");
                    Assert.AreEqual(@"c:\Windows\Installer\16ca0c.msp", patch.Patch, true, "The Patch property is incorrect.");
                    Assert.AreEqual<int>(0, patch.Sequence, "The Sequence property is incorrect.");
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", patch.Product, "The Product property is incorrect.");
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", patch.ProductCode, "The ProductCode property is incorrect.");
                }
            }
        }
    }
}
