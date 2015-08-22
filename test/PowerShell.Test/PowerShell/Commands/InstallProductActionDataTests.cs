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
