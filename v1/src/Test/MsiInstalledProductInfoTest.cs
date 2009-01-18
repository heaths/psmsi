// Tests the InstalledProductInfo class.
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
    /// A test for the <see cref="InstalledProductInfo"/> class.
    /// </summary>
    [TestClass]
    public class InstalledProductInfoTest
    {
        /// <summary>
        /// A test for all installed properties.
        /// </summary>
        [TestMethod]
        [Description("A test for all installed properties")]
        [DeploymentItem(@"data\registry.xml")]
        public void AllPropertiesTest()
        {
            using (MockRegistry reg = new MockRegistry())
            {
                reg.Import(@"registry.xml");

                // Make sure our data represents an advertised product.
                ProductInfo temp = ProductInfo.Create("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}");
                Assert.IsInstanceOfType(temp, typeof(InstalledProductInfo));

                // Check all the properties.
                InstalledProductInfo product = (InstalledProductInfo)temp;
                Assert.AreEqual<string>("Microsoft Silverlight", product.InstalledProductName);
                Assert.AreEqual<string>("2.0.30226.2", product.VersionString);
                Assert.AreEqual<string>(@"http://go.microsoft.com/fwlink/?LinkID=91955", product.HelpLink);
                Assert.IsNull(product.HelpTelephone);
                Assert.IsNull(product.InstallLocation);
                Assert.AreEqual<string>(@"c:\2014be2e43e417a3b9\", product.InstallSource);
                Assert.AreEqual<DateTime>(DateTime.Parse(@"3/3/2008 12:00:00 AM"), product.InstallDate);
                Assert.AreEqual<string>("Microsoft Corporation", product.Publisher);
                Assert.AreEqual<string>(@"c:\Windows\Installer\16ca06.msi", product.LocalPackage);
                Assert.IsNull(product.UrlInfoAbout);
                Assert.IsNull(product.UrlUpdateInfo);
                Assert.AreEqual<int>(0, product.VersionMinor);
                Assert.AreEqual<int>(2, product.VersionMajor);
                Assert.IsNull(product.ProductId);
                Assert.IsNull(product.RegCompany);
                Assert.IsNull(product.RegOwner);
                Assert.AreEqual<ProductState>(ProductState.Installed, product.ProductState);
                Assert.AreEqual<string>("silverlight.msi", product.PackageName);
                Assert.IsNull(product.Transforms);
                Assert.AreEqual<string>("1033", product.Language);
                Assert.AreEqual<string>("Microsoft Silverlight", product.ProductName);
                Assert.AreEqual<AssignmentType>(AssignmentType.Machine, product.AssignmentType);
                Assert.AreEqual<InstanceType>(InstanceType.None, product.InstanceType);
                Assert.IsTrue(product.AuthorizedLUAApp);
                Assert.AreEqual<string>("{C0ED0C05-06B1-4A49-9C02-AE75A17D8E30}", product.PackageCode);
                Assert.AreEqual<string>("33584658", product.Version);
                Assert.AreEqual<string>(@"c:\Windows\Installer\{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}\ARPIcon", product.ProductIcon);
                Assert.AreEqual<string>(@"n;1;c:\2014be2e43e417a3b9\", product.LastUsedSource);
                Assert.AreEqual<string>(@"n;1;c:\2014be2e43e417a3b9\", product.LastUsedType);
                Assert.IsNull(product.MediaPackagePath);
                Assert.IsNull(product.DiskPrompt);
            }
        }
    }
}
