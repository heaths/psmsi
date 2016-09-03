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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileHashCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileHashCommandTest : TestBase
    {
        [TestMethod]
        public void PathTest()
        {
            // Test a file using new property names.
            using (var p = CreatePipeline(@"get-msifilehash -path *.txt"))
            {
                int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                var objs = p.Invoke();

                Assert.AreEqual<int>(2, objs.Count);

                var obj = objs.Where(o => o.GetPropertyValue<int>("MSIHashPart1") == hash[0]).FirstOrDefault();
                Assert.IsNotNull(obj);
                Assert.AreEqual("823F806C8E20FC8A41A8FAAEA5999F00", obj.GetPropertyValue<string>("MSIHash"));
                Assert.AreEqual<int>(hash[0], obj.GetPropertyValue<int>("MSIHashPart1"));
                Assert.AreEqual<int>(hash[1], obj.GetPropertyValue<int>("MSIHashPart2"));
                Assert.AreEqual<int>(hash[2], obj.GetPropertyValue<int>("MSIHashPart3"));
                Assert.AreEqual<int>(hash[3], obj.GetPropertyValue<int>("MSIHashPart4"));
            }

            // Test with no parameter.
            using (var p = CreatePipeline(@"get-msifilehash"))
            {
                var objs = p.Invoke();

                Assert.AreNotEqual<int>(0, objs.Count);
                CollectionAssert.AllItemsAreUnique(objs);
            }
        }

        [TestMethod]
        public void PassThruTest()
        {
            // Test against a file using new property names.
            using (var p = CreatePipeline(@"get-childitem -path example.txt | get-msifilehash -passthru"))
            {
                var hash = new List<PSObject>(
                    from i in new int[] { 1820344194, -1963188082, -1359304639, 10459557 }
                    select PSObject.AsPSObject(i)
                );

                var objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.IsInstanceOfType(objs[0].BaseObject, typeof(System.IO.FileInfo));
                Assert.AreEqual<PSObject>("823F806C8E20FC8A41A8FAAEA5999F00", objs[0].GetPropertyValue<PSObject>("MSIHash"));
                Assert.AreEqual<PSObject>(hash[0], objs[0].GetPropertyValue<PSObject>("MSIHashPart1"));
                Assert.AreEqual<PSObject>(hash[1], objs[0].GetPropertyValue<PSObject>("MSIHashPart2"));
                Assert.AreEqual<PSObject>(hash[2], objs[0].GetPropertyValue<PSObject>("MSIHashPart3"));
                Assert.AreEqual<PSObject>(hash[3], objs[0].GetPropertyValue<PSObject>("MSIHashPart4"));
            }

            // Test against a directory using new property names.
            using (var p = CreatePipeline(@"get-msifilehash -path . -passthru"))
            {
                var objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.IsNull(objs[0].Properties["MSIHash"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart1"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart2"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart3"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart4"].Value);
            }
        }

        [TestMethod]
        public void LiteralPathTest()
        {
            // Test that a wildcard is not accepted.
            using (var p = CreatePipeline(@"get-msifilehash -literalpath *.txt"))
            {
                Collection<PSObject> objs = null;
                try
                {
                    // Wrapped in a try-catch since the behavior changedin PSv3.
                    objs = p.Invoke();
                }
                catch { }

                Assert.AreEqual<int>(0, objs.Count);
            }

            // Test that a registry item path is not accepted.
            using (var p = CreatePipeline(@"get-childitem hkcu:\software | get-msifilehash"))
            {
                var objs = p.Invoke();
                Assert.AreNotEqual<int>(0, p.Error.Count);
            }
        }
    }
}
