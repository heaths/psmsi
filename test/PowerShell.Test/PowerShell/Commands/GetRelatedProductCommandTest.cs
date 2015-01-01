// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
