// Mocks a registry hive from a file with registry data.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.Tools.WindowsInstaller
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
        private static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(unchecked((int)0x80000001));
        private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(unchecked((int)0x80000002));
        private static readonly IntPtr HKEY_USERS = new IntPtr(unchecked((int)0x80000003));

        private static volatile object syncRoot = new object();
        private static int count = 0;

        private string baseKeyPath;
        private RegistryKey currentUser;
        private RegistryKey localMachine;
        private RegistryKey users;

        [DllImport("advapi32.dll")]
        private static extern int RegOverridePredefKey(
            IntPtr hKey,
            IntPtr hNewHKey);

        private SafeHandle GetRegistryHandle(RegistryKey key)
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
            // Treat this disposable object as a singleton.
            if (0 == count)
            {
                lock (syncRoot)
                {
                    if (0 == count)
                    {
                        // Redirect registry access to a user location.
                        AssemblyName name = this.GetType().Assembly.GetName();

                        this.baseKeyPath = string.Format(@"Software\{0}\{1:B}", name.Name, Guid.NewGuid());

                        this.currentUser = Registry.CurrentUser.CreateSubKey(baseKeyPath + @"\HKCU");
                        this.localMachine = Registry.CurrentUser.CreateSubKey(baseKeyPath + @"\HKLM");
                        this.users = Registry.CurrentUser.CreateSubKey(baseKeyPath + @"\HKU");

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
            this.Import(path, null);
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
            // Decrement the ref count and dispose if needed.
            lock (syncRoot)
            {
                if (0 == --count)
                {
                    // Restore the redirected keys to their original hives.
                    RegOverridePredefKey(HKEY_CURRENT_USER, IntPtr.Zero);
                    RegOverridePredefKey(HKEY_LOCAL_MACHINE, IntPtr.Zero);
                    RegOverridePredefKey(HKEY_USERS, IntPtr.Zero);

                    if (disposing)
                    {
                        // Close the redirected keys and delete the parent.
                        this.currentUser.Dispose();
                        this.localMachine.Dispose();
                        this.users.Dispose();

                        Registry.CurrentUser.DeleteSubKeyTree(this.baseKeyPath);
                    }
                }
            }
        }
    }
}
