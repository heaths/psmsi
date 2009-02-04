// Unit test class for the Get-MSIFileType cmdlet.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileHashCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileHashCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetFileHashCommand));
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.Path")]
        [DeploymentItem(@"data\example.txt")]
        public void PathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();

                // Test a file.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path example.txt"))
                {
                    // TODO: Compare against the following hash.
                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };
                }

                // Test against a directory.
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -path ."))
                {
                    // TODO: Verify that there's one PSObject returned.
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.PassThru"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.PassThru")]
        [DeploymentItem(@"data\example.txt")]
        public void PassThruTest()
        {
            // Now invoke the cmdlet and check the file hash properties.
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-childitem -path example.txt | get-msifilehash -passthru"))
                {
                    // TODO: Verify a FileInfo is returned with the following hash.
                    int[] hash = new int[] { 1820344194, -1963188082, -1359304639, 10459557 };
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileHashCommand.LiteralPath"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileHashCommand.LiteralPath")]
        [DeploymentItem(@"data\example.txt")]
        public void LiteralPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msifilehash -literalpath example.*"))
                {
                    // TODO: Verify that nothing is returned (wildcards not supported).
                }
            }
        }
    }
}
