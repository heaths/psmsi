// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Provides conversion between strings and <see cref="Encoding"/>.
    /// </summary>
    internal sealed class EncodingConverter : TypeConverter
    {
        /// <summary>
        /// Returns True when <paramref name="sourceType"/> is a <see cref="String"/> or <see cref="Int32"/>.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="sourceType">The type of the source object.</param>
        /// <returns>True if the <paramref name="sourceType"/> is a <see cref="String"/> or <see cref="Int32"/>.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(string) == sourceType || IsNumeric(sourceType) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns True when <paramref name="destinationType"/> is a <see cref="String"/> or <see cref="Int32"/>.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="destinationType">The type of the destination object.</param>
        /// <returns>True if the <paramref name="destinationType"/> is a <see cref="String"/> or <see cref="Int32"/>.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(string) == destinationType || IsNumeric(destinationType) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts a <see cref="String"/> or <see cref="Int32"/> to an <see cref="Encoding"/>.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <param name="value">The <see cref="String"/> or <see cref="Int32"/> value to convert.</param>
        /// <returns>The <see cref="Encoding"/> for the string.</returns>
        /// <exception cref="ArgumentException">No <see cref="Encoding"/> could be matched to the string or code page integer.</exception>
        /// <remarks>
        /// Returns <see cref="Encoding.Default"/> if the <paramref name="value"/> is null or an empty string.
        /// </remarks>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value)
            {
                return Encoding.Default;
            }
            else if (this.CanConvertFrom(context, value.GetType()))
            {
                var type = value.GetType();
                if (typeof(string) == type)
                {
                    string name = (string)value;
                    var property = typeof(Encoding).GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
                    if (null != property)
                    {
                        return property.GetValue(null, null) as Encoding;
                    }
                    else
                    {
                        return Encoding.GetEncoding(name);
                    }
                }
                else if (IsNumeric(type))
                {
                    return Encoding.GetEncoding((int)value);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Gets the name or code page for an <see cref="Encoding"/>.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <param name="value">The <see cref="ReinstallModes"/> to convert.</param>
        /// <param name="destinationType">The type of the destination object.</param>
        /// <returns>The name or code page for an <see cref="Encoding"/>.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var enc = value as Encoding;
            if (null != enc && this.CanConvertTo(context, destinationType))
            {
                if (typeof(string) == destinationType)
                {
                    return enc.WebName;
                }
                else if (IsNumeric(destinationType))
                {
                    return Convert.ChangeType(enc.CodePage, destinationType);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns true if the <paramref name="type"/> is a signed or unsigned short or larger integer.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>True if the <paramref name="type"/> is a signed or unsigned short or larger integer.</returns>
        internal static bool IsNumeric(Type type)
        {
            return typeof(short) == type
                || typeof(int) == type
                || typeof(long) == type
                || typeof(ushort) == type
                || typeof(uint) == type
                || typeof(ulong) == type;
        }
    }
}
