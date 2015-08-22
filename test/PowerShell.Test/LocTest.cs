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

using Microsoft.Tools.WindowsInstaller.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests localization of the project using pseudo-localization files.
    /// </summary>
    /// <remarks>
    /// To enable this on Windows Vista and newer, see http://msdn.microsoft.com/en-us/library/aa366908.aspx.
    /// </remarks>
    [TestClass]
    public sealed class LocTest : TestBase
    {
        [TestMethod]
        public void PlocCmdletHelpTest()
        {
            using (var p = CreatePipeline(@"set-uiculture 'qps-ploc'; get-help get-msiproductinfo"))
            {
                var objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<string>(@"{Gééts próódüüçt ííñformââtioñ for registered produçts.}", objs[0].GetPropertyValue<string>("Synopsis"));
            }
        }

        [TestMethod]
        public void PlocFunctionHelpTest()
        {
            using (var p = CreatePipeline(@"set-uiculture 'qps-ploc'; get-help get-msisharedcomponentinfo"))
            {
                var objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<string>(@"{Gééts ííñfóórmââtioñ aboüüt shared çompoñeñts iñstalled or registered for the çurreñt user or the maçhiñe.}", objs[0].GetPropertyValue<string>("Synopsis"));
            }
        }
    }
}
