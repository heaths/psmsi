// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="Set&lt;T&gt;"/> class.
    /// </summary>
    [TestClass]
    public sealed class SetTests : TestBase
    {
        [TestMethod]
        public void DefaultSet()
        {
            var set = new Set<string>();
            set.Add("A");
            set.Add("B");
            set.Add("a");

            Assert.AreEqual<int>(3, set.Count, "The number of items in the set are not correct.");
        }

        [TestMethod]
        public void CaseInsensitiveSet()
        {
            var set = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            set.Add("A");
            set.Add("B");
            set.Add("a");

            Assert.AreEqual<int>(2, set.Count, "The number of items in the set are not correct.");
        }
    }
}
