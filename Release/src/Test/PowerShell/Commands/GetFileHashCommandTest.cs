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
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Windows.Installer.PowerShell.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    
    
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileHashCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileHashCommandTest
    {
        private TestContext testContext;
        private RunspaceConfiguration config;

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            config = RunspaceConfiguration.Create();
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSIFileHash", typeof(GetFileHashCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.Path")]
        [DeploymentItem(@"data\example.txt")]
        public void PathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                // Test a file.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path example.txt"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);

                    // Cast to a FileHashInfo object to increase code coverage.
                    PSObject obj = objs[0];
                    Assert.IsInstanceOfType(obj.BaseObject, typeof(FileHashInfo));

                    FileHashInfo info = (FileHashInfo)obj.BaseObject;
                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                    Assert.AreEqual<int>(hash[0], info.HashPart1.Value);
                    Assert.AreEqual<int>(hash[1], info.HashPart2.Value);
                    Assert.AreEqual<int>(hash[2], info.HashPart3.Value);
                    Assert.AreEqual<int>(hash[3], info.HashPart4.Value);
                }

                // Test against a directory.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path ."))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);

                    // Cast to a FileHashInfo object to increase code coverage.
                    PSObject obj = objs[0];
                    Assert.IsInstanceOfType(obj.BaseObject, typeof(FileHashInfo));

                    FileHashInfo info = (FileHashInfo)obj.BaseObject;
                    Assert.IsFalse(info.HashPart1.HasValue);
                    Assert.IsFalse(info.HashPart2.HasValue);
                    Assert.IsFalse(info.HashPart3.HasValue);
                    Assert.IsFalse(info.HashPart4.HasValue);
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
            // First just check the property, increasing code coverage.
            GetFileHashCommand cmdlet = new GetFileHashCommand();
            cmdlet.PassThru = true;
            Assert.IsTrue(cmdlet.PassThru);

            // Now invoke the cmdlet and check the file hash properties.
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-childitem -path example.txt | get-msifilehash -passthru"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);

                    PSObject obj = objs[0];
                    Assert.IsInstanceOfType(obj.BaseObject, typeof(FileInfo));

                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };

                    PSPropertyInfo prop1 = obj.Properties["MSIHashPart1"];
                    PSPropertyInfo prop2 = obj.Properties["MSIHashPart2"];
                    PSPropertyInfo prop3 = obj.Properties["MSIHashPart3"];
                    PSPropertyInfo prop4 = obj.Properties["MSIHashPart4"];

                    Assert.IsNotNull(prop1);
                    Assert.IsNotNull(prop2);
                    Assert.IsNotNull(prop3);
                    Assert.IsNotNull(prop4);

                    Assert.AreEqual<int>(hash[0], (int)prop1.Value);
                    Assert.AreEqual<int>(hash[1], (int)prop2.Value);
                    Assert.AreEqual<int>(hash[2], (int)prop3.Value);
                    Assert.AreEqual<int>(hash[3], (int)prop4.Value);
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
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -literalpath example.*"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(0, objs.Count);
                }
            }
        }
    }
}
