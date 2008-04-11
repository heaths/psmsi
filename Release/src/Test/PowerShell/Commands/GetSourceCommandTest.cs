// Unit test class for the get-msisource cmdlet.
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
using System.Reflection;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetRelatedProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetSourceCommandTest
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
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSIPatchInfo", typeof(GetPatchCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSISource", typeof(GetSourceCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
        }

        /// <summary>
        /// Enumerates product source.
        /// </summary>
        [TestMethod]
        [Description("Enumerates product source")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductSource()
        {
            List<PackageSource> sources = new List<PackageSource>();
            sources.Add(new PackageSource(SourceTypes.Network, 0, @"c:\2014be2e43e417a3b9\"));

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msisource -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(sources.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PackageSource));
                            PackageSource source = (PackageSource)obj.BaseObject;

                            // Remove an equal PackageSource from the list.
                            sources.RemoveAll(delegate(PackageSource item)
                            {
                                return item.Index == source.Index
                                    && item.SourceType == source.SourceType
                                    && string.Equals(item.Path, source.Path, StringComparison.OrdinalIgnoreCase);
                            });
                        }
                    }
                }
            }

            // Make sure all products were found.
            Assert.AreEqual<int>(0, sources.Count);
        }

        /// <summary>
        /// Enumerates source for products from the pipeline.
        /// </summary>
        [TestMethod]
        [Description("Enumerates source for products from the pipeline")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductSourceFromPipeline()
        {
            List<PackageSource> sources = new List<PackageSource>();
            sources.Add(new PackageSource(SourceTypes.Network, 0, @"c:\2014be2e43e417a3b9\"));

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" | get-msisource"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(sources.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PackageSource));
                            PackageSource source = (PackageSource)obj.BaseObject;

                            // Remove an equal PackageSource from the list.
                            sources.RemoveAll(delegate(PackageSource item)
                            {
                                return item.Index == source.Index
                                    && item.SourceType == source.SourceType
                                    && string.Equals(item.Path, source.Path, StringComparison.OrdinalIgnoreCase);
                            });
                        }
                    }
                }
            }

            // Make sure all products were found.
            Assert.AreEqual<int>(0, sources.Count);
        }

        /// <summary>
        /// Enumerates patch source.
        /// </summary>
        [TestMethod]
        [Description("Enumerates patch source")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumeratePatchSource()
        {
            List<PackageSource> sources = new List<PackageSource>();
            sources.Add(new PackageSource(SourceTypes.Network, 0, @"c:\updates\"));

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msisource -patchcode ""{6E52C409-0D0D-4B84-AB63-463438D4D33B}"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(sources.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PackageSource));
                            PackageSource source = (PackageSource)obj.BaseObject;

                            // Remove an equal PackageSource from the list.
                            sources.RemoveAll(delegate(PackageSource item)
                            {
                                return item.Index == source.Index
                                    && item.SourceType == source.SourceType
                                    && string.Equals(item.Path, source.Path, StringComparison.OrdinalIgnoreCase);
                            });
                        }
                    }
                }
            }

            // Make sure all products were found.
            Assert.AreEqual<int>(0, sources.Count);
        }

        /// <summary>
        /// Enumerates source for patches from the pipeline.
        /// </summary>
        [TestMethod]
        [Description("Enumerates source for patches from the pipeline")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumeratePatchSourceFromPipeline()
        {
            List<PackageSource> sources = new List<PackageSource>();
            sources.Add(new PackageSource(SourceTypes.Network, 0, @"c:\updates\"));

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" -patchcode ""{6E52C409-0D0D-4B84-AB63-463438D4D33B}"" | get-msisource"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(sources.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PackageSource));
                            PackageSource source = (PackageSource)obj.BaseObject;

                            // Remove an equal PackageSource from the list.
                            sources.RemoveAll(delegate(PackageSource item)
                            {
                                return item.Index == source.Index
                                    && item.SourceType == source.SourceType
                                    && string.Equals(item.Path, source.Path, StringComparison.OrdinalIgnoreCase);
                            });
                        }
                    }
                }
            }

            // Make sure all products were found.
            Assert.AreEqual<int>(0, sources.Count);
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.GetErrorDetails"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.GetErrorDetails")]
        public void GetErrorDetailsTest()
        {
            GetSourceCommand cmdlet = new GetSourceCommand();
            Type t = cmdlet.GetType();

            FieldInfo currentProductOrPatchCode = t.GetField("currentProductOrPatchCode", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo code = t.GetField("code", BindingFlags.Instance | BindingFlags.NonPublic);

            // Call GetErrorDetails directly. Through testing it seems that Windows Installer will never
            // return ERROR_BAD_CONFIGURATION for products, as it simply ignores invalid data during enumeration.
            MethodInfo m = t.GetMethod("GetErrorDetails", BindingFlags.Instance | BindingFlags.NonPublic);

            // Bad patch configuration.
            currentProductOrPatchCode.SetValue(cmdlet, "{6E52C409-0D0D-4B84-AB63-463438D4D33B}");
            code.SetValue(cmdlet, Code.Patch);
            ErrorDetails error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_BAD_CONFIGURATION });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("The configuration data for patch {6E52C409-0D0D-4B84-AB63-463438D4D33B} is corrupt.", error.Message);

            // Bad product configuration.
            currentProductOrPatchCode.SetValue(cmdlet, "{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");
            code.SetValue(cmdlet, Code.Product);
            error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_BAD_CONFIGURATION });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("The configuration data for product {89F4137D-6C26-4A84-BDB8-2E5A4BB71E00} is corrupt.", error.Message);
            Assert.AreEqual<string>("Reinstall the product with REINSTALLMODE=vomus.", error.RecommendedAction);

            error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_ACCESS_DENIED });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("Access denied.", error.Message);
            Assert.AreEqual<string>("Run the expression again in an elevated process.", error.RecommendedAction);
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.ProductCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.ProductCode")]
        public void ProductCodeTest()
        {
            // First just set and get the ProductCode property value to increase code coverage.
            GetSourceCommand cmdlet = new GetSourceCommand();
            cmdlet.ProductCode = new string[] { "{0CABECAC-4E23-4928-871A-6E65CD370F9F}" };
            Assert.AreEqual<int>(1, cmdlet.ProductCode.Length);
            Assert.AreEqual<string>("{0CABECAC-4E23-4928-871A-6E65CD370F9F}", cmdlet.ProductCode[0]);
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.PatchCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.PatchCode")]
        public void PatchCodeTest()
        {
            // First just set and get the ProductCode property value to increase code coverage.
            GetSourceCommand cmdlet = new GetSourceCommand();
            cmdlet.PatchCode = new string[] { "{6E52C409-0D0D-4B84-AB63-463438D4D33B}" };
            Assert.AreEqual<int>(1, cmdlet.PatchCode.Length);
            Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", cmdlet.PatchCode[0]);
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.UserSid"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.UserSid")]
        public void UserSidTest()
        {
            GetSourceCommand cmdlet = new GetSourceCommand();
            cmdlet.UserSid = "S-1-5-21-2127521184-1604012920-1887927527-2039434";
            Assert.AreEqual<string>("S-1-5-21-2127521184-1604012920-1887927527-2039434", cmdlet.UserSid);
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.InstallContext"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.InstallContext")]
        [DeploymentItem(@"data\registry.xml")]
        public void InstallContextTest()
        {
            // Test that the default is Machine.
            GetSourceCommand cmdlet = new GetSourceCommand();
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

            List<PackageSource> sources = new List<PackageSource>();
            sources.Add(new PackageSource(SourceTypes.Network, 0, @"c:\2014be2e43e417a3b9\"));

            // Test that "Context" is a supported alias.
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                using (Pipeline p = rs.CreatePipeline(@"get-msisource -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" -context ""machine"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(sources.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PackageSource));
                            PackageSource source = (PackageSource)obj.BaseObject;

                            // Remove an equal PackageSource from the list.
                            sources.RemoveAll(delegate(PackageSource item)
                            {
                                return item.Index == source.Index
                                    && item.SourceType == source.SourceType
                                    && string.Equals(item.Path, source.Path, StringComparison.OrdinalIgnoreCase);
                            });
                        }
                    }
                }
            }

            // Make sure all expected patches were found.
            Assert.AreEqual<int>(0, sources.Count);
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.Filter"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.Filter")]
        [DeploymentItem(@"data\registry.xml")]
        public void SourceTypeTest()
        {
            // Test that the default is Network.
            GetSourceCommand cmdlet = new GetSourceCommand();
            Assert.AreEqual<SourceTypes>(SourceTypes.Network, cmdlet.SourceType);

            // Test string values as PowerShell would convert.
            System.ComponentModel.TypeConverter converter = new System.ComponentModel.EnumConverter(typeof(SourceTypes));
            foreach (string sourceType in new string[] { "Network", "Url" })
            {
                SourceTypes st = (SourceTypes)converter.ConvertFromString(sourceType);
                cmdlet.SourceType = st;
                Assert.AreEqual<SourceTypes>(st, cmdlet.SourceType);
            }

            // Test that None, Media, and All are not supported.
            foreach (string sourceType in new string[] { "None", "Media", "All" })
            {
                try
                {
                    SourceTypes st = (SourceTypes)converter.ConvertFromString(sourceType);
                    cmdlet.SourceType = st;
                    Assert.Fail(string.Format("SourceTypes.{0} should not be supported", sourceType));
                }
                catch (PSInvalidParameterException ex)
                {
                    Assert.AreEqual<string>(string.Format(@"""{0}"" is not valid for the SourceType parameter.", sourceType), ex.Message);
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetSourceCommand.Everyone"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetSourceCommand.Everyone")]
        public void EveryoneTest()
        {
            GetSourceCommand cmdlet = new GetSourceCommand();

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
