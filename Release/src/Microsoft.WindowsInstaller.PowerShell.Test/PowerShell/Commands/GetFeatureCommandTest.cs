// Unit test class for the get-wifeatureinfo cmdlet.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFeatureCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFeatureCommandTest : CmdletTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            base.AddCmdlet(typeof(GetFeatureCommand), typeof(GetProductCommand));
        }

        [TestMethod]
        [Description("A test for GetFeatureCommand.Product")]
        [DeploymentItem(@"data\registry.xml")]
        public void ProductTest()
        {
            GetFeatureCommand cmdlet = new GetFeatureCommand();
            using (MockRegistry reg = new MockRegistry())
            {
                reg.Import(@"registry.xml");
                cmdlet.Product = new ProductInstallation[] { new ProductInstallation("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}") };

                Assert.AreEqual<int>(1, cmdlet.Product.Length);
                Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", cmdlet.Product[0].ProductCode);
            }
        }

        [TestMethod]
        [Description("A test for GetFeatureCommand.ProductCode")]
        public void ProductCodeTest()
        {
            GetFeatureCommand cmdlet = new GetFeatureCommand();
            cmdlet.ProductCode = "{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}";

            Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", cmdlet.ProductCode);
        }

        [TestMethod]
        [Description("A test for GetFeatureCommand.FeatureName")]
        public void FeatureNameTest()
        {
            GetFeatureCommand cmdlet = new GetFeatureCommand();
            cmdlet.FeatureName = new string[] { "Complete", "Complete2.0.30226.2" };

            Assert.AreEqual<int>(2, cmdlet.FeatureName.Length);
            CollectionAssert.AreEquivalent(new string[] { "Complete2.0.30226.2", "Complete" }, cmdlet.FeatureName);
        }

        [TestMethod]
        [Description("Enumerate ProductInstallation.Features")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateProductFeatures()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(this.Configuration))
            {
                rs.Open();
                Runspace.DefaultRunspace = rs;

                // Check the number of features for a product object.
                using (Pipeline p = rs.CreatePipeline(@"get-msiproductinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' | get-wifeatureinfo"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import(@"registry.xml");

                        Collection<PSObject> objs = p.Invoke();
                        Assert.AreEqual<int>(2, objs.Count);

                        Collection<string> actual = new Collection<string>();
                        foreach (PSObject obj in objs)
                        {
                            // Use the ETS-added Name property as an additional check.
                            Assert.IsNotNull(obj.Properties["Name"]);
                            actual.Add(obj.Properties["Name"].Value.ToString());

                            // Make sure its the right ProductCode while testing the ETS-added property.
                            Assert.IsNotNull(obj.Properties["ProductCode"]);
                            Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", obj.Properties["ProductCode"].Value.ToString());
                        }
                        CollectionAssert.AreEquivalent(new string[] { "Complete2.0.30226.2", "Complete" }, actual);
                    }
                }

                // Check that a null parameter is invalid.
                using (Pipeline p = rs.CreatePipeline(@"get-wifeatureinfo -product $null"))
                {
                    TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Check that a collection containing null is invalid.
                using (Pipeline p = rs.CreatePipeline(@"get-wifeatureinfo -product @($null)"))
                {
                    TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }
            }
        }

        [TestMethod]
        [Description("Enumerate specific features for a ProductCode")]
        [DeploymentItem(@"data\registry.xml")]
        public void EnumerateSpecificFeatures()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(this.Configuration))
            {
                rs.Open();
                Runspace.DefaultRunspace = rs;

                // Enumerate a single named feature.
                using (Pipeline p = rs.CreatePipeline(@"get-wifeatureinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' 'Complete'"))
                {
                    using (MockRegistry reg = new MockRegistry())
                    {
                        reg.Import("registry.xml");

                        Collection<PSObject> objs = p.Invoke();

                        Assert.AreEqual<int>(1, objs.Count);
                        Assert.IsNotNull(objs[0].Properties["ProductCode"]);
                        Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value.ToString());
                        Assert.IsNotNull(objs[0].Properties["Name"]);
                        Assert.AreEqual<string>("Complete", objs[0].Properties["Name"].Value.ToString());
                    }
                }

                // Check that a null parameter is invalid.
                using (Pipeline p = rs.CreatePipeline(@"get-wifeatureinfo -productcode $null"))
                {
                    TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Check that a collection containing null is invalid.
                using (Pipeline p = rs.CreatePipeline(@"get-wifeatureinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' @($null)"))
                {
                    TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }
            }
        }
    }
}
