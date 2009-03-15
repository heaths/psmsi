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
    public class GetComponentCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetComponentCommand));
        }

        /// <summary>
        /// A test for <see cref="GetComponentCommand.ComponentCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetComponentCommand.ComponentCode")]
        public void ComponentCodeTest()
        {
            // First just set and get the ProductCode property value to increase code coverage.
            GetComponentCommand cmdlet = new GetComponentCommand();
            cmdlet.ComponentCode = new string[] { "{E7F56051-B133-4702-A5C6-D8C192C04D5F}", "{8633DDE2-53C9-4481-ACE7-AB6198BFC728}" };

            Assert.AreEqual<int>(2, cmdlet.ComponentCode.Length);
            CollectionAssert.AreEquivalent(
                new string[] { "{8633DDE2-53C9-4481-ACE7-AB6198BFC728}", "{E7F56051-B133-4702-A5C6-D8C192C04D5F}" },
                cmdlet.ComponentCode);
        }

        /// <summary>
        /// A test for <see cref="GetComponentCommand.ProductCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetComponentCommand.ProductCode")]
        public void ProductCodeTest()
        {
            // First just set and get the ProductCode property value to increase code coverage.
            GetComponentCommand cmdlet = new GetComponentCommand();
            cmdlet.ProductCode = "{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}";
            Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", cmdlet.ProductCode);
        }

        [TestMethod]
        [Description("A test for GetComponentCommand.ProcessRecord")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateAllComponents()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                using (Pipeline p = rs.CreatePipeline(@"get-wicomponentinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(42, objs.Count);
                    }
                }
            }
        }

        [TestMethod]
        [Description("A test for GetComponentCommand.ProcessRecord")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateClients()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                Runspace.DefaultRunspace = rs;

                using (Pipeline p = rs.CreatePipeline(@"get-wicomponentinfo '{E7F56051-B133-4702-A5C6-D8C192C04D5F}'"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value.ToString());
                    }
                }
            }
        }

        [TestMethod]
        [Description("A test for GetComponentCommand.ProcessRecord")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductComponents()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                using (Pipeline p = rs.CreatePipeline(@"get-wicomponentinfo '{E7F56051-B133-4702-A5C6-D8C192C04D5F}', '{CB473DC3-F7BA-4E5B-9721-72CF66BC5262}' '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}'"))
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
                            actual.Add(obj.Properties["ComponentCode"].Value as string);
                        }

                        CollectionAssert.AreEquivalent(expected, actual);
                    }
                }
            }
        }
    }
}
