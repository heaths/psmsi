// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Threading;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileTypeCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileTypeCommandTest : TestBase
    {
        [TestMethod]
        public void PathTest()
        {
            // Now invoke the cmdlet and check the file type property.
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Enumerate only example.ms* files.
            using (var p = CreatePipeline(@"get-msifiletype -path example.ms*"))
            {
                var objs = p.Invoke();

                CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Patch"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Transform"));
            }

            // Enumerate all files without a filter.
            using (var p = CreatePipeline(@"get-msifiletype"))
            {
                var objs = p.Invoke();

                CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Patch"));
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Transform"));
            }
        }

        [TestMethod]
        public void PassThruTest()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            using (var p = CreatePipeline(@"get-childitem -filter example.ms* | get-msifiletype -passthru"))
            {
                var objs = p.Invoke();

                Assert.AreNotEqual<int>(0, objs.Count);
                Assert.IsInstanceOfType(objs[0].BaseObject, typeof(System.IO.FileInfo));

                foreach (var obj in objs)
                {
                    Assert.IsNotNull(obj.Properties["MSIFileType"]);

                    var file = obj.BaseObject as System.IO.FileInfo;
                    switch (file.Extension)
                    {
                        case ".msi":
                            Assert.AreEqual<string>("Package", obj.GetPropertyValue<string>("MSIFileType"));
                            break;

                        case ".msp":
                            Assert.AreEqual<string>("Patch", obj.GetPropertyValue<string>("MSIFileType"));
                            break;

                        case ".mst":
                            Assert.AreEqual<string>("Transform", obj.GetPropertyValue<string>("MSIFileType"));
                            break;

                        default:
                            Assert.Fail("Unexpected extension {0}", file.Extension);
                            break;
                    }
                }
            }
        }

        [TestMethod]
        public void LiteralPathTest()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Test that a wildcard is not accepted.
            using (var p = CreatePipeline(@"get-msifiletype -literalpath example.*"))
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
            using (var p = CreatePipeline(@"get-childitem hkcu:\software | get-msifiletype"))
            {
                var objs = p.Invoke();
                Assert.AreNotEqual<int>(0, p.Error.Count);
            }

            // Test against example.msi specifically.
            using (var p = CreatePipeline(@"get-msifiletype -literalpath example.msi"))
            {
                var objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                CollectionAssert.Contains(objs, PSObject.AsPSObject("Package"));
            }
        }
    }
}
