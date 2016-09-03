// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="TestProductCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class TestProductCommandTests : TestBase
    {
        [TestMethod]
        public void TestProductDefaultIceCube()
        {
            var package = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            using (var p = CreatePipeline(@"get-item Example.msi | test-msiproduct -include ICE0* -exclude ICE03 -patch Example.msp -transform Example.mst -v"))
            {
                using (this.OverrideRegistry())
                {
                    var items = p.Invoke();
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
            var package = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            var keys = new string[] { "Key1", "Key2" };

            using (var p = CreatePipeline(@"test-msiproduct Example.msi -nodefault -add test.cub"))
            {
                using (this.OverrideRegistry())
                {
                    var items = p.Invoke();
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
