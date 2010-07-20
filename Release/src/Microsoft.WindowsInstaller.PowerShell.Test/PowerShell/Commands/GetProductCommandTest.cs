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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetProductCommandTest : CommandTestBase
    {
        /// <summary>
        /// Enumerates all machine-assigned products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all machine-assigned products")]
        public void EnumerateProducts()
        {
            List<string> products = new List<string>();
            products.Add("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");

            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msiproductinfo"))
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

        /// <summary>
        /// Enumerates all unmanaged, user-assigned products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all unmanaged, user-assigned products")]
        public void EnumerateUserUnmanagedProducts()
        {
            List<string> expected = new List<string>();
            expected.Add("{EC637522-73A5-4428-8B46-65A621529CC7}");
            expected.Add("{B4EA7821-1AC1-41B5-8021-A2FC77D1B7B7}");

            string cmd = string.Format(@"get-msiproductinfo -installcontext userunmanaged -usersid ""{0}""", TestProject.CurrentSID);
            using (Pipeline p = TestRunspace.CreatePipeline(cmd))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();

                    List<string> actual = new List<string>(objs.Count);
                    foreach (PSObject obj in objs)
                    {
                        actual.Add(obj.Properties["ProductCode"].Value as string);
                    }

                    Assert.AreEqual<int>(expected.Count, objs.Count);
                    CollectionAssert.AreEquivalent(expected, actual);
                }
            }
        }

        /// <summary>
        /// Enumerates all products matching a given name.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all products matching a given name")]
        public void EnumerateNamedProducts()
        {
            // Use two strings that will match the same product; make sure only one product is returned.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msiproductinfo -name Silver*, *Light"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value as string);
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetProductCommand.ProductCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetProductCommand.ProductCode")]
        public void ProductCodeTest()
        {
            // Finally invoke the cmdlet for a single product.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msiproductinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"""))
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
        public void InstallContextTest()
        {
            // Test that None is not supported.
            GetProductCommand cmdlet = new GetProductCommand();
            TestProject.ExpectException(typeof(ArgumentException), null, delegate()
            {
                cmdlet.UserContext = UserContexts.None;
            });

            List<string> expected = new List<string>();
            expected.Add("{EC637522-73A5-4428-8B46-65A621529CC7}");
            expected.Add("{B4EA7821-1AC1-41B5-8021-A2FC77D1B7B7}");

            // Test that "Context" is a supported alias.
            string cmd = string.Format(@"get-msiproductinfo -context userunmanaged -usersid ""{0}""", TestProject.CurrentSID);
            using (Pipeline p = TestRunspace.CreatePipeline(cmd))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();

                    List<string> actual = new List<string>(objs.Count);
                    foreach (PSObject obj in objs)
                    {
                        actual.Add(obj.Properties["ProductCode"].Value as string);
                    }

                    Assert.AreEqual<int>(expected.Count, objs.Count);
                    CollectionAssert.AreEquivalent(expected, actual);
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

        [TestMethod]
        [Description("Tests chained execution of get-msiproductinfo")]
        [WorkItem(9464)]
        public void ChainedExecution()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msiproductinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' | get-msiproductinfo"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value as string);
                }
            }
        }
    }
}
