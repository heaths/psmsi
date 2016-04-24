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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Imports and exports a registry key tree from or to XML.
    /// </summary>
    internal sealed class RegistryXml
    {
        private static readonly Regex Variables = new Regex(@"\$\((?<var>\w+)\)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        private RegistryKey key;
        private Stack<RegistryKey> keys;

        /// <summary>
        /// Creates a new instance of the <see cref="RegistryKey"/> class.
        /// </summary>
        /// <remarks>
        /// Without specifying a root key, imported files must all begin with hives.
        /// </remarks>
        internal RegistryXml() : this(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RegistryKey"/> class.
        /// </summary>
        /// <param name="key">The <see cref="RegistryKey"/> to import or export.</param>
        /// <remarks>
        /// The given <paramref name="key"/> is the root for keys not nested under a hive element in imported files.
        /// </remarks>
        internal RegistryXml(RegistryKey key)
        {
            this.key = key;
            this.keys = new Stack<RegistryKey>();

            if (null != key)
            {
                this.keys.Push(key);
            }

            this.Properties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the propety dictionary.
        /// </summary>
        internal Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Import a registry key tree from the given <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> that contains the keys and values to import.</param>
        /// <exception cref="NotSupportedException">A hive name was specified that is not supported, or a value type was not supported.</exception>
        /// <exception cref="XmlException">A general XML exception occured.</exception>
        internal void Import(XmlReader reader)
        {
            Debug.Assert(null != reader, $@"The argument ""{nameof(reader)}"" is null.");

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            // Get the name attribute if provided.
                            string name = reader.GetAttribute("name");
                            RegistryKey key;

                            switch (reader.LocalName)
                            {
                                case "hive":
                                    // If empty, do nothing (no keys to create).
                                    if (reader.IsEmptyElement) { break; }

                                    // Add the specified hive to the stack.
                                    switch (name)
                                    {
                                        case "HKEY_CLASSES_ROOT":
                                            keys.Push(Registry.ClassesRoot);
                                            break;

                                        case "HKEY_CURRENT_USER":
                                            keys.Push(Registry.CurrentUser);
                                            break;

                                        case "HKEY_LOCAL_MACHINE":
                                            keys.Push(Registry.LocalMachine);
                                            break;

                                        case "HKEY_USERS":
                                            keys.Push(Registry.Users);
                                            break;

                                        default:
                                            throw new NotSupportedException(string.Format(@"Hive ""{0}"" is not supported.", name));
                                    }
                                    break;

                                case "key":
                                    // Create a new key as a subkey of the key currently at the top of the stack.
                                    try
                                    {
                                        key = keys.Peek();
                                    }
                                    catch (InvalidOperationException ex)
                                    {
                                        throw new NotSupportedException(string.Format(@"The key ""{0}"" requires that a root key was specified in the constructor.", name), ex);
                                    }

                                    name = ReplaceVariables(name);
                                    RegistryKey subkey = key.CreateSubKey(name);
                                    if (reader.IsEmptyElement)
                                    {
                                        // Just a key, so dispose it.
                                        subkey.Close();
                                    }
                                    else
                                    {
                                        // If children exist, push this key into the key stack.
                                        keys.Push(subkey);
                                    }
                                    break;

                                case "value":
                                    // Get the type of the registry value to create.
                                    string type = reader.GetAttribute("type");
                                    object value;
                                    RegistryValueKind kind = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), type);

                                    // Replace variables first.
                                    value = ReplaceVariables(reader.ReadString());

                                    switch (kind)
                                    {
                                        case RegistryValueKind.Binary:
                                            value = Convert.FromBase64String((string)value);
                                            break;

                                        case RegistryValueKind.DWord:
                                            if (reader.IsEmptyElement)
                                            {
                                                value = (int)0;
                                            }
                                            else
                                            {
                                                value = Convert.ToInt32((string)value);
                                            }
                                            break;

                                        case RegistryValueKind.ExpandString:
                                        case RegistryValueKind.String:
                                            break;

                                        case RegistryValueKind.MultiString:
                                            value = ((string)value).Split('\n');
                                            break;

                                        case RegistryValueKind.QWord:
                                            if (reader.IsEmptyElement)
                                            {
                                                value = (long)0;
                                            }
                                            else
                                            {
                                                value = Convert.ToInt64((string)value);
                                            }
                                            break;

                                        default:
                                            throw new NotSupportedException(string.Format(@"The registry type ""{0}"" is not supported.", type));
                                    }

                                    key = keys.Peek();
                                    key.SetValue(name, value, kind);
                                    break;
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        {
                            switch (reader.LocalName)
                            {
                                case "hive":
                                case "key":
                                    // Pop the last key from the stack and close it.
                                    RegistryKey key = keys.Pop();
                                    key.Close();
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private string ReplaceVariables(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // XmlReader.ReadString() returns an empty string if the element is empty.
                return string.Empty;
            }

            string replacement = Variables.Replace(value, m =>
            {
                string var = m.Groups["var"].Value;
                if (this.Properties.ContainsKey(var))
                {
                    return this.Properties[var];
                }

                return string.Format("$({0})", var);
            });

            return replacement;
        }
    }
}
