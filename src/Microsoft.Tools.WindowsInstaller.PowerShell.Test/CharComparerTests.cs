// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    [TestClass]
    public class CharComparerTests : TestBase
    {
        [TestMethod]
        public void CompareChars()
        {
            var comparer = CharComparer.InvariantCultureIgnoreCase;
            Assert.IsFalse(comparer.Equals('a', 'b'));
            Assert.IsTrue(comparer.Equals('a', 'a'));
            Assert.IsTrue(comparer.Equals('a', 'A'));
        }

        [TestMethod]
        public void CharHashCode()
        {
            var comparer = CharComparer.InvariantCultureIgnoreCase;
            Assert.AreNotEqual<int>('a'.GetHashCode(), comparer.GetHashCode('b'));
            Assert.AreEqual<int>('a'.GetHashCode(), comparer.GetHashCode('a'));
            Assert.AreEqual<int>('a'.GetHashCode(), comparer.GetHashCode('A'));
        }
    }
}
