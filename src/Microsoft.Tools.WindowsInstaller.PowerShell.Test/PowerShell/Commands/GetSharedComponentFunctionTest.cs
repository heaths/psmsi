// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests the get-msisharedcomponentinfo function.
    /// </summary>
    [TestClass]
    public class GetSharedComponentFunctionTest : TestBase
    {
        [TestMethod]
        public void NoParamsTest()
        {
            using (var p = CreatePipeline(@"get-msisharedcomponentinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();
                    Assert.AreEqual<int>(5, objs.Count);
                }
            }
        }

        [TestMethod]
        public void ComponentParamTest()
        {
            using (var p = CreatePipeline(@"get-msisharedcomponentinfo -component '{9D8E88E9-8E05-4FC7-AFC7-87759D1D417E}'"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();
                    Assert.AreEqual<int>(2, objs.Count);

                }
            }
        }

        [TestMethod]
        public void CountParamTest()
        {
            using (var p = CreatePipeline(@"get-msisharedcomponentinfo -count 3"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();
                    Assert.AreEqual<int>(3, objs.Count);
                }
            }
        }
    }
}
