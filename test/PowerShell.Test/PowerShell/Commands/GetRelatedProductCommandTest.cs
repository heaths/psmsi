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
using System.Management.Automation.Runspaces;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetRelatedProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetRelatedProductCommandTest : TestBase
    {
        [TestMethod]
        public void EnumerateRelatedProducts()
        {
            using (var p = CreatePipeline(@"get-msirelatedproductinfo -upgradecode '{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}'"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].GetPropertyValue<string>("ProductCode"));
                }
            }
        }

        [TestMethod]
        [WorkItem(9464)]
        public void GetRelatedProductChainedExecution()
        {
            using (Pipeline p = CreatePipeline(@"get-msirelatedproductinfo '{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}' | add-member -name UpgradeCode -type noteproperty -value '{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}' -passthru | get-msirelatedproductinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].GetPropertyValue<string>("ProductCode"));
                }
            }
        }
    }
}
