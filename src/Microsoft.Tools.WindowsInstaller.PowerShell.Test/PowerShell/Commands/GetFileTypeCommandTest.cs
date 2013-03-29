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
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileTypeCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileTypeCommandTest : CommandTestBase
    {
        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.Path")]
        public void PathTest()
        {
            // Now invoke the cmdlet and check the file type property.
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Enumerate only example.ms* files.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifiletype -path example.ms*"))
            {
                Collection<PSObject> objs = p.Invoke();

                CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Patch"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Transform"));
            }

            // Enumerate all files without a filter.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifiletype"))
            {
                Collection<PSObject> objs = p.Invoke();

                // Should have encountered errors copying the whole directory.
                Assert.AreNotEqual<int>(0, p.Error.Count);

                CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Patch"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Transform"));
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.PassThru"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.PassThru")]
        public void PassThruTest()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-childitem -filter example.ms* | get-msifiletype -passthru"))
            {
                Collection<PSObject> objs = p.Invoke();

                Assert.AreNotEqual<int>(0, objs.Count);
                Assert.IsInstanceOfType(objs[0].BaseObject, typeof(FileInfo));

                foreach (PSObject obj in objs)
                {
                    Assert.IsNotNull(obj.Properties["MSIFileType"]);
                    Assert.IsInstanceOfType(obj.Properties["MSIFileType"].Value, typeof(string));

                    FileInfo file = obj.BaseObject as FileInfo;
                    switch (file.Extension)
                    {
                        case ".msi":
                            Assert.AreEqual<string>("Package", (string)obj.Properties["MSIFileType"].Value);
                            break;

                        case ".msp":
                            Assert.AreEqual<string>("Patch", (string)obj.Properties["MSIFileType"].Value);
                            break;

                        case ".mst":
                            Assert.AreEqual<string>("Transform", (string)obj.Properties["MSIFileType"].Value);
                            break;

                        default:
                            Assert.Fail("Unexpected extension {0}", file.Extension);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.LiteralPath"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.LiteralPath")]
        public void LiteralPathTest()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Test that a wildcard is not accepted.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifiletype -literalpath example.*"))
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
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-childitem hkcu:\software | get-msifiletype"))
            {
                Collection<PSObject> objs = p.Invoke();
                Assert.AreNotEqual<int>(0, p.Error.Count);
            }

            // Test against example.msi specifically.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msifiletype -literalpath example.msi"))
            {
                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
            }
        }
    }
}
