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
    /// Unit and functional tests for <see cref="GetFileTypeCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileTypeCommandTest
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
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSIFileType", typeof(GetFileTypeCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.Path")]
        [DeploymentItem(@"data")]
        public void PathTest()
        {
            // First just check the property, increasing code coverage.
            GetFileTypeCommand cmdlet = new GetFileTypeCommand();
            cmdlet.PassThru = true;
            Assert.IsTrue(cmdlet.PassThru);

            // Now invoke the cmdlet and check the file type property.
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -path example.*"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    foreach (string value in new string[] { "Package", "Patch", "Transform" })
                    {
                        Assert.IsTrue(objs.Contains(new PSObject(value)));
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.PassThru"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.PassThru")]
        [DeploymentItem(@"data")]
        public void PassThruTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-childitem -path example.* | get-msifiletype -passthru"))
                {
                    // Make sure we detect all supported file types.
                    bool msi, msp, mst, other;
                    msi = msp = mst = other = false;

                    Collection<PSObject> objs = p.Invoke();
                    foreach (PSObject obj in objs)
                    {
                        Assert.IsInstanceOfType(obj.BaseObject, typeof(FileInfo));
                        FileInfo info = (FileInfo)obj.BaseObject;

                        PSPropertyInfo type = obj.Properties["MSIFileType"];
                        Assert.IsNotNull(type);

                        switch (info.Extension)
                        {
                            case ".msi":
                                Assert.AreEqual<string>("Package", type.Value.ToString());
                                msi = true;
                                break;

                            case ".msp":
                                Assert.AreEqual<string>("Patch", type.Value.ToString());
                                msp = true;
                                break;

                            case ".mst":
                                Assert.AreEqual<string>("Transform", type.Value.ToString());
                                mst = true;
                                break;

                            default:
                                Assert.IsNull(type.Value);
                                other = true;
                                break;
                        }

                    }
                    
                    Assert.IsTrue(msi && msp && mst && other, "Not all supported file types were detected.");
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.LiteralPath"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.LiteralPath")]
        [DeploymentItem(@"data\example.txt")]
        public void LiteralPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -literalpath example.*"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(0, objs.Count);
                }
            }
        }
    }
}
