// Unit tests for the Utilities class.
//
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
    /// Tests the <see cref="Utilities"/> class.
    /// </summary>
    [TestClass]
    public sealed class UtilitiesTest
    {
        /// <summary>
        /// Tests the <see cref="Utility.MatchesAnyWildcardPattern"/> method using a null "value" argument.
        /// </summary>
        [TestMethod]
        [Description("A test for Utility.MatchesAnyWildcardPattern using a null \"value\" argument")]
        public void MatchesAnyWildcardPatternTest_NullValue()
        {
            // Test a null value to match.
            Assert.IsFalse(Utility.MatchesAnyWildcardPattern(null, null));
            Assert.IsFalse(Utility.MatchesAnyWildcardPattern(string.Empty, null));
        }

        /// <summary>
        /// Tests the <see cref="Utility.MatchesAnyWildcardPattern"/> method using a null "patterns" argument.
        /// </summary>
        [TestMethod]
        [Description("A test for Utility.MatchesAnyWildcardPattern using a null \"patterns\" argument")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchesAnyWildcardPatternTest_NullPatterns()
        {
            Utility.MatchesAnyWildcardPattern("test", null);
        }

        /// <summary>
        /// Tests the <see cref="Utility.MatchesAnyWildcardPattern"/> method using an empty "patterns" list.
        /// </summary>
        [TestMethod]
        [Description("A test for Utility.MatchesAnyWildcardPattern using an empty \"patterns\" list")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchesAnyWildcardPatternTest_EmptyPatterns()
        {
            Utility.MatchesAnyWildcardPattern("test", new WildcardPattern[] { });
        }

        /// <summary>
        /// Teste the <see cref="Utility.MatchesAnyWildcardPattern"/> method.
        /// </summary>
        [TestMethod]
        [Description("A test for Utility.MatchesAnyWildcardPattern")]
        public void MatchesAnyWildcardPatternTest()
        {
            List<WildcardPattern> patterns = new List<WildcardPattern>();
            patterns.Add(new WildcardPattern("Windows*"));
            patterns.Add(new WildcardPattern("*Installer"));

            Assert.IsTrue(Utility.MatchesAnyWildcardPattern("Windows Installer", patterns));
            Assert.IsFalse(Utility.MatchesAnyWildcardPattern("Microsoft Corporation", patterns));
        }
    }
}
