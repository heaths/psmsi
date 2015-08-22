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

using System;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Simple caching for key/value pairs using a simple last used time algorithm.
    /// </summary>
    /// <typeparam name="TKey">The type to hash into the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value to store in the dictionary.</typeparam>
    internal sealed class Cache<TKey, TValue>
    {
        internal const int DefaultCapacity = 4;

        private Dictionary<TKey, CacheItem> store;
        private CacheItemSorter sorter;

        /// <summary>
        /// Creates a new instance of the <see cref="Cache&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        internal Cache() : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Cache&lt;TKey, TValue&gt;"/> class using the given <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The initial capacity that should not be exceeded.</param>
        internal Cache(int capacity)
        {
            if (0 >= capacity)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            this.Capacity = capacity;
            this.store = new Dictionary<TKey, CacheItem>(capacity);
            this.sorter = new CacheItemSorter();
        }

        /// <summary>
        /// Gets the capacity of the <see cref="Cache&lt;TKey, TValue&gt;"/>.
        /// </summary>
        /// <remarks>
        /// If capacity is reduced, older objects will be removed on the next addition to the <see cref="Cache&lt;TKey, TValue&gt;"/>.
        /// </remarks>
        internal int Capacity { get; private set; }

        /// <summary>
        /// Gets the number of objects in the <see cref="Cache&lt;TKey, TValue&gt;"/>.
        /// </summary>
        internal int Count
        {
            get { return this.store.Count; }
        }

        /// <summary>
        /// Gets the collection of keys.
        /// </summary>
        internal ICollection<TKey> Keys
        {
            get { return this.store.Keys; }
        }

        /// <summary>
        /// Adds an object to the <see cref="Cache&lt;TKey, TValue&gt;"/> or updates an existing one.
        /// </summary>
        /// <param name="key">The key of the object to add.</param>
        /// <param name="value">The object value to add.</param>
        /// <remarks>
        /// If adding an object would exceed the <see cref="Capacity"/>, older objects are removed.
        /// </remarks>
        internal void Add(TKey key, TValue value)
        {
            if (this.store.ContainsKey(key))
            {
                this.store[key].Value = value;
            }
            else
            {
                this.store.Add(key, new CacheItem(key, value));
            }

            this.Shrink();
        }

        /// <summary>
        /// Tries to get a value from the <see cref="Cache&lt;TKey, TValue&gt;"/>.
        /// </summary>
        /// <param name="key">The key of the object to get.</param>
        /// <param name="value">The object value to get, or null if the key does not exist in the <see cref="Cache&lt;TKey, TValue&gt;"/>.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.store.ContainsKey(key))
            {
                value = this.store[key].Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        private void Shrink()
        {
            if (this.Count > this.Capacity)
            {
                var list = new List<CacheItem>(this.store.Values);
                list.Sort(this.sorter);

                foreach (var item in list)
                {
                    if (this.Count <= this.Capacity)
                    {
                        break;
                    }

                    this.store.Remove(item.Key);
                }
            }
        }

        private class CacheItem
        {
            private TValue value;

            internal CacheItem(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }

            internal DateTime LastAccessed { get; private set; }
            internal TKey Key { get; private set; }
            internal TValue Value
            {
                get
                {
                    this.Update();
                    return this.value;
                }

                set
                {
                    this.Update();
                    this.value = value;
                }
            }

            private void Update()
            {
                this.LastAccessed = DateTime.Now;
            }
        }

        private class CacheItemSorter : IComparer<CacheItem>
        {
            private IComparer<TKey> comparer;

            internal CacheItemSorter()
            {
                this.comparer = Comparer<TKey>.Default;
            }

            public int Compare(CacheItem x, CacheItem y)
            {
                if (null == x)
                {
                    return null == y ? 0 : -1;
                }
                else if (null == y)
                {
                    return 1;
                }

                int result = DateTime.Compare(x.LastAccessed, y.LastAccessed);
                if (0 == result)
                {
                    return this.comparer.Compare(x.Key, y.Key);
                }

                return result;
            }
        }
    }
}
