// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="SnapIn"/> class.
    /// </summary>
    [TestClass]
    public class SnapInTest
    {
        /// <summary>
        /// A test for <see cref="SnapIn.Description"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.Description")]
        public void DescriptionTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Windows Installer PowerShell Snap-In", snapIn.Description);
        }

        /// <summary>
        /// A test for <see cref="SnapIn.DescriptionResource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.DescriptionResource")]
        public void DescriptionResourceTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Microsoft.Tools.WindowsInstaller.Properties.Resources,SnapIn_Description", snapIn.DescriptionResource);
        }

        /// <summary>
        /// A test for <see cref="SnapIn.Formats"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.Formats")]
        public void FormatsTest()
        {
            SnapIn snapIn = new SnapIn();
            List<string> formats = new List<string>(snapIn.Formats);
            CollectionAssert.AreEqual(new string[] { @"MSI.formats.ps1xml" }, formats, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// A test for <see cref="SnapIn.Name"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.Name")]
        public void NameTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"psmsi", snapIn.Name);
        }

        /// <summary>
        /// A test for <see cref="SnapIn.Types"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.Types")]
        public void TypesTest()
        {
            SnapIn snapIn = new SnapIn();
            List<string> types = new List<string>(snapIn.Types);
            CollectionAssert.AreEqual(new string[] { @"MSI.types.ps1xml" }, types, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// A test for <see cref="SnapIn.Vendor"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.Vendor")]
        public void VendorTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Microsoft Corporation", snapIn.Vendor);
        }

        /// <summary>
        /// A test for <see cref="SnapIn.VendorResource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for SnapIn.VendorResource")]
        public void VendorResourceTest()
        {
            SnapIn snapIn = new SnapIn();
            Assert.AreEqual<string>(@"Microsoft.Tools.WindowsInstaller.Properties.Resources,SnapIn_Vendor", snapIn.VendorResource);
        }
    }
}
