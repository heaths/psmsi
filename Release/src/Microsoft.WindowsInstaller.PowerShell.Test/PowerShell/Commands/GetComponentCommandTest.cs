// Unit test class for the get-wicomponentinfo cmdlet.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetComponentCommand"/>.
    ///</summary>
    [TestClass]
    public class GetComponentCommandTest : CommandTestBase
    {
        [TestMethod]
        [Description("A test for GetComponentCommand.ProcessRecord")]
        public void EnumerateAllComponents()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wicomponentinfo"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(46, objs.Count);
                }
            }
        }

        [TestMethod]
        [Description("A test for GetComponentCommand.ProcessRecord")]
        public void EnumerateClients()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wicomponentinfo '{E7F56051-B133-4702-A5C6-D8C192C04D5F}'"))
            {
                Runspace.DefaultRunspace = p.Runspace;
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value.ToString());
                }
            }
        }

        [TestMethod]
        [Description("A test for GetComponentCommand.ProcessRecord")]
        public void EnumerateProductComponents()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wicomponentinfo '{E7F56051-B133-4702-A5C6-D8C192C04D5F}', '{CB473DC3-F7BA-4E5B-9721-72CF66BC5262}' '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}'"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(2, objs.Count);

                    Collection<string> expected = new Collection<string>();
                    expected.Add(@"{E7F56051-B133-4702-A5C6-D8C192C04D5F}");
                    expected.Add(@"{CB473DC3-F7BA-4E5B-9721-72CF66BC5262}");

                    Collection<string> actual = new Collection<string>();
                    foreach (PSObject obj in objs)
                    {
                        actual.Add(obj.Properties["ComponentCode"].Value.ToString());
                    }

                    CollectionAssert.AreEquivalent(expected, actual);
                }
            }
        }
    }
}
