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
    /// Tests for the <see cref="Tree&lt;T&gt;"/> class.
    /// </summary>
    [TestClass]
    public sealed class TreeTests
    {
        [TestMethod]
        public void CheckUnderDirectories()
        {
            var tree = new Tree<bool>();
            tree.Add(@"C:\A\1", true);
            tree.Add(@"C:\A\2", true);
            tree.Add(@"C:\B", true);
            tree.Add(@"C:\B\1", false);

            Assert.IsTrue(tree.Under(@"C:\A\1\foo"));
            Assert.IsTrue(tree.Under(@"C:\B\2\foo"));
            Assert.IsTrue(tree.Under(@"C:\B"));
            Assert.IsFalse(tree.Under(@"C:\B\1\foo"));
            Assert.IsFalse(tree.Under(@"C:\C\foo"));
            Assert.IsFalse(tree.Under(@"D:\foo"));
        }

        [TestMethod]
        public void CheckUnderDirectoriesWithWildcard()
        {
            var tree = new Tree<bool>();
            tree.Add(@"C:\X", true);
            tree.Add(@"C:\X\*", false);

            Assert.IsTrue(tree.Under(@"C:\X"));
            Assert.IsFalse(tree.Under(@"C:\X\foo"));
            Assert.IsFalse(tree.Under(@"C:\X\1\foo"));
            Assert.IsFalse(tree.Under(@"D:\foo"));
        }

        [TestMethod]
        public void CheckCaseSensitiveSentence()
        {
            var tree = new Tree<bool>(new char[] { ' ', ',', '.' }, StringComparer.Ordinal);
            tree.Add("The quick brown fox jumps.", true);

            Assert.IsTrue(tree.Under("The quick brown fox jumps over the lazy dog."));
            Assert.IsFalse(tree.Under("THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG."));
            Assert.IsFalse(tree.Under("A different sentence."));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullKeyThrows()
        {
            var tree = new Tree<bool>();
            tree.Add(null, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullKeyUnderThrows()
        {
            var tree = new Tree<bool>();
            tree.Add(@"C:\A", true);

            tree.Under(null);
        }
    }
}
