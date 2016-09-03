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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="GetPatchSequenceCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class GetPatchSequenceCommandTests : TestBase
    {
        [TestMethod]
        public void SequencePatchesForPackage()
        {
            // No parameter names also tests that the default parameter set is correct.
            using (var p = CreatePipeline(@"get-msipatchsequence example.msp example.msi"))
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
            using (var p = CreatePipeline(@"get-msipatchinfo -product '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msipatchsequence -product '{877EF582-78AF-4D84-888B-167FDC3BCC11}'"))
            {
                using (this.OverrideRegistry())
                {
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
