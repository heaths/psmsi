// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
        /// Creates a case-insensitive instance of the <see cref="PropertySet"/> class.
        /// </summary>
        internal PropertySet() : base(StringComparer.OrdinalIgnoreCase)
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
        /// <returns></returns>
        internal Collection<PSAdaptedProperty> ToCollection()
        {
            return new Collection<PSAdaptedProperty>(this.Items);
        }
    }
}
