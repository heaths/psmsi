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

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var copy = new SummaryInfo(info);
        }

        [TestMethod]
        public void CopySummaryInfo()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            using (var info = new Deployment.WindowsInstaller.SummaryInfo(path, false))
            {
                var copy = new SummaryInfo(info);

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
