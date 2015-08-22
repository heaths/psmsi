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
