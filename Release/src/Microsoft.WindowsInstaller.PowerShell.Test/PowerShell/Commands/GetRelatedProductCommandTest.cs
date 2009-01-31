// Unit test class for the get-msirelatedproductinfo cmdlet.
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

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetRelatedProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetRelatedProductCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetRelatedProductCommand));
        }

        /// <summary>
        /// Enumerates related products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates related products")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateRelatedProducts()
        {
            List<string> products = new List<string>();
            products.Add("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");

            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msirelatedproductinfo -upgradecode ""{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}"""))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        // Import our registry entries.
                        reg.Import(@"registry.xml");

                        // TODO: Validate that the correct products were found.
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetRelatedProductCommand.UpgradeCode"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for GetRelatedProductCommand.UpgradeCode")]
        public void UpgradeCodeTest()
        {
            GetRelatedProductCommand cmdlet = new GetRelatedProductCommand();
            cmdlet.UpgradeCode = new string[] { "{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}" };
            Assert.AreEqual<int>(1, cmdlet.UpgradeCode.Length);
            Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", cmdlet.UpgradeCode[0]);
        }
    }
}
