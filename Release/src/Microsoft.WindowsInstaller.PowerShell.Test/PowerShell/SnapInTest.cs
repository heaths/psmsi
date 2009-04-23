// Unit test class for the SnapIn class.
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
    public class SnapInTest
    {
        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Description"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Description")]
        public void DescriptionTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Windows Installer PowerShell Snap-In", snapIn.Description);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.DescriptionResource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.DescriptionResource")]
        public void DescriptionResourceTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Microsoft.WindowsInstaller.Properties.Resources,SnapIn_Description", snapIn.DescriptionResource);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Formats"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Formats")]
        public void FormatsTest()
        {
            SnapIn snapIn = new SnapIn();
            List<string> formats = new List<string>(snapIn.Formats);
            CollectionAssert.AreEqual(new string[] { @"WindowsInstaller.formats.ps1xml" }, formats, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Name"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Name")]
        public void NameTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"psmsi", snapIn.Name);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Types"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Types")]
        public void TypesTest()
        {
            SnapIn snapIn = new SnapIn();
            List<string> types = new List<string>(snapIn.Types);
            CollectionAssert.AreEqual(new string[] { @"WindowsInstaller.types.ps1xml" }, types, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.Vendor"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.Vendor")]
        public void VendorTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Microsoft Corporation", snapIn.Vendor);
        }

        /// <summary>
        /// A test for <see cref="WindowsInstallerSnapIn.VendorResource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for WindowsInstallerSnapIn.VendorResource")]
        public void VendorResourceTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Microsoft.WindowsInstaller.Properties.Resources,SnapIn_Vendor", snapIn.VendorResource);
        }
    }
}
