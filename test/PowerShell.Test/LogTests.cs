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

using System;
using System.Globalization;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="Log"/> class.
    /// </summary>
    [TestClass]
    public sealed class LogTests : TestBase
    {
        [TestMethod]
        public void LoggingDefaultPath()
        {
            DateTime start = DateTime.Now;
            using (this.OverrideRegistry())
            {
                var log = new Log(null, start);
                string name = Path.Combine(Path.GetTempPath(), string.Format(CultureInfo.InvariantCulture, "MSI_{0:yyyyMMddhhmmss}", start));

                string extra = null;
                string path = log.Next(extra);

                Assert.AreEqual(
                    InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                    | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                    | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump, log.Mode,
                    "The default logging mode is incorrect.");
                Assert.AreEqual(name + "_000.log", path, true, "The first default log path is incorrect.");

                extra = "test";
                path = log.Next(extra);

                Assert.AreEqual(name + "_001_test.log", path, true, "The second default log path is incorrect.");
            }
        }

        [TestMethod]
        public void LoggingCustomPath()
        {
            DateTime start = DateTime.Now;
            using (this.OverrideRegistry())
            {
                var log = new Log(@"C:\test.txt", start);
                string name = string.Format(CultureInfo.InvariantCulture, @"C:\test_{0:yyyyMMddhhmmss}", start);

                string extra = null;
                string path = log.Next(extra);

                Assert.AreEqual(
                    InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                    | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                    | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump | InstallLogModes.ExtraDebug, log.Mode,
                    "The default logging mode is incorrect.");
                Assert.AreEqual(name + "_000.txt", path, true, "The first custom log path is incorrect.");

                extra = "test";
                path = log.Next(extra);

                Assert.AreEqual(name + "_001_test.txt", path, true, "The second custom log path is incorrect.");
            }
        }

        [TestMethod]
        public void LoggingPolicyMode()
        {
            DateTime start = DateTime.Now;
            using (this.OverrideRegistry())
            {
                // Set the policy to log extra debug information.
                using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer"))
                {
                    key.SetValue("Logging", "voicewarmupx", RegistryValueKind.String);
                }

                var log = new Log(null, start);
                log.Next(null);

                Assert.AreEqual(
                    InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                    | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                    | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump | InstallLogModes.ExtraDebug, log.Mode,
                    "The default logging mode is incorrect.");
            }
        }
    }
}
