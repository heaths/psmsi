// Imports and exports registry keys from or to XML.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Win32;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Imports and exports a registry key tree from or to XML.
    /// </summary>
    internal class RegistryXml
    {
        static readonly Regex Variables = new Regex(@"\$\((?<var>\w+)\)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        private RegistryKey key;
        private Stack<RegistryKey> keys;
        private Dictionary<string, string> properties;

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

            InitializeProperties();
        }

        /// <summary>
        /// Gets the propety dictionary.
        /// </summary>
        internal Dictionary<string, string> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Import a registry key tree from the given <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> that contains the keys and values to import.</param>
        /// <exception cref="NotSupportedException">A hive name was specified that is not supported, or a value type was not supported.</exception>
        /// <exception cref="XmlException">A general XML exception occured.</exception>
        internal void Import(XmlReader reader)
        {
            Debug.Assert(null != reader);

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

        void InitializeProperties()
        {
            properties = new Dictionary<string, string>();
            properties.Add("CurrentSID", TestProject.CurrentSID);
            properties.Add("CurrentUsername", TestProject.CurrentUsername);
        }

        string ReplaceVariables(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // XmlReader.ReadString() returns an empty string if the element is empty.
                return string.Empty;
            }

            string replacement = Variables.Replace(value, delegate(Match m)
            {
                string var = m.Groups["var"].Value;
                if (!properties.ContainsKey(var))
                {
                    throw new NotSupportedException(string.Format("The variable {0} is not supported.", var));
                }

                return properties[var];
            });
            return replacement;
        }
    }
}
