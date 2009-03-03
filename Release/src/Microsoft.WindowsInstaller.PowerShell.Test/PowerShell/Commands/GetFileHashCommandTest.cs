// Unit test class for the Get-MSIFileType cmdlet.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileHashCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileHashCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetFileHashCommand));
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.Path")]
        [DeploymentItem(@"data\example.txt")]
        public void PathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                // Test a file using new property names.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path *.txt"))
                {
                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };
                    
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<int>(hash[0], (int)objs[0].Properties["WIHashPart1"].Value);
                    Assert.AreEqual<int>(hash[1], (int)objs[0].Properties["WIHashPart2"].Value);
                    Assert.AreEqual<int>(hash[2], (int)objs[0].Properties["WIHashPart3"].Value);
                    Assert.AreEqual<int>(hash[3], (int)objs[0].Properties["WIHashPart4"].Value);
                }

                // Test with no parameter.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreNotEqual<int>(0, objs.Count);
                    CollectionAssert.AllItemsAreUnique(objs);
                }

                // Test against a directory.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path ."))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(0, objs.Count);
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.PassThru"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.PassThru")]
        [DeploymentItem(@"data\example.txt")]
        public void PassThruTest()
        {
            // Now invoke the cmdlet and check the file hash properties.
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                // Test against a file using new property names.
                using (Pipeline p = rs.CreatePipeline(@"get-childitem -path example.txt | get-msifilehash -passthru"))
                {
                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.IsInstanceOfType(objs[0].BaseObject, typeof(System.IO.FileInfo));
                    Assert.AreEqual<int>(hash[0], (int)objs[0].Properties["WIHashPart1"].Value);
                    Assert.AreEqual<int>(hash[1], (int)objs[0].Properties["WIHashPart2"].Value);
                    Assert.AreEqual<int>(hash[2], (int)objs[0].Properties["WIHashPart3"].Value);
                    Assert.AreEqual<int>(hash[3], (int)objs[0].Properties["WIHashPart4"].Value);
                }

                // Test against a directory using new property names.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path . -passthru"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.IsNull(objs[0].Properties["WIHashPart1"].Value);
                    Assert.IsNull(objs[0].Properties["WIHashPart2"].Value);
                    Assert.IsNull(objs[0].Properties["WIHashPart3"].Value);
                    Assert.IsNull(objs[0].Properties["WIHashPart4"].Value);
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.LiteralPath"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.LiteralPath")]
        [DeploymentItem(@"data\example.txt")]
        public void LiteralPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                // Test that a wildcard is not accepted.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -literalpath *.txt"))
                {
                    TestProject.ExpectException(typeof(CmdletProviderInvocationException), typeof(ArgumentException), delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Test that a registry item path is not accepted.
                using (Pipeline p = rs.CreatePipeline(@"get-childitem hkcu:\software | get-msifilehash"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreNotEqual<int>(0, p.Error.Count);
                }

                // Test a file using new property names.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path example.txt"))
                {
                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<int>(hash[0], (int)objs[0].Properties["WIHashPart1"].Value);
                    Assert.AreEqual<int>(hash[1], (int)objs[0].Properties["WIHashPart2"].Value);
                    Assert.AreEqual<int>(hash[2], (int)objs[0].Properties["WIHashPart3"].Value);
                    Assert.AreEqual<int>(hash[3], (int)objs[0].Properties["WIHashPart4"].Value);
                }
            }
        }
    }
}
