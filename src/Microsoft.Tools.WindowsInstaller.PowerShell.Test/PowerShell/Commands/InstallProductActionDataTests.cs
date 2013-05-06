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

                var data = InstallProductActionData.CreateActionData<InstallProductActionData>(ps.Runspace.SessionStateProxy.Path, file);
                data.SetProductCode();
                Assert.AreEqual("{5F45B2AD-6B18-40EF-AA07-083876579689}", data.ProductCode, "The ProductCode is incorrect.");
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

                var data = InstallProductActionData.CreateActionData<InstallProductActionData>(ps.Runspace.SessionStateProxy.Path, file);
                data.SetProductCode();
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

                var data = InstallProductActionData.CreateActionData<InstallProductActionData>(ps.Runspace.SessionStateProxy.Path, file);
                data.SetProductCode();
            }
        }
    }
}
