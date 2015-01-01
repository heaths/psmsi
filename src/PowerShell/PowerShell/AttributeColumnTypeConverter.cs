// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Converts an <see cref="Int32"/> (or nullable) to an <see cref="AttributeColumn"/>.
    /// </summary>
    public sealed class AttributeColumnTypeConverter : PSTypeConverter
    {
        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="sourceValue">The value to convert from.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>Always returns false.</returns>
        public override bool CanConvertFrom(object sourceValue, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Returns true if the <paramref name="destinationType"/> is an <see cref="Int32"/> (or nullable).
        /// </summary>
        /// <param name="sourceValue">Should always be an <see cref="AttributeColumn"/>.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>True if the <paramref name="destinationType"/> is an <see cref="Int32"/> (or nullable).</returns>
        public override bool CanConvertTo(object sourceValue, Type destinationType)
        {
            if (null != sourceValue && typeof(AttributeColumn) == sourceValue.GetType())
            {
                return typeof(int) == destinationType || typeof(int?) == destinationType;
            }

            return false;
        }

        /// <summary>
        /// Always returns null.
        /// </summary>
        /// <param name="sourceValue">The vaue to convert from.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> to use during conversion.</param>
        /// <param name="ignoreCase">Whether to ignore case during conversion.</param>
        /// <returns>Always throws a <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">Conversion to the <paramref name="sourceValue"/> type is not supported.</exception>
        public override object ConvertFrom(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an <see cref="Int32"/> (or null) from an <see cref="AttributeColumn"/>.
        /// </summary>
        /// <param name="sourceValue">The <see cref="AttributeColumn"/> to convert.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> to use during conversion.</param>
        /// <param name="ignoreCase">Whether to ignore case during conversion.</param>
        /// <returns>An <see cref="Int32"/> (or null) from an <see cref="AttributeColumn"/>.</returns>
        /// <exception cref="NotSupportedException">Conversion to the <paramref name="destinationType"/> is not supported.</exception>
        public override object ConvertTo(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
        {
            if (this.CanConvertTo(sourceValue, destinationType))
            {
                var value = (AttributeColumn)sourceValue;
                if (typeof(int?) == destinationType)
                {
                    return value.Value;
                }
                else
                {
                    // Returns 0 if the value is null.
                    return Convert.ToInt32(value.Value, formatProvider);
                }
            }

            throw new NotSupportedException();
        }
    }
}
