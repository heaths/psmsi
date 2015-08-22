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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="PatchSequencer"/> class.
    /// </summary>
    [TestClass]
    public sealed class PatchSequencerTests : TestBase
    {
        [TestMethod]
        public void SequenceValidPatchOnly()
        {
            var sequencer = new PatchSequencer();
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msp");

            // Add a valid patch.
            Assert.IsTrue(sequencer.Add(path, true), "Failed to add a valid patch to the sequencer.");
            Assert.AreEqual<int>(1, sequencer.Patches.Count, "The number of added patches is incorrect.");
            Assert.AreEqual<string>(path, sequencer.Patches[0], "The added patch path is incorrect.");

            path = Path.Combine(this.TestContext.DeploymentDirectory, "Applicable.xml");

            // Try to add patch XML.
            Assert.IsFalse(sequencer.Add(path, true), "Should not have added patch XML to the sequencer.");

            Assert.AreEqual<int>(1, sequencer.TargetProductCodes.Count, "The number of target ProductCodes is incorrect.");
            Assert.AreEqual<string>("{877EF582-78AF-4D84-888B-167FDC3BCC11}", sequencer.TargetProductCodes[0], "The target ProductCode is incorrect.");

            // Sequence the patch.
            path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            var e = sequencer.GetApplicablePatches(path).Select(patch => patch.Patch);
            Assert.IsNotNull(e, "The list of applicable patches is null.");

            var list = e.ToList();
            Assert.AreEqual<int>(1, list.Count, "The number of applicable patches is incorrect.");
            CollectionAssert.AreEqual(sequencer.Patches, list.ToArray(), "The list of applicable patches is incorrect.");
        }

        [TestMethod]
        public void SequencePatchXml()
        {
            var sequencer = new PatchSequencer();
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Applicable.xml");

            // Add a valid patch XML file.
            Assert.IsTrue(sequencer.Add(path), "Failed to add a valid patch XML file to the sequencer.");
            Assert.AreEqual<int>(1, sequencer.Patches.Count, "The number of added patches is incorrect.");
            Assert.AreEqual<string>(path, sequencer.Patches[0], "The added patch path is incorrect.");

            Assert.AreEqual<int>(1, sequencer.TargetProductCodes.Count, "The number of target ProductCodes is incorrect.");
            Assert.AreEqual<string>("{877EF582-78AF-4D84-888B-167FDC3BCC11}", sequencer.TargetProductCodes[0], "The target ProductCode is incorrect.");

            // Sequence the patch.
            path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            var e = sequencer.GetApplicablePatches(path).Select(patch => patch.Patch);
            Assert.IsNotNull(e, "The list of applicable patches is null.");

            var list = e.ToList();
            Assert.AreEqual<int>(1, list.Count, "The number of applicable patches is incorrect.");
            CollectionAssert.AreEqual(sequencer.Patches, list.ToArray(), "The list of applicable patches is incorrect.");
        }

        [TestMethod]
        public void SequenceInapplicablePatch()
        {
            var sequencer = new PatchSequencer();
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Inapplicable.xml");
            Assert.IsTrue(sequencer.Add(path), "Failed to add a valid patch file to the sequencer.");

            var inapplicable = new List<string>();
            path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");

            sequencer.InapplicablePatch += (source, args) =>
                {
                    Assert.AreEqual<string>(path, args.Product, "The target product is incorrect.");
                    inapplicable.Add(args.Patch);
                };

            var e = sequencer.GetApplicablePatches(path).Select(patch => patch.Patch);
            Assert.IsNotNull(e, "The list of applicable patches is null.");

            var applicable = e.ToList();
            Assert.AreEqual<int>(0, applicable.Count, "The number of applicable patches is incorrect.");
            Assert.AreEqual<int>(1, inapplicable.Count, "The number of inapplicable patches is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SequenceWithNullPath()
        {
            var sequencer = new PatchSequencer();
            sequencer.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SequenceWithMissingFile()
        {
            var sequencer = new PatchSequencer();
            sequencer.Add("Missing.msp");
        }
    }
}
