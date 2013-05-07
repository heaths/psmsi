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
    public sealed class ReinstallModesConverterTests
    {
        private const ReinstallModes Default = ReinstallModes.FileOlderVersion | ReinstallModes.MachineData | ReinstallModes.UserData | ReinstallModes.Shortcut;

        [TestMethod]
        public void ConvertReinstallModesToShortForm()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertTo(typeof(string)), "Cannot convert to type String.");

            var mode = (string)converter.ConvertTo(Default, typeof(string));
            Assert.AreEqual("omus", mode, "The REINSTALLMODE short form is incorrect.");
        }

        [TestMethod]
        public void ConvertNamesToReinstallModes()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)), "Cannot convert from type String.");

            // Use mixed case to test case-insensitivity.
            var mode = (ReinstallModes)converter.ConvertFrom("FileOlderVersion, MachineData, userData, shortcut");
            Assert.AreEqual(Default, mode, "The ReinstallModes is incorrect.");
        }

        [TestMethod]
        public void ConvertShortFormToReinstallModes()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)), "Cannot convert from type String.");

            var mode = (ReinstallModes)converter.ConvertFrom("omus");
            Assert.AreEqual(Default, mode, "The ReinstallModes is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertInvalidStringToReinstallModes()
        {
            var converter = new ReinstallModesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)), "Cannot convert from type String.");

            // Should throw ArgumentException.
            var mode = (ReinstallModes)converter.ConvertFrom("xyz");
        }
    }
}
