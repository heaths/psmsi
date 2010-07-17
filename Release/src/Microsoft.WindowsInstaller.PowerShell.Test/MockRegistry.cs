// Mocks a registry hive from a file with registry data.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using Microsoft.Win32;

namespace Microsoft.WindowsInstaller
{
    /// <summary>
    /// Overrides registry hives and allows data to be imported into those hives for the current process.
    /// </summary>
    /// <remarks>
    /// The following registry hives are supported.
    /// <list type="list">
    /// <item>
    /// <term>HKEY_CURRENT_USER</term>
    /// </item>
    /// <item>
    /// <term>HKEY_LOCAL_MACHINE</term>
    /// </item>
    /// <item>
    /// <term>HKEY_USERS</term>
    /// </item>
    /// </list>
    /// Only a single instance of this class may be active at a time. Overriding registry hives is a per-process
    /// operation so modifying the registry key overrides may corrupt other threads.
    /// </remarks>
    internal sealed class MockRegistry : IDisposable
    {
        static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(unchecked((int)0x80000001));
        static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(unchecked((int)0x80000002));
        static readonly IntPtr HKEY_USERS = new IntPtr(unchecked((int)0x80000003));

        static object syncRoot = new object();
        static int count = 0;

        string baseKeyPath;
        RegistryKey currentUser;
        RegistryKey localMachine;
        RegistryKey users;

        [DllImport("advapi32.dll")]
        internal static extern int RegOverridePredefKey(
            IntPtr hKey,
            IntPtr hNewHKey);

        static SafeHandle GetRegistryHandle(RegistryKey key)
        {
            FieldInfo hkey = key.GetType().GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic);
            if (null != hkey)
            {
                return (SafeHandle)hkey.GetValue(key);
            }
            else
            {
                throw new InvalidOperationException("Field 'hkey' was not found in the RegistryKey class.");
            }
        }

        /// <summary>
        /// Creates a new instance of this class and redirects supported registry hives.
        /// </summary>
        /// <exception cref="InvalidOperationException">Another instance of this class is already active.</exception>
        internal MockRegistry()
        {
            lock (syncRoot)
            {
                if (0 == count)
                {
                    // Redirect registry access to a user location.
                    Assembly asm = GetType().Assembly;
                    AssemblyName name = asm.GetName();

                    baseKeyPath = string.Format(@"Software\{0}\{1:B}", name.Name, Guid.NewGuid());

                    currentUser = Registry.CurrentUser.CreateSubKey(baseKeyPath + @"\HKCU");
                    localMachine = Registry.CurrentUser.CreateSubKey(baseKeyPath + @"\HKLM");
                    users = Registry.CurrentUser.CreateSubKey(baseKeyPath + @"\HKU");

                    int ret;
                    SafeHandle handle;

                    handle = GetRegistryHandle(currentUser);
                    ret = RegOverridePredefKey(HKEY_CURRENT_USER, handle.DangerousGetHandle());
                    if (0 != ret) { throw new Win32Exception(ret); }

                    handle = GetRegistryHandle(localMachine);
                    ret = RegOverridePredefKey(HKEY_LOCAL_MACHINE, handle.DangerousGetHandle());
                    if (0 != ret) { throw new Win32Exception(ret); }

                    handle = GetRegistryHandle(users);
                    ret = RegOverridePredefKey(HKEY_USERS, handle.DangerousGetHandle());
                    if (0 != ret) { throw new Win32Exception(ret); }

                    // Finally increment the count if we got this far.
                    count++;
                }
                else
                {
                    // Throw from the constructor to disposing.
                    GC.SuppressFinalize(this);
                    throw new InvalidOperationException();
                }
            }
        }

        ~MockRegistry()
        {
            Dispose(false);
        }

        /// <summary>
        /// Imports a registry XML file.
        /// </summary>
        /// <param name="path">Path to the registry XML file to import.</param>
        internal void Import(string path)
        {
            Import(path, null);
        }

        /// <summary>
        /// Imports a registry XML file.
        /// </summary>
        /// <param name="path">Path to the registry XML file to import.</param>
        /// <param name="properties">Optional properties to replace during import.</param>
        internal void Import(string path, Dictionary<string, string> properties)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.DtdProcessing = DtdProcessing.Prohibit;

            string absolutePath = System.IO.Path.Combine(Environment.CurrentDirectory, path);
            using (XmlReader reader = XmlReader.Create(absolutePath, settings))
            {
                RegistryXml importer = new RegistryXml();
                if (null != properties)
                {
                    // Overwrite or add whatever is there by default.
                    foreach (KeyValuePair<string, string> pair in properties)
                    {
                        importer.Properties[pair.Key] = pair.Value;
                    }
                }

                importer.Import(reader);
            }
        }

        /// <summary>
        /// Restores the redirected registry hives and removes the imported values.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Restore hives, close the redirected keys, and delete them.
                using (currentUser)
                {
                    RegOverridePredefKey(HKEY_CURRENT_USER, IntPtr.Zero);
                }

                using (localMachine)
                {
                    RegOverridePredefKey(HKEY_LOCAL_MACHINE, IntPtr.Zero);
                }

                using (users)
                {
                    RegOverridePredefKey(HKEY_USERS, IntPtr.Zero);
                }

                Registry.CurrentUser.DeleteSubKeyTree(baseKeyPath);
            }

            count--;
        }
    }
}
