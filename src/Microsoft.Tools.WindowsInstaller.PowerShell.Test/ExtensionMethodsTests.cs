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
    /// <remarks>
    /// Methods are called explicitly from <see cref="ExtensionMethods"/> since the test project might otherwise
    /// use extension methods from System.Core that did not exist in .NET 2.0 assemblies.
    /// </remarks>
    [TestClass]
    public sealed class ExtensionMethodsTests : TestBase
    {
        #region FirstOrDefault
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FirstOrDefaultThrows()
        {
            string[] source = null;
            var item = ExtensionMethods.FirstOrDefault(source);
        }

        [TestMethod]
        public void FirstFromList()
        {
            var source = new List<string>() { "A", "B" };
            var item = ExtensionMethods.FirstOrDefault(source);

            Assert.AreEqual("A", item, "The first item is incorrect.");
        }

        [TestMethod]
        public void FirstFromEnumerable()
        {
            var source = new HashSet<string> { "A", "B" };
            var item = ExtensionMethods.FirstOrDefault(source);

            Assert.AreEqual("A", item, "The first item is incorrect.");
        }

        [TestMethod]
        public void DefaultFromEmptyList()
        {
            var source = new List<string>();
            var item = ExtensionMethods.FirstOrDefault(source);

            Assert.IsNull(item, "The default item is incorrect.");
        }
        #endregion

        #region Select
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SelectWithNullSource()
        {
            string[] source = null;
            Func<string, int> selector = null;
            var list = ExtensionMethods.Select(source, selector);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SelectWithNullSelector()
        {
            var source = new string[] { "A", "B" };
            Func<string, int> selector = null;
            var list = ExtensionMethods.Select(source, selector);
        }

        [TestMethod]
        public void Select()
        {
            var source = new string[] { "A", "B" };
            var list = new List<int>(ExtensionMethods.Select(source, x => x.Length));

            CollectionAssert.AreEqual(new int[] { 1, 1 }, list, "The projected list is incorrect.");
        }
        #endregion

        #region Sum
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SumWithNullSource()
        {
            string[] source = null;
            Func<string, long> selector = null;
            var sum = ExtensionMethods.Sum(source, selector);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SumWithNullSelector()
        {
            var source = new string[] { "A", "B" };
            Func<string, long> selector = null;
            var sum = ExtensionMethods.Sum(source, selector);
        }

        [TestMethod]
        public void Sum()
        {
            var source = new string[] { "A", "B" };
            var sum = ExtensionMethods.Sum(source, x => x.Length);

            Assert.AreEqual<long>(2, sum);
        }
        #endregion

        #region ToArray
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToArrayWithNullSource()
        {
            string[] source = null;
            var array = source.ToArray();
        }

        [TestMethod]
        public void ToArrayPassThrough()
        {
            string[] source = { "A", "B" };
            var array = source.ToArray();

            CollectionAssert.AreEqual(source, array, "The arrays are not equivalent.");
        }
        #endregion

        #region ToList
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToListWithNullSource()
        {
            string[] source = null;
            var list = source.ToList();
        }

        [TestMethod]
        public void ToListPassThrough()
        {
            string[] source = { "A", "B" };
            var list = source.ToList();

            // IList<T> doesn't implement ICollection, so we convert back to an array.
            CollectionAssert.AreEqual(source, list.ToArray(), "The arrays are not equivalent.");
        }
        #endregion
    }
}
