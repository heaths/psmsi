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
using System.Reflection;
using System.Security.Principal;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetProductCommandTest
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
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSIProductInfo", typeof(GetProductCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
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

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(products.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            PSPropertyInfo info = obj.Properties["ProductCode"];
                            Assert.IsNotNull(info);

                            string productCode = (string)info.Value;
                            products.Remove(productCode);

                            if ("{0CABECAC-4E23-4928-871A-6E65CD370F9F}" == productCode)
                            {
                                Assert.IsInstanceOfType(obj.BaseObject, typeof(AdvertisedProductInfo));
                            }
                            else if ("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}" == productCode)
                            {
                                Assert.IsInstanceOfType(obj.BaseObject, typeof(InstalledProductInfo));
                            }
                        }
                    }
                }
            }

            // Make sure all products were found.
            Assert.AreEqual<int>(0, products.Count);
        }

        /// <summary>
        /// Enumerates all products using the legacy function.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all products using the legacy function")]
        [DeploymentItem(@"data\registry.xml")]
        public void LegacyEnumerateProducts()
        {
            List<string> products = new List<string>();
            products.Add("{0CABECAC-4E23-4928-871A-6E65CD370F9F}");
            products.Add("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");
            products.Add("{EC637522-73A5-4428-8B46-65A621529CC7}");

            try
            {
                MsiTest.MajorOverride = 2;
                using (Runspace rs = RunspaceFactory.CreateRunspace(config))
                {
                    rs.Open();
                    using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo"))
                    {
                        using (MockRegistry reg = new MockRegistry())
                        {
                            // Import our registry entries.
                            reg.Import(@"registry.xml");

                            Collection<PSObject> objs = p.Invoke();
                            Assert.AreEqual<int>(products.Count, objs.Count);

                            foreach (PSObject obj in objs)
                            {
                                Assert.IsInstanceOfType(obj.BaseObject, typeof(InstalledProductInfo));

                                PSPropertyInfo info = obj.Properties["ProductCode"];
                                Assert.IsNotNull(info);

                                products.Remove((string)info.Value);
                            }
                        }
                    }
                }
            }
            finally
            {
                // Force a re-get of the msi.dll version.
                MsiTest.MajorOverride = 0;
            }

            // Check that all products were found.
            Assert.AreEqual<int>(0, products.Count);
        }

        /// <summary>
        /// Enumerates all unmanaged, user-assigned products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all unmanaged, user-assigned products")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateUserUnmanagedProducts()
        {
            List<string> products = new List<string>();
            products.Add("{EC637522-73A5-4428-8B46-65A621529CC7}");

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                string cmd = string.Format(@"get-msiproductinfo -installcontext userunmanaged -usersid ""{0}""", TestProject.CurrentSID);
                using (Pipeline p = rs.CreatePipeline(cmd))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(products.Count, objs.Count);

                        PSObject obj = objs[0];
                        Assert.IsInstanceOfType(obj.BaseObject, typeof(InstalledProductInfo));

                        PSPropertyInfo info = obj.Properties["ProductCode"];
                        Assert.IsNotNull(info);

                        products.Remove((string)info.Value);
                    }
                }
            }

            // Check that all products were found.
            Assert.AreEqual<int>(0, products.Count);
        }

        /// <summary>
        /// Enumerates products with a user SID that is initially too small.
        /// </summary>
        [TestMethod]
        [Description("Enumerates products with a user SID that is initially too small")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductsIncreasingSid()
        {
            List<string> products = new List<string>();
            products.Add("{EC637522-73A5-4428-8B46-65A621529CC7}");

            int originalSidLength = NativeMethods.DefaultSidLength;
            try
            {
                NativeMethods.DefaultSidLength = 40;

                using (Runspace rs = RunspaceFactory.CreateRunspace(config))
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

                            PSObject obj = objs[0];
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(InstalledProductInfo));

                            PSPropertyInfo info = obj.Properties["ProductCode"];
                            Assert.IsNotNull(info);

                            products.Remove((string)info.Value);
                        }
                    }
                }
            }
            finally
            {
                NativeMethods.DefaultSidLength = originalSidLength;
            }

            // Check that all products were found.
            Assert.AreEqual<int>(0, products.Count);
        }

        /// <summary>
        /// A test for <see cref="GetProductCommand.GetErrorDetails"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetProductCommand.GetErrorDetails")]
        public void GetErrorDetailsTest()
        {
            GetProductCommand cmdlet = new GetProductCommand();
            Type t = cmdlet.GetType();

            FieldInfo f = t.GetField("currentProductCode", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(cmdlet, "{0CABECAC-4E23-4928-871A-6E65CD370F9F}");

            // Call GetErrorDetails directly. Through testing it seems that Windows Installer will never
            // return ERROR_BAD_CONFIGURATION for products, as it simply ignores invalid data during enumeration.
            MethodInfo m = t.GetMethod("GetErrorDetails", BindingFlags.Instance | BindingFlags.NonPublic);

            ErrorDetails error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_BAD_CONFIGURATION });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("The configuration data for product {0CABECAC-4E23-4928-871A-6E65CD370F9F} is corrupt.", error.Message);
            Assert.AreEqual<string>("Reinstall the product with REINSTALLMODE=vomus.", error.RecommendedAction);

            error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_ACCESS_DENIED });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("Access denied.", error.Message);
            Assert.AreEqual<string>("Run the expression again in an elevated process.", error.RecommendedAction);
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
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
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

                        PSObject obj = objs[0];
                        Assert.IsInstanceOfType(obj.BaseObject, typeof(InstalledProductInfo));

                        ProductInfo info = (ProductInfo)obj.BaseObject;
                        Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", info.ProductCode);
                        Assert.AreEqual<InstallContext>(InstallContext.Machine, info.InstallContext);
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
            Assert.AreEqual<InstallContext>(InstallContext.Machine, cmdlet.InstallContext);

            // Test string values as PowerShell would convert.
            System.ComponentModel.TypeConverter converter = new System.ComponentModel.EnumConverter(typeof(InstallContext));
            foreach (string context in new string[] { "All", "Machine", "UserManaged", "UserUnmanaged" })
            {
                InstallContext ic = (InstallContext)converter.ConvertFromString(context);
                cmdlet.InstallContext = ic;
                Assert.AreEqual<InstallContext>(ic, cmdlet.InstallContext);
            }

            // Test that None is not supported.
            try
            {
                cmdlet.InstallContext = InstallContext.None;
                Assert.Fail("InstallContext.None should not be supported");
            }
            catch (PSInvalidParameterException ex)
            {
                Assert.AreEqual<string>(@"""None"" is not valid for the InstallContext parameter.", ex.Message);
            }

            // Test that "Context" is a supported alias.
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
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

                        PSObject obj = objs[0];
                        Assert.IsInstanceOfType(obj.BaseObject, typeof(InstalledProductInfo));
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
            Assert.AreEqual<string>(NativeMethods.World, cmdlet.UserSid);

            // Test that explicitly setting it to false nullifies the UserSid.
            cmdlet.Everyone = false;
            Assert.AreEqual<bool>(false, cmdlet.Everyone);
            Assert.AreEqual<string>(null, cmdlet.UserSid);
        }
    }
}
