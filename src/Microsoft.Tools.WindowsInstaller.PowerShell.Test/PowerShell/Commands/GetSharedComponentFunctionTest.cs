// Functional tests for the get-msisharedcomponentinfo function.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests the get-msisharedcomponentinfo function.
    /// </summary>
    [TestClass]
    public class GetSharedComponentFunctionTest : CommandTestBase
    {
        [TestMethod]
        [Description("A test for Get-MSISharedComponentInfo with no parameters")]
        public void NoParamsTest()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msisharedcomponentinfo"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(5, objs.Count);
                }
            }
        }

        [TestMethod]
        [Description("A test for Get-MSISharedComponentInfo with component GUID")]
        public void ComponentParamTest()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msisharedcomponentinfo -component '{9D8E88E9-8E05-4FC7-AFC7-87759D1D417E}'"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(2, objs.Count);

                }
            }
        }

        [TestMethod]
        [Description("A test for Get-MSISharedComponentInfo with minimum share count")]
        public void CountParamTest()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msisharedcomponentinfo -count 3"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(3, objs.Count);
                }
            }
        }
    }
}
