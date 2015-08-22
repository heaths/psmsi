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

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="PackageInfo"/> class.
    /// </summary>
    [TestClass]
    public class PackageInfoTests : TestBase
    {
        [TestMethod]
        public void PackageWeightFromPathTest()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.msi");

            var weight = PackageInfo.GetWeightFromPath(path);
            Assert.AreEqual<long>(1419, weight);
        }

        [TestMethod]
        public void PackageWeightFromProductCodeTest()
        {
            using (OverrideRegistry())
            {
                var weight = PackageInfo.GetWeightFromProductCode("{877EF582-78AF-4D84-888B-167FDC3BCC11}");
                Assert.AreEqual<long>(1419, weight);
            }
        }

        [TestMethod]
        public void PackageWeightFromFileSizeTest()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "noweight.msi");

            var weight = PackageInfo.GetWeightFromPath(path);
            Assert.AreEqual<long>(24576, weight);
        }
    }
}
