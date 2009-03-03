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
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.WindowsInstaller.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetPatchCommand"/>.
    ///</summary>
    [TestClass]
    public class GetPatchCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetPatchCommand), typeof(GetProductCommand));
        }

        /// <summary>
        /// Enumerates all machine-assigned patches.
        /// </summary>
        [TestMethod]
        [Description("Enumerates all machine-assigned patches")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumeratePatches()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].Properties["PatchCode"].Value as string);
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates patches for a specific product.
        /// </summary>
        [TestMethod]
        [Description("Enumerates patches for a specific product")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductPatches()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" | get-msipatchinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].Properties["PatchCode"].Value as string);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a specific patch for a specific product.
        /// </summary>
        [TestMethod]
        [Description("Gets a specific patch for a specific product")]
        [DeploymentItem(@"data\registry.xml")]
        public void GetSpecificPatch()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msipatchinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" -patchcode ""{6E52C409-0D0D-4B84-AB63-463438D4D33B}"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].Properties["PatchCode"].Value as string);
                    }
                }
            }
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

                string cmd = string.Format(@"get-msipatchinfo -context ""machine""");
                using (Pipeline p = rs.CreatePipeline(cmd))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].Properties["PatchCode"].Value as string);
                    }
                }
            }
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
                PatchStates ps = (PatchStates)converter.ConvertFromString(filter);
                cmdlet.Filter = ps;
                Assert.AreEqual<PatchStates>(ps, cmdlet.Filter);
            }

            // Test that Invalid is not supported.
            TestProject.ExpectException(typeof(ArgumentException), null, delegate()
            {
                cmdlet.Filter = PatchStates.None;
            });
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
            Assert.AreEqual<string>(NativeMethods_Accessor.World, cmdlet.UserSid);

            // Test that explicitly setting it to false nullifies the UserSid.
            cmdlet.Everyone = false;
            Assert.AreEqual<bool>(false, cmdlet.Everyone);
            Assert.AreEqual<string>(null, cmdlet.UserSid);
        }
    }
}
