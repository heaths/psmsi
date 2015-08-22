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
using System.ComponentModel;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="SystemRestorePoint"/> class.
    /// </summary>
    [TestClass]
    public sealed class SystemRestorePointTests
    {
        [TestMethod]
        public void CreateRestorePoint()
        {
            var service = new SystemRestoreTestService();

            try
            {
                var restorePoint = SystemRestorePoint.Create(RestorePointType.ApplicationInstall, "CreateRestorePoint test", service);
                restorePoint.Commit();

                Assert.AreEqual<string>("CreateRestorePoint test", restorePoint.Description);
                Assert.AreEqual<long>(service.SequenceNumber, restorePoint.SequenceNumber);
            }
            catch (Win32Exception ex)
            {
                if (NativeMethods.ERROR_ACCESS_DENIED == ex.NativeErrorCode)
                {
                    Assert.Inconclusive("Elevation required.");
                }
                else
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public void CancelRestorePoint()
        {
            var service = new SystemRestoreTestService();

            try
            {
                var restorePoint = SystemRestorePoint.Create(RestorePointType.ApplicationInstall, service: service);
                restorePoint.Rollback();

                // Tests the default description since it shouldn't show up (cancelled).
                Assert.AreEqual<string>("Windows Installer PowerShell Module", restorePoint.Description);
                Assert.AreEqual<long>(service.SequenceNumber, restorePoint.SequenceNumber);
            }
            catch (Win32Exception ex)
            {
                if (NativeMethods.ERROR_ACCESS_DENIED == ex.NativeErrorCode)
                {
                    Assert.Inconclusive("Elevation required.");
                }
                else
                {
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void FailRestorePointThrows()
        {
            var service = new SystemRestoreTestService();
            service.SetNextErrorCode(NativeMethods.ERROR_INVALID_DATA);

            try
            {
                SystemRestorePoint.Create(RestorePointType.ApplicationInstall, service: service);
            }
            catch (Win32Exception ex)
            {
                if (NativeMethods.ERROR_ACCESS_DENIED == ex.NativeErrorCode)
                {
                    Assert.Inconclusive("Elevation required.");
                }
                else
                {
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void CancelRestorePointInvalid()
        {
            var restorePoint = SystemRestorePoint.Create(RestorePointType.CancelledOperation);
            Assert.Fail("Creating a system restore point to cancel an operation directly is invalid.");
        }
    }
}
