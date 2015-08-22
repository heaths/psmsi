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

using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Provides conversion between the <see cref="LoggingPolicies"/> enumeration and string short form.
    /// </summary>
    internal sealed class LoggingPoliciesConverter : TypeConverter
    {
        /// <summary>
        /// Logging mode equivalent to the command line options "oicewarmup", or "*".
        /// </summary>
        internal const LoggingPolicies Normal = LoggingPolicies.OutOfDiskSpace | LoggingPolicies.Information | LoggingPolicies.CommonData | LoggingPolicies.Error |
                                            LoggingPolicies.Warning | LoggingPolicies.ActionStart | LoggingPolicies.ActionData | LoggingPolicies.FatalExit | LoggingPolicies.User | LoggingPolicies.PropertyDump;

        private static readonly Dictionary<char, LoggingPolicies> CharToModeMap;
        private static readonly Dictionary<LoggingPolicies, char> ModeToCharMap;

        static LoggingPoliciesConverter()
        {
            CharToModeMap = new Dictionary<char, LoggingPolicies>(CharComparer.InvariantCultureIgnoreCase);
            CharToModeMap.Add('v', LoggingPolicies.Verbose);
            CharToModeMap.Add('o', LoggingPolicies.OutOfDiskSpace);
            CharToModeMap.Add('i', LoggingPolicies.Information);
            CharToModeMap.Add('c', LoggingPolicies.CommonData);
            CharToModeMap.Add('e', LoggingPolicies.Error);
            CharToModeMap.Add('w', LoggingPolicies.Warning);
            CharToModeMap.Add('a', LoggingPolicies.ActionStart);
            CharToModeMap.Add('r', LoggingPolicies.ActionData);
            CharToModeMap.Add('m', LoggingPolicies.FatalExit);
            CharToModeMap.Add('u', LoggingPolicies.User);
            CharToModeMap.Add('p', LoggingPolicies.PropertyDump);
            CharToModeMap.Add('x', LoggingPolicies.ExtraDebug);
            CharToModeMap.Add('!', LoggingPolicies.FlushEachLine);

            // Reverse the key/value pairs.
            ModeToCharMap = new Dictionary<LoggingPolicies, char>();
            foreach (var entry in CharToModeMap)
            {
                ModeToCharMap.Add(entry.Value, entry.Key);
            }

            // Special case for asterisk.
            CharToModeMap.Add('*', Normal);
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
        /// Converts a <see cref="String"/> in the short form like "omus" to a <see cref="LoggingPolicies"/> enumeration.
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <returns>The converted <see cref="LoggingPolicies"/> enumeration.</returns>
        /// <exception cref="ArgumentException">The short form string contains invalid characters.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null != value && this.CanConvertFrom(context, value.GetType()))
            {
                var s = value as string;
                var mode = LoggingPolicies.None;

                // Attempt the simple coversion.
                if (TryParse(s, out mode))
                {
                    return mode;
                }
                else
                {
                    // Try parsing the logging command line options.
                    foreach (var c in s)
                    {
                        if (CharToModeMap.ContainsKey(c))
                        {
                            mode |= CharToModeMap[c];
                        }
                        else if ('+' == c)
                        {
                            throw new ArgumentException(Resources.Error_UnsupportedLoggingMode, "value");
                        }
                        else
                        {
                            var message = String.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidLoggingMode, c);
                            throw new ArgumentException(message, "value");
                        }
                    }

                    return mode;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a <see cref="LoggingPolicies"/> to a short form <see cref="String"/> like "voicewarmup".
        /// </summary>
        /// <param name="context">Additional context for conversion.</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <param name="value">The <see cref="LoggingPolicies"/> to convert.</param>
        /// <param name="destinationType">The type of the destination object.</param>
        /// <returns>The converted short form for a <see cref="LoggingPolicies"/> enumeration.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (null != value && this.CanConvertTo(context, destinationType))
            {
                var s = string.Empty;
                var mode = (LoggingPolicies)value;

                // Return the logging modes command line form.
                foreach (LoggingPolicies val in Enum.GetValues(typeof(LoggingPolicies)))
                {
                    if (0 != (val & mode) && LoggingPolicies.All != val)
                    {
                        s += ModeToCharMap[val];
                    }
                }

                return s;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static bool TryParse(string value, out LoggingPolicies mode)
        {
            try
            {
                mode = (LoggingPolicies)Enum.Parse(typeof(LoggingPolicies), value, true);
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
