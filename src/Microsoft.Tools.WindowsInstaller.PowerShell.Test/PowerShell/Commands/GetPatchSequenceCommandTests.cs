// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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

        [TestMethod]
        public void SequencePatchesForProduct()
        {
            // Tests piping to the cmdlet.
            using (var p = this.TestRunspace.CreatePipeline(@"get-msipatchinfo -product '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msipatchsequence -product '{877EF582-78AF-4D84-888B-167FDC3BCC11}'"))
            {
                using (var reg = new MockRegistry())
                {
                    var properties = new Dictionary<string, string>()
                    {
                        { "TestDeploymentDirectory", this.TestContext.DeploymentDirectory },
                        { "TestRunDirectory", this.TestContext.TestRunDirectory },
                    };

                    reg.Import(Path.Combine(this.TestContext.DeploymentDirectory, "Registry.xml"), properties);

                    var items = p.Invoke();

                    Assert.IsNotNull(items, "No output was returned.");
                    Assert.AreEqual<int>(1, items.Count, "The number of output items is incorrect.");

                    var patch = items[0].BaseObject as PatchSequence;
                    Assert.IsNotNull(patch, "The output type is incorrect.");
                    Assert.AreEqual(Path.Combine(this.TestContext.DeploymentDirectory, "Example.msp"), patch.Patch, true, "The Patch property is incorrect.");
                    Assert.AreEqual<int>(0, patch.Sequence, "The Sequence property is incorrect.");
                    Assert.AreEqual<string>("{877EF582-78AF-4D84-888B-167FDC3BCC11}", patch.Product, "The Product property is incorrect.");
                    Assert.AreEqual<string>("{877EF582-78AF-4D84-888B-167FDC3BCC11}", patch.ProductCode, "The ProductCode property is incorrect.");
                }
            }
        }
    }
}
