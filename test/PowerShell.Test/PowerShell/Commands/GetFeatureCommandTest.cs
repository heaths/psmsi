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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetFeatureCommand"/>.
    ///</summary>
    [TestClass]
    public class GetFeatureCommandTest : TestBase
    {
        [TestMethod]
        public void EnumerateProductFeatures()
        {
            var expectedFeatures = new Collection<string>();
            var expectedProductCodes = new Collection<string>();

            // Populate expected features.
            expectedFeatures.Add("Complete");
            expectedFeatures.Add("Complete2.0.30226.2");
            expectedFeatures.Add("TEST");
            expectedFeatures.Add("Module");

            // Populate expected ProductCodes.
            expectedProductCodes.Add("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");
            expectedProductCodes.Add("{B4EA7821-1AC1-41B5-8021-A2FC77D1B7B7}");
            expectedProductCodes.Add("{877EF582-78AF-4D84-888B-167FDC3BCC11}");

            // Check the number of features for a product object.
            using (var p = CreatePipeline(@"get-msiproductinfo -context 'all' | get-msifeatureinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(expectedFeatures.Count, objs.Count);

                    var actualFeatures = new Collection<string>();
                    var actualProductCodes = new Collection<string>();

                    foreach (var obj in objs)
                    {
                        // Use the ETS-added Name property as an additional check.
                        Assert.IsNotNull(obj.Properties["Name"]);
                        actualFeatures.Add(obj.GetPropertyValue<string>("Name"));

                        // Use the ETS-added ProductCode property as an additional check.
                        Assert.IsNotNull(obj.Properties["ProductCode"]);
                        actualProductCodes.Add(obj.GetPropertyValue<string>("ProductCode"));
                    }

                    CollectionAssert.AreEquivalent(expectedFeatures, actualFeatures);
                }
            }

            // Check that a null parameter is invalid.
            using (var p = CreatePipeline(@"get-msifeatureinfo -product $null"))
            {
                ExceptionAssert.Throws<ParameterBindingException>(() =>
                {
                    p.Invoke();
                });
            }

            // Check that a collection containing null is invalid.
            using (var p = CreatePipeline(@"get-msifeatureinfo -product @($null)"))
            {
                ExceptionAssert.Throws<ParameterBindingException>(() =>
                {
                    p.Invoke();
                });
            }
        }

        [TestMethod]
        public void EnumerateSpecificFeatures()
        {
            // Enumerate a single named feature.
            using (var p = CreatePipeline(@"get-msifeatureinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' 'Complete'"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.IsNotNull(objs[0].Properties["ProductCode"]);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].GetPropertyValue<string>("ProductCode"));
                    Assert.IsNotNull(objs[0].Properties["Name"]);
                    Assert.AreEqual<string>("Complete", objs[0].GetPropertyValue<string>("Name"));
                }
            }

            // Check that a null parameter is invalid.
            using (var p = CreatePipeline(@"get-msifeatureinfo -productcode $null"))
            {
                ExceptionAssert.Throws<ParameterBindingException>(() =>
                {
                    p.Invoke();
                });
            }

            // Check that a collection containing null is invalid.
            using (var p = CreatePipeline(@"get-msifeatureinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' @($null)"))
            {
                ExceptionAssert.Throws<ParameterBindingException>(() =>
                {
                    p.Invoke();
                });
            }
        }

        [TestMethod]
        [WorkItem(9464)]
        public void GetFeatureChainedExecution()
        {
            var expectedFeatures = new Collection<string>();
            expectedFeatures.Add("Complete");
            expectedFeatures.Add("Complete2.0.30226.2");

            using (var p = CreatePipeline(@"get-msiproductinfo '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' | get-msifeatureinfo | get-msifeatureinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();
                    Assert.AreEqual(2, objs.Count);

                    var actualFeatures = new Collection<string>();
                    foreach (var obj in objs)
                    {
                        Assert.IsNotNull(obj.Properties["FeatureName"]);
                        actualFeatures.Add(obj.GetPropertyValue<string>("FeatureName"));
                    }

                    CollectionAssert.AreEquivalent(expectedFeatures, actualFeatures);
                }
            }
        }
    }
}
