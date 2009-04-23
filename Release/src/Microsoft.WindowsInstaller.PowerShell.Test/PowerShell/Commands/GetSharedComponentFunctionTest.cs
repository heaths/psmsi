// Functional tests for the Get-WISharedComponentInfo function.
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests the get-wisharedcomponentinfo function.
    /// </summary>
    [TestClass]
    public class GetSharedComponentFunctionTest : CommandTestBase
    {
        [TestMethod]
        [Description("A test for Get-WISharedComponentInfo with no parameters")]
        public void NoParamsTest()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wisharedcomponentinfo"))
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
        [Description("A test for Get-WISharedComponentInfo with component GUID")]
        public void ComponentParamTest()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wisharedcomponentinfo -component '{CE1F8ECF-0E25-4155-9BE1-E9DC1CADA4C2}'"))
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
        [Description("A test for Get-WISharedComponentInfo with minimum share count")]
        public void CountParamTest()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-wisharedcomponentinfo -count 3"))
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
