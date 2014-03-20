// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
            Assert.AreEqual<int>(1419, weight);
        }

        [TestMethod]
        public void PackageWeightFromProductCodeTest()
        {
            using (OverrideRegistry())
            {
                var weight = PackageInfo.GetWeightFromProductCode("{877EF582-78AF-4D84-888B-167FDC3BCC11}");
                Assert.AreEqual<int>(1419, weight);
            }
        }
    }
}
