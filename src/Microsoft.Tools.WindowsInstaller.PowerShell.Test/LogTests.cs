// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="Log"/> class.
    /// </summary>
    [TestClass]
    public sealed class LogTests
    {
        [TestMethod]
        public void LoggingDefaultPath()
        {
            DateTime start = DateTime.Now;
            using (var reg = new MockRegistry())
            {
                // Make sure our logging policy is consistent across machines.
                reg.Import("Registry.xml");

                var log = new Log(null, start);
                string name = Path.Combine(Path.GetTempPath(), string.Format(CultureInfo.InvariantCulture, "MSI_{0:yyyyMMddhhmmss}", start));

                string extra = null;
                string path = log.Next(extra);

                Assert.AreEqual<InstallLogModes>(InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                    | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                    | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump, log.Mode, "The default logging mode is incorrect.");
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
            using (var reg = new MockRegistry())
            {
                // Make sure our logging policy is consistent across machines.
                reg.Import("Registry.xml");

                var log = new Log(@"C:\test.txt", start);
                string name = string.Format(CultureInfo.InvariantCulture, @"C:\test_{0:yyyyMMddhhmmss}", start);

                string extra = null;
                string path = log.Next(extra);

                Assert.AreEqual<InstallLogModes>(InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                    | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                    | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump | InstallLogModes.ExtraDebug, log.Mode, "The default logging mode is incorrect.");
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
            using (var reg = new MockRegistry())
            {
                // Make sure our logging policy is consistent across machines.
                reg.Import("Registry.xml");

                // Set the policy to log extra debug information.
                using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer"))
                {
                    key.SetValue("Logging", "voicewarmupx", RegistryValueKind.String);
                }

                var log = new Log(null, start);
                log.Next(null);

                Assert.AreEqual<InstallLogModes>(InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                    | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                    | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump | InstallLogModes.ExtraDebug, log.Mode, "The default logging mode is incorrect.");
            }
        }
    }
}
