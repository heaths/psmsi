// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="ExtensionMethods"/> class.
    /// </summary>
    [TestClass]
    public sealed class ExtensionMethodsTests
    {
        #region FirstOrDefault
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FirstOrDefaultThrows()
        {
            IEnumerable<string> e = null;
            var item = e.FirstOrDefault();
        }

        [TestMethod]
        public void FirstFromList()
        {
            var list = new List<string>() { "A", "B" };
            var item = list.FirstOrDefault();

            Assert.AreEqual("A", item, "The first item is incorrect.");
        }

        [TestMethod]
        public void FirstFromEnumerable()
        {
            var set = new HashSet<string> { "A", "B" };
            var item = set.FirstOrDefault();

            Assert.AreEqual("A", item, "The first item is incorrect.");
        }

        [TestMethod]
        public void DefaultFromEmptyList()
        {
            var list = new List<string>();
            var item = list.FirstOrDefault();

            Assert.IsNull(item, "The default item is incorrect.");
        }
        #endregion
    }
}
