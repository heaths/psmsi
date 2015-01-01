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
