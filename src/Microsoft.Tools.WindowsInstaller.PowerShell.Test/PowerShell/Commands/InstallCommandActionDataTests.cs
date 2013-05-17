// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using PS = System.Management.Automation.PowerShell;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="InstallCommandActionData"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallCommandActionDataTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestNullCommandLineArgs()
        {
            var data = new InstallCommandActionData();
            data.ParseCommandLine(null);

            Assert.IsNull(data.CommandLine, "The command line should be null.");
        }

        [TestMethod]
        public void TestEmptyCommandLineArgs()
        {
            var data = new InstallCommandActionData();
            data.ParseCommandLine(new string[] { });

            Assert.IsNull(data.CommandLine, "The command line should be null.");
        }

        [TestMethod]
        public void TestCommandLineArgs()
        {
            var data = new InstallCommandActionData();
            data.ParseCommandLine(new string[] { "PROPERTY=VALUE",  "REINSTALL=ALL" });

            Assert.AreEqual("PROPERTY=VALUE REINSTALL=ALL", data.CommandLine, "The command line is incorrect.");
        }

        [TestMethod]
        public void ExtractPathFromPSPath()
        {
            using (var ps = PS.Create())
            {
                var file = ps.AddCommand("get-item").AddArgument("example.msi").Invoke().SingleOrDefault();
                Assert.IsNotNull(file, "The file item is null.");

                var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(ps.Runspace.SessionStateProxy.Path, file);
                string path = Path.Combine(this.TestContext.DeploymentDirectory, "example.msi");
                Assert.AreEqual(path, data.Path, "The paths are not the same.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullResolver()
        {
            var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullFile()
        {
            using (var ps = PS.Create())
            {
                var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(ps.Runspace.SessionStateProxy.Path, null);
            }
        }
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
                    var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(ps.Runspace.SessionStateProxy.Path, file);
                    data.SetProductCode();
                    Assert.AreEqual("{5F45B2AD-6B18-40EF-AA07-083876579689}", data.ProductCode, "The ProductCode is incorrect.");
                }
                finally
                {
                    Installer.SetInternalUI(internalUI);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InstallerException))]
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
                    var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(ps.Runspace.SessionStateProxy.Path, file);
                    data.SetProductCode();
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
                    var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(ps.Runspace.SessionStateProxy.Path, file);
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
