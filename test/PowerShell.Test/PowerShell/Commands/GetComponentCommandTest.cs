// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetComponentCommand"/>.
    ///</summary>
    [TestClass]
    public class GetComponentCommandTest : TestBase
    {
        [TestMethod]
        public void EnumerateAllComponents()
        {
            using (var p = CreatePipeline(@"get-msicomponentinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();
                    Assert.AreEqual<int>(36, objs.Count);
                }
            }
        }

        [TestMethod]
        public void EnumerateClients()
        {
            using (var p = CreatePipeline(@"get-msicomponentinfo '{E7F56051-B133-4702-A5C6-D8C192C04D5F}'"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].GetPropertyValue<string>("ProductCode"));
                }
            }
        }

        [TestMethod]
        public void EnumerateProductComponents()
        {
            using (var p = CreatePipeline(@"get-msicomponentinfo '{E7F56051-B133-4702-A5C6-D8C192C04D5F}', '{CB473DC3-F7BA-4E5B-9721-72CF66BC5262}' '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}'"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();
                    Assert.AreEqual<int>(2, objs.Count);

                    var expected = new Collection<string>();
                    expected.Add(@"{E7F56051-B133-4702-A5C6-D8C192C04D5F}");
                    expected.Add(@"{CB473DC3-F7BA-4E5B-9721-72CF66BC5262}");

                    var actual = new Collection<string>();
                    foreach (PSObject obj in objs)
                    {
                        actual.Add(obj.GetPropertyValue<string>("ComponentCode"));
                    }

                    CollectionAssert.AreEquivalent(expected, actual);
                }
            }
        }

        [TestMethod]
        [WorkItem(9464)]
        public void GetComponentChainedExecution()
        {
            using (var p = CreatePipeline(@"get-msicomponentinfo '{9D8E88E9-8E05-4FC7-AFC7-87759D1D417E}' | get-msicomponentinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    // Two shared components piped again yield 4 (duplicated).
                    Assert.AreEqual(4, objs.Count);
                }
            }
        }

        [TestMethod]
        public void ValidateFileComponentPath()
        {
            string keyPath = Path.Combine(this.TestContext.DeploymentDirectory, "return.exe");

            using (var p = CreatePipeline(@"get-msicomponentinfo '{958A3933-8CE7-6189-F0EF-CAE467FABFF4}'"))
            {
                using (OverrideRegistry())
                {
                    var obj = p.Invoke().FirstOrDefault();

                    Assert.IsNotNull(obj);
                    Assert.AreEqual(keyPath, obj.GetPropertyValue<string>("KeyPath"), true);
                    Assert.AreEqual(@"Microsoft.PowerShell.Core\FileSystem::" + keyPath, obj.GetPropertyValue<string>("Path"), true);
                    Assert.AreEqual(@"Microsoft.PowerShell.Core\FileSystem::" + keyPath, obj.GetPropertyValue<string>("PSPath"), true);
                }
            }
        }

        [TestMethod]
        public void ValidateRegistryComponentPath()
        {
            string keyPath = @"02:\SOFTWARE\Microsoft\Internet Explorer\Low Rights\ElevationPolicy\{003B91A6-61E3-4591-891D-01E94C8CB11E}\";
            string providerPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Low Rights\ElevationPolicy\{003B91A6-61E3-4591-891D-01E94C8CB11E}";

            using (var p = CreatePipeline(@"get-msicomponentinfo '{E2E70518-347A-4231-9D5D-3857059CCFA7}'"))
            {
                using (OverrideRegistry())
                {
                    var obj = p.Invoke().FirstOrDefault();

                    Assert.IsNotNull(obj);
                    Assert.AreEqual(keyPath, obj.GetPropertyValue<string>("KeyPath"), true);
                    Assert.AreEqual(@"Microsoft.PowerShell.Core\Registry::" + providerPath, obj.GetPropertyValue<string>("Path"), true);
                    Assert.AreEqual(@"Microsoft.PowerShell.Core\Registry::" + providerPath, obj.GetPropertyValue<string>("PSPath"), true);
                }
            }
        }
    }
}
