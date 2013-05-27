// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="TestProductCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class TestProductCommandTests : CommandTestBase
    {
        [TestMethod]
        public void TestProductDefaultIceCube()
        {
            using (var rs = this.TestRunspace.CreatePipeline(@"get-item ""$TestDeploymentDirectory\Example.msi"" | test-msiproduct -include ICE0* -exclude ICE03 -v"))
            {
                using (var reg = new MockRegistry())
                {
                    string path = Path.Combine(this.TestContext.DeploymentDirectory, "Registry.xml");
                    reg.Import(path, new Dictionary<string, string>() { { "TestDeploymentDirectory", this.TestContext.DeploymentDirectory } });

                    var items = rs.Invoke();
                    foreach (var item in items)
                    {
                        Assert.IsInstanceOfType(item.BaseObject, typeof(IceMessage), "The output object is not an IceMessage.");
                        var ice = (IceMessage)item.BaseObject;

                        int num = Convert.ToInt32(ice.Name.Substring(3));
                        Assert.IsTrue(0 < num && num < 10 && num != 3, "The ICE number is incorrect.");
                    }
                }
            }
        }

        [TestMethod]
        public void TestProductCustomIceCube()
        {
            using (var rs = this.TestRunspace.CreatePipeline(@"test-msiproduct ""$TestDeploymentDirectory\Example.msi"" -nodefault -add ""$TestDeploymentDirectory\test.cub"""))
            {
                using (var reg = new MockRegistry())
                {
                    string path = Path.Combine(this.TestContext.DeploymentDirectory, "Registry.xml");
                    reg.Import(path, new Dictionary<string, string>() { { "TestDeploymentDirectory", this.TestContext.DeploymentDirectory } });

                    var items = rs.Invoke();
                    foreach (var item in items)
                    {
                        Assert.IsInstanceOfType(item.BaseObject, typeof(IceMessage), "The output object is not an IceMessage.");
                        var ice = (IceMessage)item.BaseObject;

                        Assert.AreEqual<string>("ICE1000", ice.Name, "The ICE name is incorrect.");
                    }
                }
            }
        }
    }
}
