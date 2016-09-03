// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="Cache&lt;TKey, TValue&gt;"/> class.
    /// </summary>
    [TestClass]
    public sealed class CacheTests : TestBase
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
