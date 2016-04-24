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
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// A collection of <see cref="PSAdaptedProperty"/> objects keyed by the property name.
    /// </summary>
    internal sealed class PropertySet : KeyedCollection<string, PSAdaptedProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySet"/> class.
        /// </summary>
        internal PropertySet()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Gets or sets the additional type name for the object being adapted.
        /// </summary>
        internal string TypeName { get; set; }

        /// <summary>
        /// Gets the key for the given <see cref="PSAdaptedProperty"/>.
        /// </summary>
        /// <param name="item">The <see cref="PSAdaptedProperty"/> to index.</param>
        /// <returns>The key for the given <see cref="PSAdaptedProperty"/>.</returns>
        protected override string GetKeyForItem(PSAdaptedProperty item)
        {
            if (null == item)
            {
                throw new ArgumentNullException("item");
            }

            return item.Name;
        }

        /// <summary>
        /// Converts the collection of items to a collection of type <see cref="PSAdaptedProperty"/>.
        /// </summary>
        /// <returns>A collection of adapter properties.</returns>
        internal Collection<PSAdaptedProperty> ToCollection()
        {
            return new Collection<PSAdaptedProperty>(this.Items);
        }
    }
}
