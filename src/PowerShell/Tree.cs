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
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Simple tree class to assign a value to a hierarchy leaf node.
    /// </summary>
    /// <typeparam name="T">The type of the value to assign to the leaf node.</typeparam>
    /// <remarks>
    /// This class supports multiple concurrent readers but modifications are not thread safe.
    /// </remarks>
    internal sealed class Tree<T>
    {
        private readonly Node root;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}"/> class.
        /// </summary>
        /// <param name="separators">Optional separators for any key added to the tree. The default is '\'.</param>
        /// <param name="comparer">Optional string comparer for any key segment added to the tree. The default is <see cref="StringComparer.InvariantCultureIgnoreCase"/>.</param>
        internal Tree(char[] separators = null, StringComparer comparer = null)
        {
            this.root = new Node(this);
            this.root.Value = default(T);

            this.Separators = separators ?? new char[] { Path.DirectorySeparatorChar };
            this.Comparer = comparer ?? StringComparer.InvariantCultureIgnoreCase;
        }

        /// <summary>
        /// Gets the <see cref="StringComparer"/> used to compare key segments.
        /// </summary>
        internal StringComparer Comparer { get; private set; }

        /// <summary>
        /// Gets the separators used to split the key into segments.
        /// </summary>
        internal char[] Separators { get; private set; }

        /// <summary>
        /// Splits the <paramref name="key"/> into segments and adds each key as a hierarchy, assigning the <paramref name="value"/> to the leaf node.
        /// </summary>
        /// <param name="key">The key to split into segments and add to the tree.</param>
        /// <param name="value">The value to assign to the leaf node (last key segment).</param>
        /// <remarks>
        /// This method is not thread safe. Access to the tree when adding keys should be synchronized.
        /// </remarks>
        internal void Add(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            var node = this.root;

            var segments = key.Split(this.Separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                node = node.Add(segment);
            }

            node.Value = value;
        }

        /// <summary>
        /// Gets the value assigned to the last matching key segment, or the default value for the value type if not found.
        /// </summary>
        /// <param name="key">The key to split into segments and compare against the hierarchical tree nodes to find a value.</param>
        /// <returns>The value assigned to the last matching key segment, or the default value for the value type if not found.</returns>
        internal T Under(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            var node = this.root;
            var value = node.Value;

            var segments = key.Split(this.Separators, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < segments.Length && null != node; ++i)
            {
                var segment = segments[i];
                node = node.Find(segment);

                if (null != node && node.HasValue)
                {
                    value = node.Value;
                }
            }

            return value;
        }

        private class Node
        {
            private static readonly string Wildcard = "*";

            private Tree<T> tree;
            private IDictionary<string, Node> children = null;
            private T value = default(T);
            private bool hasValue = false;

            internal Node(Tree<T> tree)
            {
                this.tree = tree;
            }

            internal T Value
            {
                get
                {
                    return this.value;
                }

                set
                {
                    this.value = value;
                    this.hasValue = true;
                }
            }

            internal bool HasValue
            {
                get { return this.hasValue; }
            }

            internal Node Add(string key)
            {
                if (null == this.children)
                {
                    this.children = new Dictionary<string, Node>(this.tree.Comparer);

                    var node = new Node(this.tree);
                    this.children.Add(key, node);

                    return node;
                }
                else
                {
                    if (!this.children.ContainsKey(key))
                    {
                        var node = new Node(this.tree);
                        this.children.Add(key, node);

                        return node;
                    }
                    else
                    {
                        return this.children[key];
                    }
                }
            }

            internal Node Find(string key)
            {
                // Match the subkey or any wildcard stored at this level in the tree.
                if (null != this.children)
                {
                    if (this.children.ContainsKey(key))
                    {
                        return this.children[key];
                    }

                    if (this.children.ContainsKey(Node.Wildcard))
                    {
                        return this.children[Node.Wildcard];
                    }
                }

                return null;
            }
        }
    }
}
