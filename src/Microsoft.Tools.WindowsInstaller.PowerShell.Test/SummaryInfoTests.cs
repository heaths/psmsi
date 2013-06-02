// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="SummaryInfo"/> class.
    /// </summary>
    [TestClass]
    public sealed class SummaryInfoTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SummaryInfoThrows()
        {
            // Casting alone should throw.
            Deployment.WindowsInstaller.SummaryInfo info = null;
            var copy = (SummaryInfo)info;
        }

        [TestMethod]
        public void CopySummaryInfo()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            using (var info = new Deployment.WindowsInstaller.SummaryInfo(path, false))
            {
                var copy = (SummaryInfo)info;

                // Verify that the declared properties are the same.
                var infoProperties = info.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).OrderBy(property => property.Name);
                var copyProperties = copy.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).OrderBy(property => property.Name);

                var infoPropertyNames = infoProperties.Select(property => property.Name).ToArray();
                var copyPropertyNames = infoProperties.Select(property => property.Name).ToArray();
                CollectionAssert.AreEqual(infoPropertyNames, copyPropertyNames, "The set of property names are not the same.");

                var infoPropertyTypes = infoProperties.Select(property => property.PropertyType).ToArray();
                var copyPropertyTypes = copyProperties.Select(property => property.PropertyType).ToArray();
                CollectionAssert.AreEqual(infoPropertyTypes, copyPropertyTypes, "The set of property types are not the same.");

                // Verify that the property values are the same.
                for (int i = 0; i < infoProperties.Count(); ++i)
                {
                    var infoProperty = infoProperties.ElementAt(i);
                    var copyProperty = copyProperties.ElementAt(i);

                    var infoPropertyValue = infoProperty.GetValue(info, null);
                    var copyPropertyValue = copyProperty.GetValue(copy, null);
                    Assert.AreEqual(infoPropertyValue, copyPropertyValue, @"The value for property ""{0}"" differs.", infoProperty.Name);
                }
            }
        }
    }
}
