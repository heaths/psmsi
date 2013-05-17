// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="RepairCommandActionData"/> class.
    /// </summary>
    [TestClass]
    public sealed class RepairCommandActionDataTests
    {
        [TestMethod]
        public void AssertDefaultReinstallMode()
        {
            var data = new RepairCommandActionData();
            Assert.AreEqual<ReinstallModes>(ReinstallModes.FileOlderVersion | ReinstallModes.MachineData | ReinstallModes.UserData | ReinstallModes.Shortcut, data.ReinstallMode, "The default ReinstallMode is incorrect.");
        }
    }
}
