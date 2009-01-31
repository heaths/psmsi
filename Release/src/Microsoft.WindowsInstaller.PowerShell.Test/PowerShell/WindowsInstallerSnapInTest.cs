// Unit test class for the Get-MSIProduct cmdlet.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="WindowsInstallerSnapIn"/> class.
    /// </summary>
    [TestClass]
    public class WindowsInstallerSnapInTest
    {
        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Description"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Description")]
        public void DescriptionTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            Assert.AreEqual<string>(@"Windows Installer PowerShell Snap-In", snapIn.Description);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.DescriptionResource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.DescriptionResource")]
        public void DescriptionResourceTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            Assert.AreEqual<string>(@"Microsoft.WindowsInstaller.Properties.Resources,SnapIn_Description", snapIn.DescriptionResource);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Formats"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Formats")]
        public void FormatsTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            List<string> formats = new List<string>(snapIn.Formats);
            Assert.IsTrue(formats.Contains("formats.ps1xml"));
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Name"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Name")]
        public void NameTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            Assert.AreEqual<string>(@"psmsi", snapIn.Name);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Types"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Types")]
        public void TypesTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            List<string> types = new List<string>(snapIn.Types);
            Assert.IsTrue(types.Contains("types.ps1xml"));
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Vendor"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Vendor")]
        public void VendorTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            Assert.AreEqual<string>(@"Microsoft Corporation", snapIn.Vendor);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.VendorResource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.VendorResource")]
        public void VendorResourceTest()
        {
            WindowsInstallerSnapIn snapIn = new WindowsInstallerSnapIn();
            Assert.AreEqual<string>(@"Microsoft.WindowsInstaller.Properties.Resources,SnapIn_Vendor", snapIn.VendorResource);
        }
    }
}
