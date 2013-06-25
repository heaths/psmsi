// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Provides additional properties for column types with associated enumerations.
    /// </summary>
    public sealed class AttributeColumn : IConvertible, IFormattable
    {
        // Used as a fallback when the enumeration type cannot be determined.
        // The framework will return the integer value for undefined enumeration values.
        private enum UndefinedType
        {
            None,
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AttributeColumn"/> class.
        /// </summary>
        /// <param name="type">The type of the enumeration.</param>
        /// <param name="value">The value of the enumeration.</param>
        internal AttributeColumn(Type type, int? value)
        {
            this.Type = type ?? typeof(UndefinedType);
            this.Value = value;
        }

        /// <summary>
        /// Gets the type of the enumeration.
        /// </summary>
        internal Type Type { get; private set; }

        /// <summary>
        /// Gets the value of the enumeration.
        /// </summary>
        internal int? Value { get; private set; }

        /// <summary>
        /// Gets whether the internal values are equal.
        /// </summary>
        /// <param name="obj">The object to compre.</param>
        /// <returns>True if the objects are equivalent.</returns>
        public override bool Equals(object obj)
        {
            var value = obj as AttributeColumn;
            if (null != value)
            {
                return this.Value == value.Value;
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code from the internal value.
        /// </summary>
        /// <returns>The hash code from the intrnal value.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the internal value using the <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <returns>A string representation of the internal value.</returns>
        public override string ToString()
        {
            var formattable = (IFormattable)this;
            return formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of the internal value using the <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <param name="format">The format specification.</param>
        /// <returns>A string representation of the internal value.</returns>
        public string ToString(string format)
        {
            var formattable = (IFormattable)this;
            return formattable.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of the internal value.
        /// </summary>
        /// <param name="format">The format specification.</param>
        /// <param name="provider">The <see cref="IFormatProvider"/> that determines how values are formatted.</param>
        /// <returns>A string representation of the internal value.</returns>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            if (this.Value.HasValue)
            {
                var value = this.Value.Value;

                if (string.IsNullOrEmpty(format) || "G" == format.ToUpperInvariant())
                {
                    // Return the numeric value as the default.
                    return value.ToString(provider);
                }

                // Enum only supports a lone X but caller may expect numeric formatting.
                if (0 <= format.IndexOf("X", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Return the hexadecimal value with the proper prefix.
                    return "0x" + Enum.Format(this.Type, value, "X");
                }

                // Use any other format specifier as is.
                return Enum.Format(this.Type, value, format);
            }

            return null;
        }

        /// <summary>
        /// Gets all the names of enumeration.
        /// </summary>
        /// <returns>All the names of the enumeration.</returns>
        internal IEnumerable<string> GetNames()
        {
            return Enum.GetNames(this.Type);
        }

        /// <summary>
        /// Determines if the enumeration for the <paramref name="name"/> is set.
        /// </summary>
        /// <param name="name">The name of the enumeration to check.</param>
        /// <returns>True if the enumeration for the <paramref name="name"/> is set.</returns>
        internal bool HasValue(string name)
        {
            if (Enum.IsDefined(this.Type, name))
            {
                var e = Enum.Parse(this.Type, name, true);
                var value = Convert.ToInt32(e);

                if (this.Value == value)
                {
                    return true;
                }

                return 0 != (this.Value & value);
            }

            return false;
        }

        #region Operators
        // A 32-bit integer is the largest integer supported by Windows Installer.

        /// <summary>
        /// Converts an <see cref="AttributeColumn"/> to an <see cref="Int32"/>.
        /// </summary>
        /// <param name="column">The <see cref="AttributeColumn"/> to convert.</param>
        /// <returns>The <see cref="Int32"/> value of the <see cref="AttributeColumn"/>.</returns>
        public static implicit operator int(AttributeColumn column)
        {
            if (null != column)
            {
                return Convert.ToInt32(column.Value, CultureInfo.InvariantCulture);
            }

            return 0;
        }

        /// <summary>
        /// Converts an <see cref="Int32"/> to an <see cref="AttributeColumn"/>.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value to convert.</param>
        /// <returns>The <see cref="AttributeColumn"/> with the value of the <see cref="Int32"/>.</returns>
        public static implicit operator AttributeColumn(int value)
        {
            return new AttributeColumn(null, value);
        }

        /// <summary>
        /// Converts an <see cref="AttributeColumn"/> to a nullable <see cref="Int32"/>.
        /// </summary>
        /// <param name="column">The <see cref="AttributeColumn"/> to convert.</param>
        /// <returns>The nullable <see cref="Int32"/> value of the <see cref="AttributeColumn"/>.</returns>
        public static implicit operator int?(AttributeColumn column)
        {
            if (null != column)
            {
                return column.Value;
            }

            return null;
        }

        /// <summary>
        /// Converts a nullable <see cref="Int32"/> to an <see cref="AttributeColumn"/>.
        /// </summary>
        /// <param name="value">The nullable <see cref="Int32"/> value to convert.</param>
        /// <returns>The <see cref="AttributeColumn"/> with the value of the nullable <see cref="Int32"/>.</returns>
        public static implicit operator AttributeColumn(int? value)
        {
            return new AttributeColumn(null, value);
        }
        #endregion

        #region IConvertible
        TypeCode IConvertible.GetTypeCode()
        {
            return Convert.GetTypeCode(this.Value);
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.Value, provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.Value, provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.Value, provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.Value, provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.Value, provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.Value, provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.Value, provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.Value, provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.Value, provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.Value, provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.Value, provider);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Convert.ToString(this.Value, provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, conversionType, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.Value, provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.Value, provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.Value, provider);
        }
        #endregion
    }
}
