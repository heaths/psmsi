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
