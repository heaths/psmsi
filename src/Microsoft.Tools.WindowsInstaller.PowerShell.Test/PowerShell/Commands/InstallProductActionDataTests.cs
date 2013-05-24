// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using PS = System.Management.Automation.PowerShell;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="InstallProductActionData"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallProductActionDataTests
    {
        [TestMethod]
        public void SetProductCodeFromMsi()
        {
            using (var ps = PS.Create())
            {
                var file = ps.AddCommand("get-item").AddArgument("example.msi").Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                // Suppress any Windows Installer dialogs.
                var internalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    var data = InstallCommandActionData.CreateActionData<InstallProductActionData>(ps.Runspace.SessionStateProxy.Path, file);
                    data.SetProductCode();
                    Assert.AreEqual("{877EF582-78AF-4D84-888B-167FDC3BCC11}", data.ProductCode, "The ProductCode is incorrect.");
                }
                finally
                {
                    Installer.SetInternalUI(internalUI);
                }
            }
        }

        [TestMethod]
        public void SetProductCodeFromMsp()
        {
            using (var ps = PS.Create())
            {
                var file = ps.AddCommand("get-item").AddArgument("example.msp").Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                // Suppress any Windows Installer dialogs.
                var internalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    var data = InstallCommandActionData.CreateActionData<InstallProductActionData>(ps.Runspace.SessionStateProxy.Path, file);
                    data.SetProductCode();
                    Assert.IsNull(data.ProductCode, "The ProductCode is incorrect.");
                }
                finally
                {
                    Installer.SetInternalUI(internalUI);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InstallerException))]
        public void SetProductCodeFromTxt()
        {
            using (var ps = PS.Create())
            {
                var file = ps.AddCommand("get-item").AddArgument("example.txt").Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                // Suppress any Windows Installer dialogs.
                var internalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    var data = InstallCommandActionData.CreateActionData<InstallProductActionData>(ps.Runspace.SessionStateProxy.Path, file);
                    data.SetProductCode();
                }
                finally
                {
                    Installer.SetInternalUI(internalUI);
                }
            }
        }
    }
}
