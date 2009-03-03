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
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileTypeCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileTypeCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetFileTypeCommand));
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.Path")]
        [DeploymentItem(@"data")]
        public void PathTest()
        {
            // Now invoke the cmdlet and check the file type property.
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                // Enumerate only example.ms* files.
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -path example.ms*"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Patch"));
                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Transform"));
                }

                // Enumerate all files without a filter.
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    // Should have encountered errors copying the whole directory.
                    Assert.AreNotEqual<int>(0, p.Error.Count);

                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Patch"));
                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Transform"));
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
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                using (Pipeline p = rs.CreatePipeline(@"get-childitem -filter example.ms* | get-msifiletype -passthru"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreNotEqual<int>(0, objs.Count);
                    Assert.IsInstanceOfType(objs[0].BaseObject, typeof(FileInfo));

                    foreach (PSObject obj in objs)
                    {
                        Assert.IsNotNull(obj.Properties["WIFileType"]);
                        Assert.IsInstanceOfType(obj.Properties["WIFileType"].Value, typeof(string));

                        FileInfo file = obj.BaseObject as FileInfo;
                        switch (file.Extension)
                        {
                            case ".msi":
                                Assert.AreEqual<string>("Package", (string)obj.Properties["WIFileType"].Value);
                                break;

                            case ".msp":
                                Assert.AreEqual<string>("Patch", (string)obj.Properties["WIFileType"].Value);
                                break;

                            case ".mst":
                                Assert.AreEqual<string>("Transform", (string)obj.Properties["WIFileType"].Value);
                                break;

                            default:
                                Assert.Fail("Unexpected extension {0}", file.Extension);
                                break;
                        }
                    }
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
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                // Test that a wildcard is not accepted.
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -literalpath example.*"))
                {
                    TestProject.ExpectException(typeof(CmdletProviderInvocationException), typeof(ArgumentException), delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Test that a registry item path is not accepted.
                using (Pipeline p = rs.CreatePipeline(@"get-childitem hkcu:\software | get-msifiletype"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreNotEqual<int>(0, p.Error.Count);
                }

                // Test against example.msi specifically.
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -literalpath example.msi"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                }
            }
        }
    }
}
