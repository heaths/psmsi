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
    public class GetFeatureCommandTest : CommandTestBase
    {
        [TestMethod]
        [Description("Enumerate ProductInstallation.Features")]
        public void EnumerateProductFeatures()
        {
            Collection<string> expectedFeatures = new Collection<string>();
            Collection<string> expectedProductCodes = new Collection<string>();

            // Populate expected features.
            expectedFeatures.Add("Complete");
            expectedFeatures.Add("Complete2.0.30226.2");
            expectedFeatures.Add("DefaultFeature");
            expectedFeatures.Add("Module");

            // Populate expected ProductCodes.
            expectedProductCodes.Add("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");
            expectedProductCodes.Add("{B4EA7821-1AC1-41B5-8021-A2FC77D1B7B7}");
            expectedProductCodes.Add("{EC637522-73A5-4428-8B46-65A621529CC7}");

            // Check the number of features for a product object.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wiproductinfo -context 'all' | get-wifeatureinfo"))
            {
                Runspace.DefaultRunspace = p.Runspace;
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(expectedFeatures.Count, objs.Count);

                    Collection<string> actualFeatures = new Collection<string>();
                    Collection<string> actualProductCodes = new Collection<string>();

                    foreach (PSObject obj in objs)
                    {
                        // Use the ETS-added Name property as an additional check.
                        Assert.IsNotNull(obj.Properties["Name"]);
                        actualFeatures.Add(obj.Properties["Name"].Value.ToString());

                        // Use the ETS-added ProductCode property as an additional check.
                        Assert.IsNotNull(obj.Properties["ProductCode"]);
                        actualProductCodes.Add(obj.Properties["ProductCode"].Value.ToString());
                    }

                    CollectionAssert.AreEquivalent(expectedFeatures, actualFeatures);
                }
            }

            // Check that a null parameter is invalid.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wifeatureinfo -product $null"))
            {
                TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                {
                    Collection<PSObject> objs = p.Invoke();
                });
            }

            // Check that a collection containing null is invalid.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wifeatureinfo -product @($null)"))
            {
                TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                {
                    Collection<PSObject> objs = p.Invoke();
                });
            }
        }

        [TestMethod]
        [Description("Enumerate specific features for a ProductCode")]
        public void EnumerateSpecificFeatures()
        {
            // Enumerate a single named feature.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wifeatureinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' 'Complete'"))
            {
                Runspace.DefaultRunspace = p.Runspace;
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
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wifeatureinfo -productcode $null"))
            {
                TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                {
                    Collection<PSObject> objs = p.Invoke();
                });
            }

            // Check that a collection containing null is invalid.
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wifeatureinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' @($null)"))
            {
                TestProject.ExpectException(typeof(ParameterBindingException), null, delegate()
                {
                    Collection<PSObject> objs = p.Invoke();
                });
            }
        }
    }
}
