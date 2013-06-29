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

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Tests for the <see cref="ExtensionMethods"/> class.
    /// </summary>
    [TestClass]
    public sealed class ExtensionMethodsTests : TestBase
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
            paths[@"00:\SOFTWARE\"] = @"Microsoft.PowerShell.Core\Registry::HKEY_CLASSES_ROOT\SOFTWARE";
            paths[@"00:\SOFTWARE\Value"] = @"Microsoft.PowerShell.Core\Registry::HKEY_CLASSES_ROOT\SOFTWARE";
            paths[@"01:\SOFTWARE\"] = @"Microsoft.PowerShell.Core\Registry::HKEY_CURRENT_USER\SOFTWARE";
            paths[@"02:\SOFTWARE\"] = @"Microsoft.PowerShell.Core\Registry::HKEY_LOCAL_MACHINE\SOFTWARE";
            paths[@"03:\SOFTWARE\"] = @"Microsoft.PowerShell.Core\Registry::HKEY_USERS\SOFTWARE";

            foreach (var item in paths)
            {
                Assert.AreEqual(item.Value, TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(item.Key));
            }

            // A null key path is also valid.
            Assert.IsNull(TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(null));
        }

        [TestMethod]
        public void ConvertInvalidRegistryKeyPathToPSPath()
        {
            Assert.IsNull(TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(@"04:\SOFTWARE"));
        }

        [TestMethod]
        public void ConvertUnsupportedKeyPathToPSPath()
        {
            Assert.IsNull(TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromKeyPath(@"FOO:\BAR"));
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
            Assert.IsNull(TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromProviderPath(null));
        }

        [TestMethod]
        public void ConvertQualifiedProviderPathToPSPath()
        {
            const string path = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
            Assert.AreEqual(path, TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromProviderPath(path));
        }

        [TestMethod]
        public void ConvertProviderPathToPSPath()
        {
            const string path = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
            Assert.AreEqual(path, TestRunspace.SessionStateProxy.Path.GetUnresolvedPSPathFromProviderPath(@"C:\foo"));
        }
        #endregion

        #region GetPropertyValue
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyValueWithNullSource()
        {
            PSObject obj = null;
            var value = obj.GetPropertyValue<string>("A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyValueWithEmptyName()
        {
            var obj = PSObject.AsPSObject("TEST");
            var value = obj.GetPropertyValue<string>(string.Empty);
        }

        [TestMethod]
        public void GetPropertyValues()
        {
            var obj = PSObject.AsPSObject("TEST");
            obj.Properties.Add(new PSNoteProperty("A", "FOO"));
            obj.Properties.Add(new PSNoteProperty("B", "1"));

            var a = obj.GetPropertyValue<string>("A");
            Assert.AreEqual<string>("FOO", a, "The property value for A is incorrect.");

            var b = obj.GetPropertyValue<int>("B");
            Assert.AreEqual<int>(1, b, "The converted property value for B is incorrect.");

            var c = obj.GetPropertyValue<int>("C");
            Assert.AreEqual<int>(0, c, "The default property value for missing C is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(PSInvalidCastException))]
        public void GetPropertyValueWrongType()
        {
            var obj = PSObject.AsPSObject("TEST");
            obj.Properties.Add(new PSNoteProperty("A", "FOO"));

            // Should throw a PSInvalidCastException.
            var a = obj.GetPropertyValue<int>("A");
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
            var obj = PSObject.AsPSObject(this);
            obj.SetPropertyValue<string>(string.Empty, null);
        }

        [TestMethod]
        public void SetPropertyValues()
        {
            // Use concrete class since PSNoteProperty.Value allows type changes.
            var obj = PSObject.AsPSObject(this);

            obj.SetPropertyValue<string>("StringProperty", "FOO");
            var a = obj.GetPropertyValue<string>("StringProperty");
            Assert.AreEqual<string>("FOO", a, "The property value for A is incorrect.");

            obj.SetPropertyValue<string>("StringProperty", "BAR");
            a = obj.GetPropertyValue<string>("StringProperty");
            Assert.AreEqual<string>("BAR", a, "The property value for A is incorrect.");

            obj.SetPropertyValue<string>("IntegerProperty", "1");
            var b = obj.GetPropertyValue<int>("IntegerProperty");
            Assert.AreEqual<int>(1, b, "The converted property value for B is incorrect.");

            obj.SetPropertyValue<int>("MissingProperty", 1);
            var c = obj.GetPropertyValue<int>("MissingProperty");
            Assert.AreEqual<int>(1, c, "The property value for added C is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(PSInvalidCastException))]
        public void SetPropertyValueWrongType()
        {
            var obj = PSObject.AsPSObject(this);

            // Should throw a PSInvalidCastException.
            obj.SetPropertyValue<string>("IntegerProperty", "FOO");
        }
        #endregion
    }
}
