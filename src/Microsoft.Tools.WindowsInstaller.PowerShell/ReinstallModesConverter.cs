// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Provides conversion between the <see cref="ReinstallModes"/> enumeration and string short form.
    /// </summary>
    internal sealed class ReinstallModesConverter : TypeConverter
    {
        private static readonly Dictionary<char, ReinstallModes> CharToModeMap;
        private static readonly Dictionary<ReinstallModes, char> ModeToCharMap;

        static ReinstallModesConverter()
        {
            CharToModeMap = new Dictionary<char, ReinstallModes>();
            CharToModeMap.Add('p', ReinstallModes.FileMissing);
            CharToModeMap.Add('o', ReinstallModes.FileOlderVersion);
            CharToModeMap.Add('e', ReinstallModes.FileEqualVersion);
            CharToModeMap.Add('d', ReinstallModes.FileExact);
            CharToModeMap.Add('c', ReinstallModes.FileVerify);
            CharToModeMap.Add('a', ReinstallModes.FileReplace);
            CharToModeMap.Add('u', ReinstallModes.UserData);
            CharToModeMap.Add('m', ReinstallModes.MachineData);
            CharToModeMap.Add('s', ReinstallModes.Shortcut);
            CharToModeMap.Add('v', ReinstallModes.Package);

            // Reverse the key/value pairs.
            ModeToCharMap = new Dictionary<ReinstallModes, char>();
            foreach (var entry in CharToModeMap)
            {
                ModeToCharMap.Add(entry.Value, entry.Key);
            }
        }

        /// <summary>
        /// Returns True when <paramref name="sourceType"/> is a <see cref="String"/>.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="sourceType">The type of the source object.</param>
        /// <returns>True if the <paramref name="sourceType"/> is a <see cref="String"/>.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(string) == sourceType;
        }

        /// <summary>
        /// Returns True when <paramref name="destinationType"/> is a <see cref="String"/>.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="destinationType">The type of the destination object.</param>
        /// <returns>True if the <paramref name="destinationType"/> is a <see cref="String"/>.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(string) == destinationType;
        }

        /// <summary>
        /// Converts a <see cref="String"/> in the short form like "omus" to a <see cref="ReinstallModes"/> enumeration.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <returns>The converted <see cref="ReinstallModes"/> enumeration.</returns>
        /// <exception cref="ArgumentException">The short form string contains invalid characters.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null != value && this.CanConvertFrom(context, value.GetType()))
            {
                string s = value as string;
                ReinstallModes mode = 0;

                // Attempt the simple coversion.
                if (TryParse(s, out mode))
                {
                    return mode;
                }
                else
                {
                    // Try parsing the REINSTALLMODE property values.
                    foreach (char c in s)
                    {
                        if (CharToModeMap.ContainsKey(c))
                        {
                            mode |= CharToModeMap[c];
                        }
                        else
                        {
                            throw new ArgumentException(string.Format(Resources.Error_InvalidReinstallMode, c), "value");
                        }
                    }

                    return mode;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a <see cref="ReinstallModes"/> to a short form <see cref="String"/> like "omus".
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <param name="value">The <see cref="ReinstallModes"/> to convert.</param>
        /// <param name="destinationType">The type of the destination object.</param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (null != value && this.CanConvertTo(context, destinationType))
            {
                string s = string.Empty;
                ReinstallModes mode = (ReinstallModes)value;

                // Return the REINSTALLMODE property value form.
                foreach (ReinstallModes val in Enum.GetValues(typeof(ReinstallModes)))
                {
                    if (0 != (val & mode))
                    {
                        s += ModeToCharMap[val];
                    }
                }

                return s;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static bool TryParse(string value, out ReinstallModes mode)
        {
            try
            {
                mode = (ReinstallModes)Enum.Parse(typeof(ReinstallModes), value, true);
                return true;
            }
            catch
            {
                mode = 0;
                return false;
            }
        }
    }
}
