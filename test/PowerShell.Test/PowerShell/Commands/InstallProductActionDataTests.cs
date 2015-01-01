// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="InstallProductActionData"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallProductActionDataTests : TestBase
    {
        [TestMethod]
        public void SetProductCodeFromMsi()
        {
            using (var p = CreatePipeline(@"get-item example.msi"))
            {
                var file = p.Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                // Suppress any Windows Installer dialogs.
                var internalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    var data = InstallCommandActionData.CreateActionData<InstallProductActionData>(TestRunspace.SessionStateProxy.Path, file);
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
            using (var p = CreatePipeline(@"get-item example.msp"))
            {
                var file = p.Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                // Suppress any Windows Installer dialogs.
                var internalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    var data = InstallCommandActionData.CreateActionData<InstallProductActionData>(TestRunspace.SessionStateProxy.Path, file);
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
            using (var p = CreatePipeline(@"get-item example.txt"))
            {
                var file = p.Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                // Suppress any Windows Installer dialogs.
                var internalUI = Installer.SetInternalUI(InstallUIOptions.Silent);
                try
                {
                    var data = InstallCommandActionData.CreateActionData<InstallProductActionData>(TestRunspace.SessionStateProxy.Path, file);
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
