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
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Converts an <see cref="int"/> (or nullable) to an <see cref="AttributeColumn"/>.
    /// </summary>
    public sealed class AttributeColumnTypeConverter : PSTypeConverter
    {
        /// <summary>
        /// Determines if the <paramref name="sourceValue"/> can be converted to the <paramref name="destinationType"/>.
        /// </summary>
        /// <param name="sourceValue">The value to convert from.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>Always returns false.</returns>
        public override bool CanConvertFrom(object sourceValue, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Returns true if the <paramref name="destinationType"/> is an <see cref="int"/> (or nullable).
        /// </summary>
        /// <param name="sourceValue">Should always be an <see cref="AttributeColumn"/>.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>True if the <paramref name="destinationType"/> is an <see cref="int"/> (or nullable).</returns>
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
        /// Returns an <see cref="int"/> (or null) from an <see cref="AttributeColumn"/>.
        /// </summary>
        /// <param name="sourceValue">The <see cref="AttributeColumn"/> to convert.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> to use during conversion.</param>
        /// <param name="ignoreCase">Whether to ignore case during conversion.</param>
        /// <returns>An <see cref="int"/> (or null) from an <see cref="AttributeColumn"/>.</returns>
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
