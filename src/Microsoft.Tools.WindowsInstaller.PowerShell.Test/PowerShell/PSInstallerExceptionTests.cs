// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Test classes for <see cref="PSInstallerException"/>.
    /// </summary>
    [TestClass]
    public sealed class PSInstallerExceptionTests : TestBase
    {
        [TestMethod]
        public void DeserializePSInstallerException()
        {
            PSInstallerException ex;
            var formatter = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                // Serialize the error record.
                using (var record = new Record(2))
                {
                    record.FormatString = "Installed [2]";
                    record.SetInteger(1, 1715);
                    record.SetString(2, "TEST");

                    using (ex = new PSInstallerException(record))
                    {
                        formatter.Serialize(ms, ex);
                    }
                }

                // Reset the position.
                ms.Position = 0;

                // Deserialize the error record.
                using (ex = formatter.Deserialize(ms) as PSInstallerException)
                {
                    Assert.IsNotNull(ex);

                    var error = ex.ErrorRecord;
                    Assert.IsNotNull(error);
                    Assert.AreEqual("Installed TEST", ex.Message, true);
                    Assert.AreEqual("TEST", error.TargetObject as string, true);
                }
            }
        }

        [TestMethod]
        public void CreateFromInstallerExceptionForRecord()
        {
            // Construct an InstallerException from Windows Installer record data.
            var iex = new InstallerException();
            var data = new object[] { 1715, "TEST" };
            iex.GetType().GetField("errorData", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(iex, data);

            using (var psiex = new PSInstallerException(iex))
            {
                var error= psiex.ErrorRecord;

                Assert.IsNotNull(error);
                Assert.AreEqual("Installed TEST", psiex.Message, true);
                Assert.AreEqual("TEST", error.TargetObject as string, true);
            }
        }

        [TestMethod]
        public void ValidateErrorRecordOpenError()
        {
            using (var record = new Record(2))
            {
                record.SetInteger(1, 1301);
                record.SetString(2, @"C:\test.txt");

                using (var ex = new PSInstallerException(record))
                {
                    var error = ex.ErrorRecord;
                    Assert.IsNotNull(error);
                    Assert.AreEqual(@"C:\test.txt", error.TargetObject as string, true);

                    var info = error.CategoryInfo;
                    Assert.IsNotNull(info);
                    Assert.AreEqual(ErrorCategory.WriteError, info.Category);
                }
            }
        }

        [TestMethod]
        public void ValidateErrorRecordForFusion()
        {
            using (var record = new Record(6))
            {
                record.SetInteger(1, 1935);
                record.SetString(2, "TestComponent");
                record.SetInteger(3, unchecked((int)0x80070005));
                record.SetString(4, "ITestInterface");
                record.SetString(5, "TestFunction");
                record.SetString(6, "TestAssembly");

                using (var ex = new PSInstallerException(record))
                {
                    var error = ex.ErrorRecord;
                    Assert.IsNotNull(error);
                    Assert.AreEqual("TestAssembly", error.TargetObject as string, true);

                    var info = error.CategoryInfo;
                    Assert.IsNotNull(info);
                    Assert.AreEqual(ErrorCategory.InvalidData, info.Category);
                }
            }
        }
    }
}
