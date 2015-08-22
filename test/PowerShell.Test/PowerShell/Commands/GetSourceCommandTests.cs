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
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Functional tests for <see cref="GetSourceCommand"/>.
    /// </summary>
    [TestClass]
    public class GetSourceCommandTests : TestBase
    {
        [TestMethod]
        public void GetProductSource()
        {
            var expected = @"c:\2014be2e43e417a3b9\";
            using (var p = CreatePipeline("get-msisource '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}'"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();

                    Assert.IsNotNull(output);
                    Assert.AreEqual<int>(1, output.Count());

                    var actual = output.FirstOrDefault().As<SourceInfo>();
                    Assert.IsNotNull(actual);
                    Assert.AreEqual("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", actual.ProductCode);
                    Assert.AreEqual(expected, actual.Path, true);
                }
            }
        }

        [TestMethod]
        public void GetProductSourceFromPipeline()
        {
            var expected = @"c:\2014be2e43e417a3b9\";
            using (var p = CreatePipeline("get-msiproductinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' | get-msisource"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();

                    Assert.IsNotNull(output);
                    Assert.AreEqual<int>(1, output.Count());

                    var actual = output.FirstOrDefault().As<SourceInfo>();
                    Assert.IsNotNull(actual);
                    Assert.AreEqual("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", actual.ProductCode);
                    Assert.AreEqual(expected, actual.Path, true);
                }
            }
        }

        [TestMethod]
        public void GetCorruptProductSource()
        {
            using (var p = CreatePipeline("get-msisource '{9AC08E99-230B-47e8-9721-4577B7F124EA}'"))
            {
                var path = Path.Combine(base.TestContext.DeploymentDirectory, "Corrupt.xml");
                using (OverrideRegistry(path))
                {
                    var output = p.Invoke();

                    Assert.IsNotNull(output);
                    Assert.AreEqual<int>(0, output.Count());
                    Assert.AreEqual<int>(0, p.Output.Count);
                    Assert.AreEqual<int>(1, p.Error.Count);
                }
            }
        }

        [TestMethod]
        public void GetPatchSource()
        {
            var expected = @"c:\updates\";
            using (var p = CreatePipeline("get-msisource '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' -patchcode '{6E52C409-0D0D-4B84-AB63-463438D4D33B}'"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();

                    Assert.IsNotNull(output);
                    Assert.AreEqual<int>(1, output.Count());

                    var actual = output.FirstOrDefault().As<PatchSourceInfo>();
                    Assert.IsNotNull(actual);
                    Assert.AreEqual("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", actual.ProductCode);
                    Assert.AreEqual("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", actual.PatchCode);
                    Assert.AreEqual(expected, actual.Path, true);
                }
            }
        }

        [TestMethod]
        public void GetPatchSourceFromPipeline()
        {
            var expected = @"c:\updates\";
            using (var p = CreatePipeline("get-msipatchinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' -patchcode '{6E52C409-0D0D-4B84-AB63-463438D4D33B}' | get-msisource"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();

                    Assert.IsNotNull(output);
                    Assert.AreEqual<int>(1, output.Count());

                    var actual = output.FirstOrDefault().As<PatchSourceInfo>();
                    Assert.IsNotNull(actual);
                    Assert.AreEqual("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", actual.ProductCode);
                    Assert.AreEqual("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", actual.PatchCode);
                    Assert.AreEqual(expected, actual.Path, true);
                }
            }
        }
    }
}
