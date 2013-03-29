// Unit test class for the Get-MSIFileType cmdlet.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileHashCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileHashCommandTest : CommandTestBase
    {
        /// <summary>
        /// A test for <see cref="GetFileHashCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.Path")]
        public void PathTest()
        {
                // Test a file using new property names.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifilehash -path *.txt"))
            {
                int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };
                
                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<int>(hash[0], (int)objs[0].Properties["MSIHashPart1"].Value);
                Assert.AreEqual<int>(hash[1], (int)objs[0].Properties["MSIHashPart2"].Value);
                Assert.AreEqual<int>(hash[2], (int)objs[0].Properties["MSIHashPart3"].Value);
                Assert.AreEqual<int>(hash[3], (int)objs[0].Properties["MSIHashPart4"].Value);
            }

            // Test with no parameter.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifilehash"))
            {
                Collection<PSObject> objs = p.Invoke();

                Assert.AreNotEqual<int>(0, objs.Count);
                CollectionAssert.AllItemsAreUnique(objs);
            }

            // Test against a directory.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifilehash -path ."))
            {
                Collection<PSObject> objs = p.Invoke();
                Assert.AreEqual<int>(0, objs.Count);
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.PassThru"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.PassThru")]
        public void PassThruTest()
        {
            // Test against a file using new property names.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-childitem -path example.txt | get-msifilehash -passthru"))
            {
                int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.IsInstanceOfType(objs[0].BaseObject, typeof(System.IO.FileInfo));
                Assert.AreEqual<int>(hash[0], (int)objs[0].Properties["MSIHashPart1"].Value);
                Assert.AreEqual<int>(hash[1], (int)objs[0].Properties["MSIHashPart2"].Value);
                Assert.AreEqual<int>(hash[2], (int)objs[0].Properties["MSIHashPart3"].Value);
                Assert.AreEqual<int>(hash[3], (int)objs[0].Properties["MSIHashPart4"].Value);
            }

            // Test against a directory using new property names.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifilehash -path . -passthru"))
            {
                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.IsNull(objs[0].Properties["MSIHashPart1"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart2"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart3"].Value);
                Assert.IsNull(objs[0].Properties["MSIHashPart4"].Value);
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.LiteralPath"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.LiteralPath")]
        public void LiteralPathTest()
        {
            // Test that a wildcard is not accepted.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifilehash -literalpath *.txt"))
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
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-childitem hkcu:\software | get-msifilehash"))
            {
                Collection<PSObject> objs = p.Invoke();
                Assert.AreNotEqual<int>(0, p.Error.Count);
            }

            // Test a file using new property names.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifilehash -path example.txt"))
            {
                int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<int>(hash[0], (int)objs[0].Properties["MSIHashPart1"].Value);
                Assert.AreEqual<int>(hash[1], (int)objs[0].Properties["MSIHashPart2"].Value);
                Assert.AreEqual<int>(hash[2], (int)objs[0].Properties["MSIHashPart3"].Value);
                Assert.AreEqual<int>(hash[3], (int)objs[0].Properties["MSIHashPart4"].Value);
            }
        }
    }
}
