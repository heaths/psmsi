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

using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                using (var record = new Deployment.WindowsInstaller.Record(2))
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
                var error = psiex.ErrorRecord;

                Assert.IsNotNull(error);
                Assert.AreEqual("Installed TEST", psiex.Message, true);
                Assert.AreEqual("TEST", error.TargetObject as string, true);
            }
        }

        [TestMethod]
        public void ValidateErrorRecordOpenError()
        {
            using (var record = new Deployment.WindowsInstaller.Record(2))
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
            using (var record = new Deployment.WindowsInstaller.Record(6))
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
