// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Tests for the <see cref="ReinstallModeAttribute"/> class.
    /// </summary>
    [TestClass]
    public sealed class ReinstallModeAttributeTests
    {
        private const ReinstallModes Default = ReinstallModes.FileOlderVersion | ReinstallModes.MachineData | ReinstallModes.UserData | ReinstallModes.Shortcut;

        [TestMethod]
        public void TransformNullValue()
        {
            var attr = new ReinstallModeAttribute();
            Assert.IsNull(attr.Transform(null, null), "The return value was not null.");
        }

        [TestMethod]
        public void TransformStringValue()
        {
            var attr = new ReinstallModeAttribute();
            Assert.AreEqual<ReinstallModes>(Default, (ReinstallModes)attr.Transform(null, "omus"), "The transformed ReinstallMode is incorrect.");
        }

        [TestMethod]
        public void TransformUnsupportedValue()
        {
            var attr = new ReinstallModeAttribute();
            Assert.AreEqual<int>(0, (int)attr.Transform(null, 0), "The value to be transformed was not returned as-is.");
        }
    }
}
