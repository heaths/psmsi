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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="RegistryView"/> class.
    /// </summary>
    [TestClass]
    public sealed class RegistryViewTests
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            // Initialize to avoid showing perf hit to individual tests.
            RegistryView.GetInstance();
        }

        [TestMethod]
        public void SingletonMatchesArchitecture()
        {
            var view = RegistryView.GetInstance();
            Assert.AreEqual<bool>(8 == IntPtr.Size, view.Is64Bit);
        }

        [TestMethod]
        public void MapKeyPathsIn32BitProcess()
        {
            var view = new RegistryView(false);
            Assert.IsFalse(view.Is64Bit);

            // Invalid paths.
            Assert.IsNull(view.MapKeyPath(null));
            Assert.IsNull(view.MapKeyPath("TRE"));
            Assert.IsNull(view.MapKeyPath("TEST"));

            // 32-bit paths.
            Assert.AreEqual(@"HKEY_CLASSES_ROOT\CLSID\", view.MapKeyPath(@"00:\CLSID\"), true);
            Assert.AreEqual(@"HKEY_CURRENT_USER\Software\Classes\CLSID\", view.MapKeyPath(@"01:\Software\Classes\CLSID\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\TEST\", view.MapKeyPath(@"02:\Software\TEST\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Microsoft\", view.MapKeyPath(@"02:\Software\Microsoft\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Microsoft\EventSystem\", view.MapKeyPath(@"02:\Software\Microsoft\EventSystem\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\TEST\", view.MapKeyPath(@"02:\Software\Wow6432Node\TEST\"), true);
            Assert.AreEqual(@"HKEY_USERS\S-1-5-18\Software\Classes\CLSID\", view.MapKeyPath(@"03:\S-1-5-18\Software\Classes\CLSID\"), true);

            // 64-bit paths.
            Assert.IsNull(view.MapKeyPath(@"20:\CLSID\"));
            Assert.IsNull(view.MapKeyPath(@"21:\Software\Classes\CLSID\"));
            Assert.IsNull(view.MapKeyPath(@"22:\Software\TEST\"));
            Assert.IsNull(view.MapKeyPath(@"22:\Software\Microsoft\"));
            Assert.IsNull(view.MapKeyPath(@"22:\Software\Microsoft\EventSystem\"));
            Assert.IsNull(view.MapKeyPath(@"22:\Software\Wow6432Node\TEST\"));
            Assert.IsNull(view.MapKeyPath(@"23:\S-1-5-18\Software\Classes\CLSID\"));
        }

        [TestMethod]
        public void MapKeyPathsIn64BitProcess()
        {
            var view = new RegistryView(true);
            Assert.IsTrue(view.Is64Bit);

            // Invalid paths.
            Assert.IsNull(view.MapKeyPath(null));
            Assert.IsNull(view.MapKeyPath("TRE"));
            Assert.IsNull(view.MapKeyPath("TEST"));

            // 32-bit paths.
            Assert.AreEqual(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\", view.MapKeyPath(@"00:\CLSID\"), true);
            Assert.AreEqual(@"HKEY_CURRENT_USER\Software\Wow6432Node\Classes\CLSID\", view.MapKeyPath(@"01:\Software\Classes\CLSID\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\TEST\", view.MapKeyPath(@"02:\Software\TEST\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\", view.MapKeyPath(@"02:\Software\Microsoft\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Microsoft\EventSystem\", view.MapKeyPath(@"02:\Software\Microsoft\EventSystem\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\TEST\", view.MapKeyPath(@"02:\Software\Wow6432Node\TEST\"), true);
            Assert.AreEqual(@"HKEY_USERS\S-1-5-18\Software\Wow6432Node\Classes\CLSID\", view.MapKeyPath(@"03:\S-1-5-18\Software\Classes\CLSID\"), true);

            // 64-bit paths.
            Assert.AreEqual(@"HKEY_CLASSES_ROOT\CLSID\", view.MapKeyPath(@"20:\CLSID\"), true);
            Assert.AreEqual(@"HKEY_CURRENT_USER\Software\Classes\CLSID\", view.MapKeyPath(@"21:\Software\Classes\CLSID\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\TEST\", view.MapKeyPath(@"22:\Software\TEST\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Microsoft\", view.MapKeyPath(@"22:\Software\Microsoft\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Microsoft\EventSystem\", view.MapKeyPath(@"22:\Software\Microsoft\EventSystem\"), true);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\TEST\", view.MapKeyPath(@"22:\Software\Wow6432Node\TEST\"), true);
            Assert.AreEqual(@"HKEY_USERS\S-1-5-18\Software\Classes\CLSID\", view.MapKeyPath(@"23:\S-1-5-18\Software\Classes\CLSID\"), true);
        }
    }
}
