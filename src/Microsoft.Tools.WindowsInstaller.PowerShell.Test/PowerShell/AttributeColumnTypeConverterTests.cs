// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Tests for the <see cref="AttributeColumnTypeConverter"/> class.
    /// </summary>
    [TestClass]
    public sealed class AttributeColumnTypeConverterTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertIntegerToAttributeColumn()
        {
            var converter = new AttributeColumnTypeConverter();
            Assert.IsFalse(converter.CanConvertFrom(0, typeof(AttributeColumn)));

            converter.ConvertFrom(0, typeof(AttributeColumn), null, false);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertAttributeColumnToString()
        {
            var converter = new AttributeColumnTypeConverter();
            var column = new AttributeColumn(null, 42);

            Assert.IsFalse(converter.CanConvertTo(column, typeof(string)));
            converter.ConvertTo(column, typeof(string), CultureInfo.InvariantCulture, false);
        }

        [TestMethod]
        public void ConvertNullAttributeColumnToInteger()
        {
            var converter = new AttributeColumnTypeConverter();
            Assert.IsFalse(converter.CanConvertTo(null, typeof(int)));
        }

        [TestMethod]
        public void ConvertAttributeColumnToInteger()
        {
            var converter = new AttributeColumnTypeConverter();
            var column = new AttributeColumn(null, 42);

            Assert.IsTrue(converter.CanConvertTo(column, typeof(int)));
            int value = (int)converter.ConvertTo(column, typeof(int), CultureInfo.InvariantCulture, false);
            Assert.AreEqual<int>(42, value);
        }

        [TestMethod]
        public void ConvertAttributeColumnToNullableInteger()
        {
            var converter = new AttributeColumnTypeConverter();
            var column = new AttributeColumn(null, null);

            Assert.IsTrue(converter.CanConvertTo(column, typeof(int?)));
            int? value = (int?)converter.ConvertTo(column, typeof(int?), CultureInfo.InvariantCulture, false);
            Assert.IsNull(value);
        }
    }
}
