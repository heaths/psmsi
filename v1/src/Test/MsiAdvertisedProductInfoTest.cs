// Tests the AdvertisedProductInfo class.
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
    /// A test for the <see cref="AdvertisedProductInfo"/> class.
    /// </summary>
    [TestClass]
    public class AdvertisedProductInfoTest
    {
        /// <summary>
        /// A test for all advertised properties.
        /// </summary>
        [TestMethod]
        [Description("A test for all advertised properties")]
        [DeploymentItem(@"data\registry.xml")]
        public void AllPropertiesTest()
        {
            using (MockRegistry reg = new MockRegistry())
            {
                reg.Import(@"registry.xml");

                // Make sure our data represents an advertised product.
                ProductInfo temp = ProductInfo.Create("{0CABECAC-4E23-4928-871A-6E65CD370F9F}");
                Assert.IsInstanceOfType(temp, typeof(AdvertisedProductInfo));

                // Check all the properties.
                AdvertisedProductInfo product = (AdvertisedProductInfo)temp;
                Assert.AreEqual<ProductState>(ProductState.Advertised, product.ProductState);
                Assert.AreEqual<string>("psmsi.msi", product.PackageName);
                Assert.IsNull(product.Transforms);
                Assert.AreEqual<string>("1033", product.Language);
                Assert.AreEqual<string>("Windows Installer PowerShell Extensions", product.ProductName);
                Assert.AreEqual<AssignmentType>(AssignmentType.Machine, product.AssignmentType);
                Assert.AreEqual<InstanceType>(InstanceType.None, product.InstanceType);
                Assert.IsFalse(product.AuthorizedLUAApp);
                Assert.AreEqual<string>("{9F4F0897-AD6C-484A-AF5E-DF51D0D38D7B}", product.PackageCode);
                Assert.AreEqual<string>("16807534", product.Version);
                Assert.IsNull(product.ProductIcon);
                Assert.IsNull(product.LastUsedSource);
                Assert.IsNull(product.LastUsedType);
                Assert.IsNull(product.MediaPackagePath);
                Assert.IsNull(product.DiskPrompt);
            }
        }
    }
}
