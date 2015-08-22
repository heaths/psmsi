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

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    [TestClass]
    public sealed class ReinstallModeAttributeTests : TestBase
    {
        private const ReinstallModes Default = ReinstallModes.FileOlderVersion | ReinstallModes.MachineData | ReinstallModes.UserData | ReinstallModes.Shortcut;

        [TestMethod]
        public void TransformNullReinstallModeValue()
        {
            var attr = new ReinstallModeAttribute();
            Assert.IsNull(attr.Transform(null, null));
        }

        [TestMethod]
        public void TransformStringReinstallModeValue()
        {
            var attr = new ReinstallModeAttribute();
            Assert.AreEqual<ReinstallModes>(Default, (ReinstallModes)attr.Transform(null, "omUS"));
        }

        [TestMethod]
        public void TransformUnsupportedReinstallModeValue()
        {
            var attr = new ReinstallModeAttribute();
            Assert.AreEqual<int>(0, (int)attr.Transform(null, 0));
        }
    }
}
