// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
        [TestCategory("Impactful")]
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
        [TestCategory("Impactful")]
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
