// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Simple collection of unique items.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    internal sealed class Set<T> : KeyedCollection<T, T>
    {
        /// <summary>
        /// Creates a new instance of the class with the given equality comparer.
        /// </summary>
        /// <param name="comparer">The equality comparer to use.</param>
        internal Set(IEqualityComparer<T> comparer = null) : base(comparer ?? EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Gets the item itself to use as the key.
        /// </summary>
        /// <param name="item">The item to hash.</param>
        /// <returns>The item itself to use as the key.</returns>
        protected override T GetKeyForItem(T item)
        {
            return item;
        }

        /// <summary>
        /// Inserts items into the set if not already preset.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (!this.Contains(item))
            {
                base.InsertItem(index, item);
            }
        }
    }
}
