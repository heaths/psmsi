// Tests the PatchInfo class.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer
{
    /// <summary>
    /// A test for the <see cref="PatchInfo"/> class.
    /// </summary>
    [TestClass]
    public class PatchInfoTest
    {
        /// <summary>
        /// A test for all patch properties.
        /// </summary>
        [TestMethod]
        [Description("A test for all patch properties")]
        [DeploymentItem(@"data\registry.xml")]
        public void AllPropertiesTest()
        {
            using (MockRegistry reg = new MockRegistry())
            {
                reg.Import(@"registry.xml");

                // Check all the properties.
                PatchInfo patch = new PatchInfo("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", "{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", null, InstallContext.Machine);
                Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", patch.PatchCode);
                Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", patch.ProductCode);
                Assert.IsNull(patch.UserSid);
                Assert.AreEqual<InstallContext>(InstallContext.Machine, patch.InstallContext);
                Assert.IsTrue(patch.Uninstallable);
                Assert.AreEqual<PatchStates>(PatchStates.Applied, patch.PatchState);
                Assert.IsFalse(patch.LUAEnabled);
                Assert.AreEqual<string>("Microsoft Silverlight 2.0.30226.2", patch.DisplayName);
                Assert.AreEqual<string>(@"http://go.microsoft.com/fwlink/?LinkID=91955", patch.MoreInfoUrl);
                Assert.AreEqual<string>(@"c:\Windows\Installer\16ca0c.msp", patch.LocalPackage);
            }
        }
    }
}
