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
            string package = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            using (var rs = this.TestRunspace.CreatePipeline(@"get-item ""$TestDeploymentDirectory\Example.msi"" | test-msiproduct -include ICE0* -exclude ICE03 -patch ""$TestDeploymentDirectory\Example.msp"" -transform ""$TestDeploymentDirectory\Example.mst"" -v"))
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
                        Assert.AreEqual(package, ice.Path, true, "The path to the database is incorrect.");
                    }
                }
            }
        }

        [TestMethod]
        public void TestProductCustomIceCube()
        {
            string package = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            var keys = new string[] { "Key1", "Key2" };

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
                        Assert.AreEqual(package, ice.Path, true, "The path to the database is incorrect.");

                        if (IceMessageType.Error == ice.Type || IceMessageType.Warning == ice.Type)
                        {
                            Assert.AreEqual<string>("http://psmsi.codeplex.com", ice.Url, "The ICE URL is incorrect.");
                            Assert.AreEqual<string>("Table", ice.Table, "The ICE table is incorrect.");
                            Assert.AreEqual<string>("Column", ice.Column, "The ICE column is incorrect.");
                            CollectionAssert.AreEqual(keys, ice.PrimaryKeys, "The ICE primary keys are incorrect.");
                        }
                    }
                }
            }
        }
    }
}
