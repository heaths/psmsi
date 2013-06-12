// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Tests for the <see cref="AttributeColumnPropertyAdapter"/> class.
    /// </summary>
    [TestClass]
    public sealed class AttributeColumnPropertyAdapterTests
    {
        [TestMethod]
        public void ColumnAttributesTypeAdapted()
        {
            var adapter = new AttributeColumnPropertyAdapter();
            var column = new AttributeColumn(typeof(ComponentAttributes), 260);

            // Get the names we will expected.
            var expectedNames = Enum.GetNames(column.Type).Select(s => "Has" + s).ToList();
            expectedNames.Add("Value");

            // Make sure all the expected names will be adapted.
            var properties = adapter.GetProperties(column).Select(p => p.Name);
            CollectionAssert.AreEqual(expectedNames.ToArray(), properties.ToArray());

            // Validate the specific HasRegistryKeyPath property using case-insensitive search.
            var property = adapter.GetProperty(column, "hasregistrykeypath");
            Assert.IsNotNull(property);
            Assert.AreEqual("RegistryKeyPath", property.Tag);
            Assert.AreEqual<string>(typeof(bool).FullName, AttributeColumnPropertyAdapter.GetPropertyTypeNameInternal(property, column));
            Assert.IsTrue(adapter.IsGettable(property));
            Assert.IsTrue((bool)AttributeColumnPropertyAdapter.GetPropertyValueInternal(property, column));

            // Validate the specific Value property using case-sensitive search.
            property = adapter.GetProperty(column, "Value");
            Assert.IsNotNull(property);
            Assert.IsNull(property.Tag);
            Assert.AreEqual<string>(typeof(ComponentAttributes).FullName, AttributeColumnPropertyAdapter.GetPropertyTypeNameInternal(property, column));
            Assert.IsTrue(adapter.IsGettable(property));
            Assert.AreEqual<int>(260, (int)AttributeColumnPropertyAdapter.GetPropertyValueInternal(property, column));

            // Validate the PSTypeNames.
            var types = adapter.GetTypeNameHierarchy(column);
            var expectedTypes = new string[]
            {
                typeof(AttributeColumn).FullName + "#ComponentAttributes",
                typeof(AttributeColumn).FullName,
                typeof(object).FullName,
            };
            CollectionAssert.AreEqual(expectedTypes, types);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ColumnAttributesReadOnly()
        {
            var adapter = new AttributeColumnPropertyAdapter();
            var column = new AttributeColumn(typeof(ComponentAttributes), 260);

            // Validate the specific HasRegistryKeyPath property using case-insensitive search.
            var property = adapter.GetProperty(column, "hasregistrykeypath");
            Assert.IsNotNull(property);
            Assert.IsFalse(adapter.IsSettable(property));
            adapter.SetPropertyValue(property, 260);
        }
    }
}
