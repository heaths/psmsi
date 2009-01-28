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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetRelatedProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetSourceCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetProductCommand), typeof(GetPatchCommand), typeof(GetSourceCommand));
        }

        /// <summary>
        /// Enumerates product source.
        /// </summary>
        [TestMethod]
        [Description("Enumerates product source")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductSource()
        {
            // TODO: Need to rewrite these tests since the cmdlets are very different.
        }

        /// <summary>
        /// Enumerates source for products from the pipeline.
        /// </summary>
        [TestMethod]
        [Description("Enumerates source for products from the pipeline")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductSourceFromPipeline()
        {
            // TODO: Need to rewrite these tests since the cmdlets are very different.
        }

        /// <summary>
        /// Enumerates patch source.
        /// </summary>
        [TestMethod]
        [Description("Enumerates patch source")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumeratePatchSource()
        {
            // TODO: Need to rewrite these tests since the cmdlets are very different.
        }

        /// <summary>
        /// Enumerates source for patches from the pipeline.
        /// </summary>
        [TestMethod]
        [Description("Enumerates source for patches from the pipeline")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumeratePatchSourceFromPipeline()
        {
            // TODO: Need to rewrite these tests since the cmdlets are very different.
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
            try
            {
                cmdlet.UserContext = UserContexts.None;
                Assert.Fail("InstallContext.None should not be supported");
            }
            catch (Exception)
            {
                // TODO: Should validate the exception type.
            }

            // TODO: Validate cmdlet invocation using the UserContext parameter.
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
