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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="ExportPatchXmlCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class ExportPatchXmlCommandTests : TestBase
    {
        [TestMethod]
        public void ExtractPatchFileXml()
        {
            using (var p = CreatePipeline(@"export-msipatchxml example.msp ""$TestRunDirectory\patchfilexml.xml"""))
            {
                p.Invoke();

                var path = Path.Combine(this.TestContext.TestRunDirectory, "patchfilexml.xml");
                Assert.IsTrue(File.Exists(path), "The output file does not exist.");

                // Check that the default encoding is correct.
                using (var file = File.OpenText(path))
                {
                    Assert.AreEqual<Encoding>(Encoding.UTF8, file.CurrentEncoding, "The encoding is incorreect.");
                }
            }
        }

        [TestMethod]
        public void ExtractPatchFileFormattedXml()
        {
            using (var p = CreatePipeline(@"export-msipatchxml example.msp ""$TestRunDirectory\patchfileformattedxml.xml"" -encoding Unicode -formatted"))
            {
                p.Invoke();

                var path = Path.Combine(this.TestContext.TestRunDirectory, "patchfileformattedxml.xml");
                Assert.IsTrue(File.Exists(path), "The output file does not exist.");

                // Make sure the file contains tabs.
                using (var file = File.OpenText(path))
                {
                    var xml = file.ReadToEnd();
                    StringAssert.Contains(xml, "\t", "The file does not contain tabs.");
                    Assert.AreEqual<Encoding>(Encoding.Unicode, file.CurrentEncoding, "The encoding is incorrect.");
                }
            }
        }
    }
}
