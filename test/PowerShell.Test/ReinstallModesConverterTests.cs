// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
