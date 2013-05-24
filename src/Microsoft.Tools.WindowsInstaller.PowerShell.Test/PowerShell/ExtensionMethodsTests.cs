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
        public string StringProperty { get; set; }
        public int IntegerProperty { get; set; }

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

                // A null key path is also valid.
                Assert.IsNull(ps.Runspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(null));
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

        #region GetPropertyValue
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyValueWithNullSource()
        {
            PSObject obj = null;
            string value = obj.GetPropertyValue<string>("A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyValueWithEmptyName()
        {
            PSObject obj = PSObject.AsPSObject("TEST");
            string value = obj.GetPropertyValue<string>(string.Empty);
        }

        [TestMethod]
        public void GetPropertyValues()
        {
            PSObject obj = PSObject.AsPSObject("TEST");
            obj.Properties.Add(new PSNoteProperty("A", "FOO"));
            obj.Properties.Add(new PSNoteProperty("B", "1"));

            string a = obj.GetPropertyValue<string>("A");
            Assert.AreEqual<string>("FOO", a, "The property value for A is incorrect.");

            int b = obj.GetPropertyValue<int>("B");
            Assert.AreEqual<int>(1, b, "The converted property value for B is incorrect.");

            int c = obj.GetPropertyValue<int>("C");
            Assert.AreEqual<int>(0, c, "The default property value for missing C is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(PSInvalidCastException))]
        public void GetPropertyValueWrongType()
        {
            PSObject obj = PSObject.AsPSObject("TEST");
            obj.Properties.Add(new PSNoteProperty("A", "FOO"));

            // Should throw a PSInvalidCastException.
            int a = obj.GetPropertyValue<int>("A");
        }
        #endregion

        #region SetPropertyValue
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetPropertyValueWithNullSource()
        {
            PSObject obj = null;
            obj.SetPropertyValue<string>("A", "FOO");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetPropertyValueWithEmptyName()
        {
            PSObject obj = PSObject.AsPSObject(this);
            obj.SetPropertyValue<string>(string.Empty, null);
        }

        [TestMethod]
        public void SetPropertyValues()
        {
            // Use concrete class since PSNoteProperty.Value allows type changes.
            PSObject obj = PSObject.AsPSObject(this);

            obj.SetPropertyValue<string>("StringProperty", "FOO");
            string a = obj.GetPropertyValue<string>("StringProperty");
            Assert.AreEqual<string>("FOO", a, "The property value for A is incorrect.");

            obj.SetPropertyValue<string>("StringProperty", "BAR");
            a = obj.GetPropertyValue<string>("StringProperty");
            Assert.AreEqual<string>("BAR", a, "The property value for A is incorrect.");

            obj.SetPropertyValue<string>("IntegerProperty", "1");
            int b = obj.GetPropertyValue<int>("IntegerProperty");
            Assert.AreEqual<int>(1, b, "The converted property value for B is incorrect.");

            obj.SetPropertyValue<int>("MissingProperty", 1);
            int c = obj.GetPropertyValue<int>("MissingProperty");
            Assert.AreEqual<int>(1, c, "The property value for added C is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(PSInvalidCastException))]
        public void SetPropertyValueWrongType()
        {
            PSObject obj = PSObject.AsPSObject(this);

            // Should throw a PSInvalidCastException.
            obj.SetPropertyValue<string>("IntegerProperty", "FOO");
        }
        #endregion
    }
}
