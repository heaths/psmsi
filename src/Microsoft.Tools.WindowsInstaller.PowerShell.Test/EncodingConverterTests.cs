// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="EncodingConverter"/> class.
    /// </summary>
    [TestClass]
    public sealed class EncodingConverterTests : TestBase
    {
        [TestMethod]
        public void ConvertFromStringPropertyName()
        {
            var converter = new EncodingConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)), "Cannot convert from a string.");

            var value = (Encoding)converter.ConvertFrom("UTF8");
            Assert.AreEqual<Encoding>(Encoding.UTF8, value, "Could not convert from a string.");
        }

        [TestMethod]
        public void ConvertFromStringEncodingName()
        {
            var converter = new EncodingConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)), "Cannot convert from a string.");

            var value = (Encoding)converter.ConvertFrom("utf-8");
            Assert.AreEqual<Encoding>(Encoding.UTF8, value, "Could not convert from a string.");
        }

        [TestMethod]
        public void ConvertFromNull()
        {
            var converter = new EncodingConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)), "Cannot convert from a string.");

            var value = (Encoding)converter.ConvertFrom(null);
            Assert.AreEqual<Encoding>(Encoding.Default, value, "Could not convert from a null string.");
        }

        [TestMethod]
        public void ConvertFromIntegerCodePage()
        {
            var converter = new EncodingConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(int)), "Cannot convert from an integer.");

            var value = (Encoding)converter.ConvertFrom(65001);
            Assert.AreEqual<Encoding>(Encoding.UTF8, value, "Could not convert from an integer.");
        }

        [TestMethod]
        public void ConvertToString()
        {
            var converter = new EncodingConverter();
            Assert.IsTrue(converter.CanConvertTo(typeof(string)), "Cannot convert to a string");

            var value = (string)converter.ConvertTo(Encoding.UTF8, typeof(string));
            Assert.AreEqual("utf-8", value, true, "Could not convert to a string.");
        }

        [TestMethod]
        public void ConvertToInteger()
        {
            var converter = new EncodingConverter();
            Assert.IsTrue(converter.CanConvertTo(typeof(int)), "Cannot convert to an integer.");

            var value = (int)converter.ConvertTo(Encoding.UTF8, typeof(int));
            Assert.AreEqual<int>(65001, value, "Could not convert to an integer.");
        }

        [TestMethod]
        public void IsNumeric()
        {
            Assert.IsTrue(EncodingConverter.IsNumeric(typeof(short)));
            Assert.IsTrue(EncodingConverter.IsNumeric(typeof(int)));
            Assert.IsTrue(EncodingConverter.IsNumeric(typeof(long)));
            Assert.IsTrue(EncodingConverter.IsNumeric(typeof(ushort)));
            Assert.IsTrue(EncodingConverter.IsNumeric(typeof(uint)));
            Assert.IsTrue(EncodingConverter.IsNumeric(typeof(ulong)));
            Assert.IsFalse(EncodingConverter.IsNumeric(typeof(byte)));
            Assert.IsFalse(EncodingConverter.IsNumeric(typeof(float)));
            Assert.IsFalse(EncodingConverter.IsNumeric(typeof(double)));
        }
    }
}
