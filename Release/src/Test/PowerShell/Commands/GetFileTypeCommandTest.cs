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

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFileTypeCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFileTypeCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetFileTypeCommand));
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.Path"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.Path")]
        [DeploymentItem(@"data")]
        public void PathTest()
        {
            // Now invoke the cmdlet and check the file type property.
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -path example.*"))
                {
                    // TODO: Validate that each of the package types are provided.
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.PassThru"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.PassThru")]
        [DeploymentItem(@"data")]
        public void PassThruTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-childitem -path example.* | get-msifiletype -passthru"))
                {
                    // TODO: Validate that FileInfo objects are returned along with the file type members.
                }
            }
        }

        /// <summary>
        /// A test for <see cref="GetFileTypeCommand.LiteralPath"/>.
        ///</summary>
        [TestMethod]
        [Description("A test for GetFileTypeCommand.LiteralPath")]
        [DeploymentItem(@"data\example.txt")]
        public void LiteralPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(base.Configuration))
            {
                rs.Open();
                using (Pipeline p = rs.CreatePipeline(@"get-msifiletype -literalpath example.*"))
                {
                    // TODO: Validate that no objects are returned (wildcards not supported).
                }
            }
        }
    }
}
