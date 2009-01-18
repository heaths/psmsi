// Unit test class for the get-msigetpatch cmdlet.
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
    /// Unit and functional tests for <see cref="GetPatchCommand"/>.
    ///</summary>
    [TestClass]
    public class GetPatchCommandTest
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
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSIPatchInfo", typeof(GetPatchCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
            config.Cmdlets.Append(new CmdletConfigurationEntry("Get-MSIProductInfo", typeof(GetProductCommand), "Microsoft.Windows.Installer.PowerShell.dll-Help.xml"));
        }

        /// <summary>
        /// Enumerates all machine-assigned patches.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all machine-assigned patches")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumeratePatches()
        {
            List<string> patches = new List<string>();
            patches.Add("{6E52C409-0D0D-4B84-AB63-463438D4D33B}");

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(patches.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PatchInfo));

                            PSPropertyInfo info = obj.Properties["PatchCode"];
                            Assert.IsNotNull(info);

                            // Remove current patch from expected list.
                            patches.Remove((string)info.Value);
                        }
                    }
                }
            }

            // Make sure all patches were found.
            Assert.AreEqual<int>(0, patches.Count);
        }

        /// <summary>
        /// Enumerates patches for a specific product.
        /// </summary>
        [TestMethod]
        [Description("Enumerates patches for a specific product")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductPatches()
        {
            List<string> patches = new List<string>();
            patches.Add("{6E52C409-0D0D-4B84-AB63-463438D4D33B}");

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" | get-msipatchinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(patches.Count, objs.Count);

                        // Make sure expected patch codes were found.
                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PatchInfo));

                            PSPropertyInfo info = obj.Properties["PatchCode"];
                            Assert.IsNotNull(info);

                            patches.Remove((string)info.Value);
                        }
                    }
                }
            }

            // Check that all products were found.
            Assert.AreEqual<int>(0, patches.Count);
        }

        /// <summary>
        /// Gets a specific patch for a specific product.
        /// </summary>
        [TestMethod]
        [Description("Gets a specific patch for a specific product")]
        [DeploymentItem(@"data\registry.xml")]
        public void GetSpecificPatch()
        {
            List<string> patches = new List<string>();
            patches.Add("{6E52C409-0D0D-4B84-AB63-463438D4D33B}");

            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" -patchcode ""{6E52C409-0D0D-4B84-AB63-463438D4D33B}"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(patches.Count, objs.Count);

                        // Make sure expected patch codes were found.
                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PatchInfo));

                            PSPropertyInfo info = obj.Properties["PatchCode"];
                            Assert.IsNotNull(info);

                            patches.Remove((string)info.Value);
                        }
                    }
                }
            }

            // Check that all products were found.
            Assert.AreEqual<int>(0, patches.Count);
        }

        /// <summary>
        /// Enumerates all patches using the legacy function.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all patches using the legacy function")]
        [DeploymentItem(@"data\registry.xml")]
        public void LegacyEnumeratePatches()
        {
            List<string> patches = new List<string>();
            patches.Add("{6E52C409-0D0D-4B84-AB63-463438D4D33B}");

            try
            {
                MsiTest.MajorOverride = 2;
                using (Runspace rs = RunspaceFactory.CreateRunspace(config))
                {
                    rs.Open();
                    using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"""))
                    {
                        using (MockRegistry reg = new MockRegistry())
                        {
                            // Import our registry entries.
                            reg.Import(@"registry.xml");

                            Collection<PSObject> objs = p.Invoke();
                            Assert.AreEqual<int>(patches.Count, objs.Count);

                            // Make sure expected patch codes were found.
                            foreach (PSObject obj in objs)
                            {
                                Assert.IsInstanceOfType(obj.BaseObject, typeof(PatchInfo));

                                PSPropertyInfo info = obj.Properties["PatchCode"];
                                Assert.IsNotNull(info);

                                patches.Remove((string)info.Value);
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
            Assert.AreEqual<int>(0, patches.Count);
        }
        [TestMethod]
        [Description("Enumerates all patches using the legacy function without specifying a ProductCode")]
        [DeploymentItem(@"data\registry.xml")]
        public void LegacyEnumeratePatchesWithoutProductCode()
        {
            try
            {
                MsiTest.MajorOverride = 2;
                using (Runspace rs = RunspaceFactory.CreateRunspace(config))
                {
                    rs.Open();
                    using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo"))
                    {
                        using (MockRegistry reg = new MockRegistry())
                        {
                            // Import our registry entries.
                            reg.Import(@"registry.xml");

                            // Without a ProductCode, this should fail.
                            try
                            {
                                Collection<PSObject> objs = p.Invoke();
                                Assert.Fail("Expected PSNotSupportedException was not thrown.");
                            }
                            catch (RuntimeException ex)
                            {
                                Assert.IsInstanceOfType(ex.InnerException, typeof(PSNotSupportedException));
                                Assert.AreEqual<string>(@"A ProductCode is required for the version of Windows Installer installed.", ex.Message);
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
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.GetErrorDetails"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.GetErrorDetails")]
        public void GetErrorDetailsTest()
        {
            GetPatchCommand cmdlet = new GetPatchCommand();
            Type t = cmdlet.GetType();

            FieldInfo f = t.GetField("currentProductCode", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(cmdlet, "{6E52C409-0D0D-4B84-AB63-463438D4D33B}");

            // Call GetErrorDetails directly. Through testing it seems that Windows Installer will never
            // return ERROR_BAD_CONFIGURATION for products, as it simply ignores invalid data during enumeration.
            MethodInfo m = t.GetMethod("GetErrorDetails", BindingFlags.Instance | BindingFlags.NonPublic);

            ErrorDetails error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_BAD_CONFIGURATION });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("The configuration data for patch {6E52C409-0D0D-4B84-AB63-463438D4D33B} is corrupt.", error.Message);
            Assert.AreEqual<string>("Reinstall the product with REINSTALLMODE=vomus.", error.RecommendedAction);

            error = (ErrorDetails)m.Invoke(cmdlet, new object[] { NativeMethods.ERROR_ACCESS_DENIED });
            Assert.IsNotNull(error);
            Assert.AreEqual<string>("Access denied.", error.Message);
            Assert.AreEqual<string>("Run the expression again in an elevated process.", error.RecommendedAction);
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.ProductCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.ProductCode")]
        public void ProductCodeTest()
        {
            GetPatchCommand cmdlet = new GetPatchCommand();
            cmdlet.ProductCode = new string[] { "{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}" };
            Assert.AreEqual<int>(1, cmdlet.ProductCode.Length);
            Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", cmdlet.ProductCode[0]);
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.PatchCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.PatchCode")]
        public void PatchCodeTest()
        {
            GetPatchCommand cmdlet = new GetPatchCommand();
            cmdlet.PatchCode = new string[] { "{6E52C409-0D0D-4B84-AB63-463438D4D33B}" };
            Assert.AreEqual<int>(1, cmdlet.PatchCode.Length);
            Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", cmdlet.PatchCode[0]);
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.UserSid"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.UserSid")]
        public void UserSidTest()
        {
            GetPatchCommand cmdlet = new GetPatchCommand();
            cmdlet.UserSid = "S-1-5-21-2127521184-1604012920-1887927527-2039434";
            Assert.AreEqual<string>("S-1-5-21-2127521184-1604012920-1887927527-2039434", cmdlet.UserSid);
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.InstallContext"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.InstallContext")]
        [DeploymentItem(@"data\registry.xml")]
        public void InstallContextTest()
        {
            // Test that the default is Machine.
            GetPatchCommand cmdlet = new GetPatchCommand();
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

            List<string> patches = new List<string>();
            patches.Add("{6E52C409-0D0D-4B84-AB63-463438D4D33B}");

            // Test that "Context" is a supported alias.
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                string cmd = string.Format(@"get-msipatchinfo -context ""machine""");
                using (Pipeline p = rs.CreatePipeline(cmd))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(patches.Count, objs.Count);

                        foreach (PSObject obj in objs)
                        {
                            Assert.IsInstanceOfType(obj.BaseObject, typeof(PatchInfo));

                            PSPropertyInfo info = obj.Properties["PatchCode"];
                            Assert.IsNotNull(info);

                            patches.Remove((string)info.Value);
                        }
                    }
                }
            }

            // Make sure all expected patches were found.
            Assert.AreEqual<int>(0, patches.Count);
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.Filter"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.Filter")]
        [DeploymentItem(@"data\registry.xml")]
        public void FilterTest()
        {
            // Test that the default is Applied.
            GetPatchCommand cmdlet = new GetPatchCommand();
            Assert.AreEqual<PatchStates>(PatchStates.Applied, cmdlet.Filter);

            // Test string values as PowerShell would convert.
            System.ComponentModel.TypeConverter converter = new System.ComponentModel.EnumConverter(typeof(PatchStates));
            foreach (string filter in new string[] { "All", "Applied", "Superseded", "Obsoleted", "Registered" })
            {
                PatchStates ic = (PatchStates)converter.ConvertFromString(filter);
                cmdlet.Filter = ic;
                Assert.AreEqual<PatchStates>(ic, cmdlet.Filter);
            }

            // Test that Invalid is not supported.
            try
            {
                cmdlet.Filter = PatchStates.Invalid;
                Assert.Fail("PatchStates.Invalid should not be supported");
            }
            catch (PSInvalidParameterException ex)
            {
                Assert.AreEqual<string>(@"""Invalid"" is not valid for the Filter parameter.", ex.Message);
            }
        }

        /// <summary>
        /// A test for <see cref="GetPatchCommand.Everyone"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetPatchCommand.Everyone")]
        public void EveryoneTest()
        {
            GetPatchCommand cmdlet = new GetPatchCommand();

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
