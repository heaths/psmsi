// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Tools.WindowsInstaller.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Tests for the <see cref="FileInfo"/> class.
    /// </summary>
    [TestClass]
    public sealed class FileInfoTests : TestBase
    {
        [TestMethod]
        public void GetFileTypeNull()
        {
            var type = FileInfo.GetFileType(null);
            Assert.IsNull(type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileTypeDirectory()
        {
            var dir = new DirectoryInfo(this.TestContext.DeploymentDirectory);
            var obj = PSObject.AsPSObject(dir);
            var type = FileInfo.GetFileType(obj);

            Assert.IsNull(type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileTypeMsi()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.msi");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var type = FileInfo.GetFileType(obj);

            Assert.AreEqual(Resources.Type_Package, type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileTypeMsp()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.msp");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var type = FileInfo.GetFileType(obj);

            Assert.AreEqual(Resources.Type_Patch, type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileTypeMst()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.mst");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var type = FileInfo.GetFileType(obj);

            Assert.AreEqual(Resources.Type_Transform, type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileTypeTxt()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.txt");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var type = FileInfo.GetFileType(obj);

            Assert.IsNull(type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileTypeMissing()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "doesnotexist.txt");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var type = FileInfo.GetFileType(obj);

            Assert.IsNull(type, "The file type is incorrect.");
        }

        [TestMethod]
        public void GetFileHashNull()
        {
            var hash = FileInfo.GetFileHash(null);
            Assert.IsNull(hash, "The file hash is incorrect.");
        }

        [TestMethod]
        public void GetFileHashDirectory()
        {
            var dir = new DirectoryInfo(this.TestContext.DeploymentDirectory);
            var obj = PSObject.AsPSObject(dir);
            var hash = FileInfo.GetFileHash(obj);

            Assert.IsNull(hash, "The file hash is incorrect.");
        }

        [TestMethod]
        public void GetFileHashFile()
        {
            var expected = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.txt");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var hash = FileInfo.GetFileHash(obj);

            CollectionAssert.AreEqual(expected, hash.ToArray(), "The file hash is incorrect.");
        }

        [TestMethod]
        public void GetFileHashMissing()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "doesnotexist.txt");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var hash = FileInfo.GetFileHash(obj);

            Assert.IsNull(hash, "The file hash is incorrect.");
        }

        [TestMethod]
        public void GetFileHashEnumerable()
        {
            var expected = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

            var path = Path.Combine(this.TestContext.DeploymentDirectory, "example.txt");
            var file = new System.IO.FileInfo(path);
            var obj = PSObject.AsPSObject(file);
            var hash = FileInfo.GetFileHash(obj);

            int i = 0;
            var e = ((IEnumerable)hash).GetEnumerator();

            foreach (var part in hash)
            {
                // Let exceptions throw and fail the test.
                Assert.AreEqual<int>(expected[i++], part, "The file hash part is incorrect.");

                Assert.IsTrue(e.MoveNext(), "Could not advance the enumerator.");
                Assert.AreEqual(part, e.Current, "The file hash parts are not equivalent.");
            }

            Assert.AreEqual<int>(4, i, "The number of file hash parts is incorrect.");
        }
    }
}
