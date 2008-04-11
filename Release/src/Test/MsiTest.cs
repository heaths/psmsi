// Unit test class the Msi class.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer
{
    /// <summary>
    /// Unit and functional tests for the <see cref="Msi"/> support class.
    /// </summary>
    [TestClass]
    public class MsiTest
    {
        static readonly FieldInfo majorField = typeof(Msi).GetField("major", BindingFlags.NonPublic | BindingFlags.Static);
        static readonly FieldInfo minorField = typeof(Msi).GetField("minor", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Gets or sets the major version field cache.
        /// </summary>
        /// <remarks>
        /// Set to 0 to get psmsi to recheck from msi.dll.
        /// </remarks>
        internal static int MajorOverride
        {
            get { return (int)majorField.GetValue(null); }
            set { majorField.SetValue(null, value); }
        }

        /// <summary>
        /// Gets or sets the minor version field cache.
        /// </summary>
        internal static int MinorOverride
        {
            get { return (int)minorField.GetValue(null); }
            set { minorField.SetValue(null, value); }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Reset the major version field cache to 0 to force a re-get.
            MajorOverride = 0;
        }

        /// <summary>
        /// A test for <see cref="Msi.CheckVersion"/>.
        /// </summary>
        [TestMethod]
        [Description("Tests version checking by overriding the version cache fields in the Msi class")]
        public void CheckVersionTest()
        {
            // Simulate MSI 4.0.
            MajorOverride = 4;

            try
            {
                // Check for MSI 3.1.
                if (!Msi.CheckVersion(3, 1))
                {
                    Assert.Fail("The version check for MSI 3.1 failed when MSI 4.0 is installed (simulated).");
                }
            }
            catch (NotSupportedException)
            {
                Assert.Fail("An exception should not have been thrown for a version check by default.");
            }

            try
            {
                // Check for MSI 4.5.
                if (Msi.CheckVersion(4, 5, true))
                {
                    Assert.Fail("The version check for MSI 4.5 did not fail when MSI 4.0 is installed (simulated).");
                }

                Assert.Fail("An exception should have been thrown for a version check when requested.");
            }
            catch (NotSupportedException)
            {
                // This exception should have been thrown.
            }
        }
    }
}
