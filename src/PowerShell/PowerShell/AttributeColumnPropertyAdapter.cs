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
    /// Provides properties for each enumeration value to easy query column values.
    /// </summary>
    public sealed class AttributeColumnPropertyAdapter : PSPropertyAdapter
    {
        private static readonly string PropertyPrefix = "Has";
        private static readonly string ValueProperty = "Value";
        private Cache<Type, PropertySet> cache = new Cache<Type, PropertySet>();

        /// <summary>
        /// Gets a collection of properties for an <see cref="AttributeColumn"/>.
        /// </summary>
        /// <param name="baseObject">The <see cref="AttributeColumn"/> to adapt.</param>
        /// <returns>A collection of properties for a <see cref="AttributeColumn"/>.</returns>
        public override Collection<PSAdaptedProperty> GetProperties(object baseObject)
        {
            var column = baseObject as AttributeColumn;
            if (null != column)
            {
                var properties = this.EnsurePropertyCache(column);
                if (null != properties)
                {
                    return properties.ToCollection();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a property for a <see cref="AttributeColumn"/> with the given name.
        /// </summary>
        /// <param name="baseObject">The <see cref="AttributeColumn"/> to adapt.</param>
        /// <param name="propertyName">The column name of a <see cref="AttributeColumn"/>.</param>
        /// <returns>A property for a <see cref="AttributeColumn"/>.</returns>
        public override PSAdaptedProperty GetProperty(object baseObject, string propertyName)
        {
            var column = baseObject as AttributeColumn;
            if (null != column)
            {
                var properties = this.EnsurePropertyCache(column);
                if (null != properties && properties.Contains(propertyName))
                {
                    // Clone the property so the right value is retrieved.
                    var property = properties[propertyName];
                    return new PSAdaptedProperty(property.Name, property.Tag);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the native <see cref="Type"/> name for the property value.
        /// </summary>
        /// <param name="adaptedProperty">The property to check.</param>
        /// <returns>Always returns the full type name for a <see cref="bool"/>.</returns>
        public override string GetPropertyTypeName(PSAdaptedProperty adaptedProperty)
        {
            var column = adaptedProperty.BaseObject as AttributeColumn;
            return GetPropertyTypeNameInternal(adaptedProperty, column);
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="adaptedProperty">The property to get.</param>
        /// <returns>The property value.</returns>
        public override object GetPropertyValue(PSAdaptedProperty adaptedProperty)
        {
            var column = adaptedProperty.BaseObject as AttributeColumn;
            return GetPropertyValueInternal(adaptedProperty, column);
        }

        /// <summary>
        /// Adds the enumeration type to the beginning of the type name hierarchy.
        /// </summary>
        /// <param name="baseObject">The <see cref="AttributeColumn"/> to process.</param>
        /// <returns>The adapted type name hierarchy.</returns>
        public override Collection<string> GetTypeNameHierarchy(object baseObject)
        {
            var typeNames = base.GetTypeNameHierarchy(baseObject);

            var column = baseObject as AttributeColumn;
            if (null != column)
            {
                var properties = this.EnsurePropertyCache(column);
                if (!string.IsNullOrEmpty(properties.TypeName))
                {
                    typeNames.Insert(0, properties.TypeName);
                }
            }

            return typeNames;
        }

        /// <summary>
        /// Gets whether the property is gettable.
        /// </summary>
        /// <param name="adaptedProperty">The property to check.</param>
        /// <returns>Always returns true.</returns>
        public override bool IsGettable(PSAdaptedProperty adaptedProperty)
        {
            return true;
        }

        /// <summary>
        /// Gets whether the property is settable.
        /// </summary>
        /// <param name="adaptedProperty">The property to check.</param>
        /// <returns>Always returns false.</returns>
        public override bool IsSettable(PSAdaptedProperty adaptedProperty)
        {
            return false;
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="adaptedProperty">The property to set.</param>
        /// <param name="value">The property value to set.</param>
        /// <exception cref="NotSupportedException">The operation is not supported.</exception>
        public override void SetPropertyValue(PSAdaptedProperty adaptedProperty, object value)
        {
            throw new NotSupportedException();
        }

        internal static string GetPropertyTypeNameInternal(PSAdaptedProperty adaptedProperty, AttributeColumn column)
        {
            if (null != column)
            {
                var name = (string)adaptedProperty.Tag;
                if (!string.IsNullOrEmpty(name) && Enum.IsDefined(column.Type, name))
                {
                    return typeof(bool).FullName;
                }
                else if (ValueProperty.Equals(adaptedProperty.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return column.Type.FullName;
                }
            }

            return typeof(object).FullName;
        }

        internal static object GetPropertyValueInternal(PSAdaptedProperty adaptedProperty, AttributeColumn column)
        {
            if (null != column)
            {
                var name = (string)adaptedProperty.Tag;
                if (!string.IsNullOrEmpty(name) && Enum.IsDefined(column.Type, name))
                {
                    return column.HasValue(name);
                }
                else if (ValueProperty.Equals(adaptedProperty.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return column.Value;
                }
            }

            return 0;
        }

        private PropertySet EnsurePropertyCache(AttributeColumn column)
        {
            PropertySet properties;
            if (!this.cache.TryGetValue(column.Type, out properties))
            {
                properties = new PropertySet();

                foreach (var name in column.GetNames())
                {
                    var property = new PSAdaptedProperty(PropertyPrefix + name, name);
                    properties.Add(property);
                }

                // Add a property for the numeric value.
                properties.Add(new PSAdaptedProperty(ValueProperty, null));

                // Add the enumeration type to the list of type names.
                properties.TypeName = typeof(AttributeColumn).FullName + "#" + column.Type.Name;

                // Cache the properties to avoid extra allocations.
                this.cache.Add(column.Type, properties);
            }

            return properties;
        }
    }
}
