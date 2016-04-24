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
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Represents a view on the registry as imposed by the registry redirector.
    /// </summary>
    /// <remarks>
    /// This is a static respresentation based on the information at:
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa384253(v=vs.85).aspx.
    /// </remarks>
    internal sealed class RegistryView
    {
        // Improve performance by loading fields instead of allocating new strings each time.
        private static readonly string SOFTWARE = @"\SOFTWARE";
        private static readonly string WOW64 = @"\Wow6432Node";

        private static readonly string SOFTWARE_WOW64 = SOFTWARE + WOW64;

        private static readonly string HKCR = "HKEY_CLASSES_ROOT";
        private static readonly string HKCR_WOW64 = @"HKEY_CLASSES_ROOT" + WOW64;
        private static readonly string HKCU = "HKEY_CURRENT_USER";
        private static readonly string HKCU_WOW64 = @"HKEY_CURRENT_USER" + SOFTWARE_WOW64;
        private static readonly string HKLM = "HKEY_LOCAL_MACHINE";
        private static readonly string HKLM_WOW64 = @"HKEY_LOCAL_MACHINE" + SOFTWARE_WOW64;
        private static readonly string HKU = "HKEY_USERS";

        private static readonly Tree<bool> tree;

        // Singleton fields.
        private static volatile object syncRoot = new object();
        private static RegistryView instance = null;

        static RegistryView()
        {
            // The value is false if Shared or true if Redirected.
            tree = new Tree<bool>();

            // HKEY_CLASSES_ROOT
            tree.Add(@"00:\CLSID", true);
            tree.Add(@"00:\DirectShow", true);
            tree.Add(@"00:\Interface", true);
            tree.Add(@"00:\Media Type", true);
            tree.Add(@"00:\MediaFoundation", true);

            // HKEY_CURRENT_USER
            tree.Add(@"01:\SOFTWARE\Classes\CLSID", true);
            tree.Add(@"01:\SOFTWARE\Classes\DirectShow", true);
            tree.Add(@"01:\SOFTWARE\Classes\Interface", true);
            tree.Add(@"01:\SOFTWARE\Classes\Media Type", true);
            tree.Add(@"01:\SOFTWARE\Classes\MediaFoundation", true);

            // HKEY_LOCAL_MACHINE
            tree.Add(@"02:\SOFTWARE", true);
            tree.Add(@"02:\SOFTWARE\Classes", false);
            tree.Add(@"02:\SOFTWARE\Classes\CLSID", true);
            tree.Add(@"02:\SOFTWARE\Classes\DirectShow", true);
            tree.Add(@"02:\SOFTWARE\Classes\Interface", true);
            tree.Add(@"02:\SOFTWARE\Classes\Media Type", true);
            tree.Add(@"02:\SOFTWARE\Classes\MediaFoundation", true);
            tree.Add(@"02:\SOFTWARE\Clients", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\COM3", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Cryptography\Calais\Current", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Cryptography\Calais\Readers", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Cryptography\Services", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\CTF\SystemShared", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\CTF\TIP", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\DFS", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Driver Signing", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\EnterpriseCertificates", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\EventSystem", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\MSMQ", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Non-Driver Signing", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Notepad\DefaultFonts", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\OLE", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\RAS", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\RPC", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\SOFTWARE\Microsoft\Shared Tools\MSInfo", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\SystemCertificates", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\TermServLicensing", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\TransactionServer", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Control Panel\Cursors\Schemes", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\DriveIcons", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\KindMap", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Group Policy", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\PreviewHandlers", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Setup", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows\CurrentVersion\Telephony\Locations", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Console", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontDpi", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontLink", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontMapper", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontSubstitutes", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Gre_Initialize", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Language Pack", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkCards", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Ports", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Print", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList", false);
            tree.Add(@"02:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones", false);
            tree.Add(@"02:\SOFTWARE\Policies", false);
            tree.Add(@"02:\SOFTWARE\RegisteredApplications", false);

            // HKEY_USERS
            tree.Add(@"03:\*\SOFTWARE\Classes\CLSID", true);
            tree.Add(@"03:\*\SOFTWARE\Classes\DirectShow", true);
            tree.Add(@"03:\*\SOFTWARE\Classes\Interface", true);
            tree.Add(@"03:\*\SOFTWARE\Classes\Media Type", true);
            tree.Add(@"03:\*\SOFTWARE\Classes\MediaFoundation", true);
        }

        /// <summary>
        /// Gets the single instance for the current process bitness.
        /// </summary>
        /// <returns>The single instance for the current process bitness.</returns>
        internal static RegistryView GetInstance()
        {
            if (null == instance)
            {
                lock (syncRoot)
                {
                    if (null == instance)
                    {
                        instance = new RegistryView(8 == IntPtr.Size);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryView"/> class.
        /// </summary>
        /// <param name="is64Bit">True if running under a 64-bit process; otherwise, false for a 32-bit process.</param>
        internal RegistryView(bool is64Bit)
        {
            this.Is64Bit = is64Bit;
        }

        /// <summary>
        /// Gets a value indicating whether the code is executing in a 64-bit process.
        /// </summary>
        internal bool Is64Bit { get; private set; }

        /// <summary>
        /// Maps the Windows Installer key path to an appropriate registry path given the current process's bitness.
        /// </summary>
        /// <param name="keyPath">The Windows Installer key path to map.</param>
        /// <returns>The registry path that is mapped to the key path, or null if the key path is a 64-bit key path and the current process is 32-bit.</returns>
        internal string MapKeyPath(string keyPath)
        {
            // Callers should not have to handle exceptions for control flow.
            if (string.IsNullOrEmpty(keyPath) || 4 > keyPath.Length)
            {
                return null;
            }

            var root = keyPath.Substring(0, 2);
            var path = keyPath.Substring(3);
            var is64Bit = '2' == root[0];
            var hive = root[1];

            if (!this.Is64Bit)
            {
                // Simply return 32-bit paths as-is and null for 64-bit paths.
                if (!is64Bit)
                {
                    root = this.MapRoot(path, false, hive);
                    if (null != root)
                    {
                        return root + path;
                    }
                }
            }
            else
            {
                // Return 64-bit paths as-is but map 32-bit paths to WOW64 node only if redirected.
                if (is64Bit || !tree.Under(keyPath))
                {
                    root = this.MapRoot(path, true, hive);
                }
                else
                {
                    root = this.MapRoot(path, false, hive);

                    if ('3' == hive)
                    {
                        // Strip user SID from registry key.
                        var i = path.IndexOf(Path.DirectorySeparatorChar, 1);
                        path = path.Substring(i);
                    }

                    // The redirected root will be replaced.
                    if (0 == path.IndexOf(SOFTWARE, StringComparison.OrdinalIgnoreCase))
                    {
                        path = path.Substring(SOFTWARE.Length);
                    }

                    // Make sure the path doesn't reference the WOW64 node twice.
                    // Windows Installer seems to handle this correctly during installation.
                    if (0 == path.IndexOf(WOW64, StringComparison.OrdinalIgnoreCase))
                    {
                        path = path.Substring(WOW64.Length);
                    }
                }

                if (null != root)
                {
                    return root + path;
                }
            }

            return null;
        }

        private string MapRoot(string path, bool is64bit, char hive)
        {
            // Check for impossible mapping.
            if (!this.Is64Bit && is64bit)
            {
                return null;
            }

            // HKEY_CLASSES_ROOT
            if ('0' == hive)
            {
                return this.Is64Bit == is64bit ? HKCR : HKCR_WOW64;
            }

            // HKEY_CURRENT_USER
            if ('1' == hive)
            {
                return this.Is64Bit == is64bit ? HKCU : HKCU_WOW64;
            }

            // HKEY_LOCAL_MACHINE
            if ('2' == hive)
            {
                return this.Is64Bit == is64bit ? HKLM : HKLM_WOW64;
            }

            // HKEY_USERS
            if ('3' == hive && !string.IsNullOrEmpty(path))
            {
                if (this.Is64Bit == is64bit)
                {
                    return HKU;
                }
                else
                {
                    // Extract the user SID with leading backslash.
                    var userSid = path.Substring(0, path.IndexOf(Path.DirectorySeparatorChar, 1));
                    return HKU + userSid + SOFTWARE_WOW64;
                }
            }

            return null;
        }
    }
}
