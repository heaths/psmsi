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
using System.Globalization;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="AttributeColumn"/> class.
    /// </summary>
    [TestClass]
    public sealed class AttributeColumnTests
    {
        [TestMethod]
        public void ImplicitConversions()
        {
            // To/from int.
            Assert.AreEqual<int>(0, new AttributeColumn(null, 0));
            Assert.AreEqual<int>(0, (AttributeColumn)null);
            Assert.AreEqual<AttributeColumn>(new AttributeColumn(null, 0), 0);

            // To/from int?
            Assert.AreEqual<int?>((int?)null, new AttributeColumn(null, null));
            Assert.AreEqual<int?>((int?)null, (AttributeColumn)null);
            Assert.AreEqual<AttributeColumn>(new AttributeColumn(null, null), (int?)null);
        }

        [TestMethod]
        public void IConvertibleConversions()
        {
            var column = new AttributeColumn(null, 0) as IConvertible;
            Assert.AreEqual<TypeCode>(TypeCode.Int32, column.GetTypeCode());
            Assert.IsFalse(column.ToBoolean(CultureInfo.InvariantCulture));
            Assert.AreEqual<byte>(0, column.ToByte(CultureInfo.InvariantCulture));
            Assert.AreEqual<char>('\0', column.ToChar(CultureInfo.InvariantCulture));
            Assert.AreEqual<decimal>(0, column.ToDecimal(CultureInfo.InvariantCulture));
            Assert.AreEqual<double>(0, column.ToDouble(CultureInfo.InvariantCulture));
            Assert.AreEqual<short>(0, column.ToInt16(CultureInfo.InvariantCulture));
            Assert.AreEqual<int>(0, column.ToInt16(CultureInfo.InvariantCulture));
            Assert.AreEqual<long>(0, column.ToInt64(CultureInfo.InvariantCulture));
            Assert.AreEqual<sbyte>(0, column.ToSByte(CultureInfo.InvariantCulture));
            Assert.AreEqual<float>(0, column.ToSingle(CultureInfo.InvariantCulture));
            Assert.AreEqual("0", column.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual("0", column.ToType(typeof(string), CultureInfo.InvariantCulture));
            Assert.AreEqual<ushort>(0, column.ToUInt16(CultureInfo.InvariantCulture));
            Assert.AreEqual<uint>(0, column.ToUInt32(CultureInfo.InvariantCulture));
            Assert.AreEqual<ulong>(0, column.ToUInt64(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void AttributeColumnEquality()
        {
            var column = new AttributeColumn(null, 0);
            Assert.IsTrue(column.Equals(new AttributeColumn(null, 0)));
            Assert.IsTrue(column.Equals(new AttributeColumn(typeof(ComponentAttributes), 0)));
            Assert.IsFalse(column.Equals(null));
            Assert.IsFalse(column.Equals(0));
        }

        [TestMethod]
        public void AttributeColumnHashCode()
        {
            var column = new AttributeColumn(null, 0);
            Assert.AreEqual<int>(0, column.GetHashCode());
        }

        [TestMethod]
        public void AttributeColumnToString()
        {
            var column = new AttributeColumn(typeof(ComponentAttributes), (int)ComponentAttributes.Shared);
            var value = column.ToString();
            Assert.AreEqual<string>("2048", value);

            column = new AttributeColumn(null, null);
            Assert.IsNull(column.ToString());
        }

        [TestMethod]
        public void AttributeColumnToHexString()
        {
            var column = new AttributeColumn(typeof(ComponentAttributes), (int)ComponentAttributes.Shared);
            var value = column.ToString("X8");
            Assert.AreEqual<string>("0x00000800", value);
        }

        [TestMethod]
        public void AttributeColumnToEnumString()
        {
            var column = new AttributeColumn(typeof(ComponentAttributes), (int)ComponentAttributes.Shared);
            var value = column.ToString("F");
            Assert.AreEqual<string>("Shared", value);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UnsupportedFormatThrows()
        {
            var column = new AttributeColumn(typeof(ComponentAttributes), (int)ComponentAttributes.Shared);
            var value = column.ToString("Z");
        }

        [TestMethod]
        public void AttributeColumnNames()
        {
            var expected = Enum.GetNames(typeof(ComponentAttributes));
            var actual = new AttributeColumn(typeof(ComponentAttributes), 0).GetNames().ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AttributeColumnHasValue()
        {
            var column = new AttributeColumn(typeof(ComponentAttributes), 260);
            Assert.IsTrue(column.HasValue("RegistryKeyPath"));
            Assert.IsTrue(column.HasValue("SixtyFourBit"));
            Assert.IsFalse(column.HasValue("None"));

            column = new AttributeColumn(typeof(ComponentAttributes), 0);
            Assert.IsFalse(column.HasValue("RegistryKeyPath"));
            Assert.IsFalse(column.HasValue("SixtyFourBit"));
            Assert.IsTrue(column.HasValue("None"));

            Assert.IsFalse(column.HasValue("Invalid"));
        }
    }
}
