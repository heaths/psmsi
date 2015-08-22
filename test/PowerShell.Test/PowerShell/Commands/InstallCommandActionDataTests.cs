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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="InstallCommandActionData"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallCommandActionDataTests : TestBase
    {
        [TestMethod]
        public void TestNullCommandLineArgs()
        {
            var data = new InstallCommandActionData();
            data.ParseCommandLine(null);

            Assert.IsNull(data.CommandLine);
            Assert.AreEqual<long>(PackageInfo.DefaultWeight, data.Weight);
        }

        [TestMethod]
        public void TestEmptyCommandLineArgs()
        {
            var data = new InstallCommandActionData();
            data.ParseCommandLine(new string[] { });

            Assert.IsNull(data.CommandLine);
            Assert.AreEqual<long>(PackageInfo.DefaultWeight, data.Weight);
        }

        [TestMethod]
        public void TestCommandLineArgs()
        {
            var data = new InstallCommandActionData();
            data.ParseCommandLine(new string[] { "PROPERTY=VALUE",  "REINSTALL=ALL" });

            Assert.AreEqual("PROPERTY=VALUE REINSTALL=ALL", data.CommandLine);
            Assert.AreEqual<long>(PackageInfo.DefaultWeight, data.Weight);
        }

        [TestMethod]
        public void ExtractPathFromPSPath()
        {
            using (var p = CreatePipeline("get-item example.msi"))
            {
                var file = p.Invoke().SingleOrDefault();
                Assert.IsNotNull(file);

                var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(TestRunspace.SessionStateProxy.Path, file);
                string path = Path.Combine(this.TestContext.DeploymentDirectory, "example.msi");
                Assert.AreEqual(path, data.Path);

                data.UpdateWeight();
                Assert.AreEqual<long>(1419, data.Weight);
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
            var data = InstallCommandActionData.CreateActionData<InstallCommandActionData>(TestRunspace.SessionStateProxy.Path, null);
        }
    }
}
