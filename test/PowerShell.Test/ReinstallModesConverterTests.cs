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
using System;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="ReinstallModesConverter"/> class.
    /// </summary>
    [TestClass]
    public sealed class ReinstallModesConverterTests : TestBase
    {
        private const ReinstallModes Default = ReinstallModes.FileOlderVersion | ReinstallModes.MachineData | ReinstallModes.UserData | ReinstallModes.Shortcut;

        [TestMethod]
        public void ConvertReinstallModesToShortForm()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertTo(typeof(string)));

            var mode = (string)converter.ConvertTo(Default, typeof(string));
            Assert.AreEqual("omus", mode);
        }

        [TestMethod]
        public void ConvertNamesToReinstallModes()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)));

            // Use mixed case to test case-insensitivity.
            var mode = (ReinstallModes)converter.ConvertFrom("FileOlderVersion, MachineData, userData, shortcut");
            Assert.AreEqual(Default, mode);
        }

        [TestMethod]
        public void ConvertShortFormToReinstallModes()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)));

            var mode = (ReinstallModes)converter.ConvertFrom("omUS");
            Assert.AreEqual(Default, mode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertInvalidStringToReinstallModes()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)));

            // Should throw ArgumentException.
            var mode = (ReinstallModes)converter.ConvertFrom("xyz");
        }
    }
}
