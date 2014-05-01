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
