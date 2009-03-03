// Unit test class for the get-msiproductinfo cmdlet.
//
// Author: Heath Stewart
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
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.WindowsInstaller.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetProductCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetProductCommand));
        }

        /// <summary>
        /// Enumerates all machine-assigned products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all machine-assigned products")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProducts()
        {
            List<string> products = new List<string>();
            products.Add("{0CABECAC-4E23-4928-871A-6E65CD370F9F}");
            products.Add("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");

            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        List<string> actual = new List<string>(objs.Count);
                        foreach (PSObject obj in objs)
                        {
                            actual.Add(obj.Properties["ProductCode"].Value as string);
                        }

                        Assert.AreEqual<int>(products.Count, objs.Count);
                        CollectionAssert.AreEquivalent(products, actual);
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all unmanaged, user-assigned products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all unmanaged, user-assigned products")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateUserUnmanagedProducts()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                string cmd = string.Format(@"get-msiproductinfo -installcontext userunmanaged -usersid ""{0}""", TestProject.CurrentSID);
                using (Pipeline p = rs.CreatePipeline(cmd))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{EC637522-73A5-4428-8B46-65A621529CC7}", objs[0].Properties["ProductCode"].Value as string);
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetProductCommand.ProductCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetProductCommand.ProductCode")]
        [DeploymentItem(@"data\registry.xml")]
        public void ProductCodeTest()
        {
            // First just set and get the ProductCode property value to increase code coverage.
            GetProductCommand cmdlet = new GetProductCommand();
            cmdlet.ProductCode = new string[] { "{0CABECAC-4E23-4928-871A-6E65CD370F9F}" };
            Assert.AreEqual<int>(1, cmdlet.ProductCode.Length);
            Assert.AreEqual<string>("{0CABECAC-4E23-4928-871A-6E65CD370F9F}", cmdlet.ProductCode[0]);

            // Finally invoke the cmdlet for a single product.
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value as string);
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetProductCommand.UserSid"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetProductCommand.UserSid")]
        public void UserSidTest()
        {
            GetProductCommand cmdlet = new GetProductCommand();

            // Test the default.
            Assert.AreEqual<string>(null, cmdlet.UserSid);

            // Test that what we explicitly set is returned.
            cmdlet.UserSid = "S-1-5-21-2127521184-1604012920-1887927527-2039434";
            Assert.AreEqual<string>("S-1-5-21-2127521184-1604012920-1887927527-2039434", cmdlet.UserSid);
        }

        /// <summary>
        /// A test for <see cref="GetProductCommand.InstallContext"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetProductCommand.InstallContext")]
        [DeploymentItem(@"data\registry.xml")]
        public void InstallContextTest()
        {
            // Test that the default is Machine.
            GetProductCommand cmdlet = new GetProductCommand();
            Assert.AreEqual<UserContexts>(UserContexts.Machine, cmdlet.UserContext);

            // Test string values as PowerShell would convert.
            System.ComponentModel.TypeConverter converter = new System.ComponentModel.EnumConverter(typeof(UserContexts));
            foreach (string context in new string[] { "All", "Machine", "UserManaged", "UserUnmanaged" })
            {
                UserContexts uc = (UserContexts)converter.ConvertFromString(context);
                cmdlet.UserContext = uc;
                Assert.AreEqual<UserContexts>(uc, cmdlet.UserContext);
            }

            // Test that None is not supported.
            TestProject.ExpectException(typeof(ArgumentException), null, delegate()
            {
                cmdlet.UserContext = UserContexts.None;
            });

            // Test that "Context" is a supported alias.
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                string cmd = string.Format(@"get-msiproductinfo -context userunmanaged -usersid ""{0}""", TestProject.CurrentSID);
                using (Pipeline p = rs.CreatePipeline(cmd))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{EC637522-73A5-4428-8B46-65A621529CC7}", objs[0].Properties["ProductCode"].Value as string);
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetProductCommand.Everyone"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetProductCommand.Everyone")]
        public void EveryoneTest()
        {
            GetProductCommand cmdlet = new GetProductCommand();

            // Test that the default is false / not present.
            Assert.AreEqual<bool>(false, cmdlet.Everyone);
            Assert.AreEqual<bool>(false, cmdlet.Everyone.IsPresent);

            // Test that we can set it to true.
            cmdlet.Everyone = true;
            Assert.AreEqual<bool>(true, cmdlet.Everyone);
            Assert.AreEqual<string>(NativeMethods_Accessor.World, cmdlet.UserSid);

            // Test that explicitly setting it to false nullifies the UserSid.
            cmdlet.Everyone = false;
            Assert.AreEqual<bool>(false, cmdlet.Everyone);
            Assert.AreEqual<string>(null, cmdlet.UserSid);
        }
    }
}
