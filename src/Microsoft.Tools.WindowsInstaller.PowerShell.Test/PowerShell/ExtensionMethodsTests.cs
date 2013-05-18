// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using PS = System.Management.Automation.PowerShell;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Tests for the <see cref="ExtensionMethods"/> class.
    /// </summary>
    [TestClass]
    public sealed class ExtensionMethodsTests
    {
        #region Match
        [TestMethod]
        public void NullOrEmptyStringMatch()
        {
            string value = null;
            Assert.IsFalse(value.Match(null));
            Assert.IsFalse(string.Empty.Match(null));
        }

        [TestMethod]
        public void StringMatchNullPatterns()
        {
            Assert.IsFalse("test".Match(null));
        }

        [TestMethod]
        public void StringMatchEmptyPatterns()
        {
            Assert.IsFalse("test".Match(new WildcardPattern[] { }));
        }

        [TestMethod]
        public void StringMatchPatterns()
        {
            var patterns = new List<WildcardPattern>();
            patterns.Add(new WildcardPattern("Windows*"));
            patterns.Add(new WildcardPattern("*Installer"));

            Assert.IsTrue("Windows Installer".Match(patterns));
            Assert.IsFalse("Microsoft Corporation".Match(patterns));
        }
        #endregion

        #region GetUnresolvedPSPathFromKeyPath
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUnresolvedPSPathFromKeyPathThrows()
        {
            PathIntrinsics provider = null;
            provider.GetUnresolvedPSPathFromKeyPath(@"C:\foo");
        }

        [TestMethod]
        public void ConvertValidKeyPathToPSPath()
        {
            // Define all the possible translations.
            var paths = new Dictionary<string, string>();
            paths[@"C:\foo"] = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
            paths[@"C?\foo"] = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
            paths[@"\\server\share\directory"] = @"Microsoft.PowerShell.Core\FileSystem::\\server\share\directory";
            paths[@"00:\SOFTWARE"] = @"Microsoft.PowerShell.Core\Registry::HKEY_CLASSES_ROOT\SOFTWARE";
            paths[@"01:\SOFTWARE"] = @"Microsoft.PowerShell.Core\Registry::HKEY_CURRENT_USER\SOFTWARE";
            paths[@"02:\SOFTWARE"] = @"Microsoft.PowerShell.Core\Registry::HKEY_LOCAL_MACHINE\SOFTWARE";
            paths[@"03:\SOFTWARE"] = @"Microsoft.PowerShell.Core\Registry::HKEY_USERS\SOFTWARE";

            using (var ps = PS.Create())
            {
                foreach (var item in paths)
                {
                    Assert.AreEqual(item.Value, ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(item.Key));
                }
            }
        }

        [TestMethod]
        public void ConvertInvalidRegistryKeyPathToPSPath()
        {
            using (var ps = PS.Create())
            {
                Assert.IsNull(ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(@"04:\SOFTWARE"));
            }
        }

        [TestMethod]
        public void ConvertUnsupportedKeyPathToPSPath()
        {
            using (var ps = PS.Create())
            {
                Assert.IsNull(ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(@"FOO:\BAR"));
            }
        }
        #endregion

        #region GetUnresolvedPSPathFromProviderPath
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUnresolvedPSPathFromProviderPathThrows()
        {
            PathIntrinsics provider = null;
            provider.GetUnresolvedPSPathFromProviderPath(@"C:\foo");
        }

        [TestMethod]
        public void ConvertNullProviderPathToPSPath()
        {
            using (var ps = PS.Create())
            {
                Assert.IsNull(ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromProviderPath(null));
            }
        }

        [TestMethod]
        public void ConvertQualifiedProviderPathToPSPath()
        {
            const string path = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
            using (var ps = PS.Create())
            {
                Assert.AreEqual(path, ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromProviderPath(path));
            }
        }

        [TestMethod]
        public void ConvertProviderPathToPSPath()
        {
            const string path = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
            using (var ps = PS.Create())
            {
                Assert.AreEqual(path, ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromProviderPath(@"C:\foo"));
            }
        }
        #endregion
    }
}
