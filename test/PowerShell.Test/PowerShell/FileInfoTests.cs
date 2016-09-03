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

using System.Collections;
using System.IO;
using System.Management.Automation;
using Microsoft.Tools.WindowsInstaller.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual("823F806C8E20FC8A41A8FAAEA5999F00", hash.MSIHash);
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
