// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="Cache&lt;TKey, TValue&gt;"/> class.
    /// </summary>
    [TestClass]
    public sealed class CacheTests
    {
        [TestMethod]
        public void DefaultCacheCapacity()
        {
            var cache = new Cache<string, int>();
            Assert.AreEqual<int>(4, cache.Capacity, "The default capacity is incorrect.");
        }

        [TestMethod]
        public void CustomCacheCapacity()
        {
            var cache = new Cache<string, int>(8);
            Assert.AreEqual<int>(8, cache.Capacity, "The custom capacity is incorrect.");
        }

        [TestMethod]
        public void BasicCaching()
        {
            var cache = new Cache<string, int>(2);

            cache.Add("A", 1);
            cache.Add("B", 2);
            Assert.AreEqual<int>(2, cache.Count, "The count is incorrect.");

            cache.Add("C", 3);
            Assert.AreEqual<int>(2, cache.Count, "The count is incorrect.");

            var keys = new List<string>(cache.Keys);
            CollectionAssert.DoesNotContain(keys, "A", "The cache contained 'A'.");
            CollectionAssert.Contains(keys, "B", "The cache did not contain 'B'.");
            CollectionAssert.Contains(keys, "C", "The cache did not contain 'C'.");
        }

        [TestMethod]
        public void ConditionalCaching()
        {
            var cache = new Cache<string, int>(2);

            int value;
            if (!cache.TryGetValue("A", out value))
            {
                value = 1;
                cache.Add("A", value);
            }

            Assert.AreEqual<int>(1, cache.Count, "The count is incorrect.");
            Assert.IsTrue(cache.TryGetValue("A", out value), "The cache did not contain 'A'.");
            Assert.AreEqual<int>(1, value, "The cached value is incorrect.");
        }

        [TestMethod]
        public void UpdateCache()
        {
            var cache = new Cache<string, int>(2);
            cache.Add("A", 1);

            int value;
            Assert.IsTrue(cache.TryGetValue("A", out value), "The cache did not contain 'A'.");
            Assert.AreEqual<int>(1, value, "The cached value is incorrect.");

            cache.Add("A", 2);
            Assert.IsTrue(cache.TryGetValue("A", out value), "The cache did not contain 'A'.");
            Assert.AreEqual<int>(2, value, "The cached value is incorrect.");
        }
    }
}
