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
    /// Adapts row columns as object properties for a <see cref="Record"/>.
    /// </summary>
    public sealed class RecordPropertyAdapter : PSPropertyAdapter
    {
        private Cache<string, PropertySet> cache = new Cache<string, PropertySet>();

        /// <summary>
        /// Gets the path to the package that contains this <see cref="Record"/> for use in code methods.
        /// </summary>
        /// <param name="obj">The <see cref="PSObject"/> that wraps a <see cref="Record"/>.</param>
        /// <returns>The path to the package that contains this <see cref="Record"/>.</returns>
        public static string GetPath(PSObject obj)
        {
            var record = obj.As<Record>();
            if (null != record)
            {
                return record.Path;
            }

            return null;
        }
        /// <summary>
        /// Gets the query string that returned the <see cref="Record"/> for use in code methods.
        /// </summary>
        /// <param name="obj">The <see cref="PSObject"/> that wraps a <see cref="Record"/>.</param>
        /// <returns>The query string that returned the <see cref="Record"/>.</returns>
        public static string GetQuery(PSObject obj)
        {
            var record = obj.As<Record>();
            if (null != record && null != record.Columns)
            {
                return record.Columns.QueryString;
            }

            return null;
        }

        /// <summary>
        /// Gets a collection of properties for a <see cref="Record"/>.
        /// </summary>
        /// <param name="baseObject">The <see cref="Record"/> to adapt.</param>
        /// <returns>A collection of properties for a <see cref="Record"/>.</returns>
        public override Collection<PSAdaptedProperty> GetProperties(object baseObject)
        {
            var record = baseObject as Record;
            if (null != record)
            {
                var properties = this.EnsurePropertyCache(record);
                if (null != properties)
                {
                    return properties.ToCollection();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a property for a <see cref="Record"/> with the given name.
        /// </summary>
        /// <param name="baseObject">The <see cref="Record"/> to adapt.</param>
        /// <param name="propertyName">The column name of a <see cref="Record"/>.</param>
        /// <returns>A property for a <see cref="Record"/>.</returns>
        public override PSAdaptedProperty GetProperty(object baseObject, string propertyName)
        {
            var record = baseObject as Record;
            if (null != record)
            {
                var properties = this.EnsurePropertyCache(record);
                if (null != properties && properties.Contains(propertyName))
                {
                    // Clone the property so the right value is retrieved.
                    var property = properties[propertyName];
                    return new PSAdaptedProperty(propertyName, property.Tag);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the native <see cref="Type"/> name for the property value.
        /// </summary>
        /// <param name="adaptedProperty">The property to check.</param>
        /// <returns>The native <see cref="Type"/> name for the property value.</returns>
        /// <exception cref="InvalidOperationException">The property did not contain enough information to complete this operation.</exception>
        public override string GetPropertyTypeName(PSAdaptedProperty adaptedProperty)
        {
            var column = adaptedProperty.Tag as Column;
            if (null != column)
            {
                return column.ColumnType.FullName;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="adaptedProperty">The property to get.</param>
        /// <returns>The property value.</returns>
        /// <exception cref="InvalidOperationException">The property did not contain enough information to complete this operation.</exception>
        public override object GetPropertyValue(PSAdaptedProperty adaptedProperty)
        {
            if (null != adaptedProperty)
            {
                return GetPropertyValue(adaptedProperty, adaptedProperty.BaseObject as Record);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Adds the table name, if available, to the beginning of the type name hierarchy.
        /// </summary>
        /// <param name="baseObject">The <see cref="Record"/> to process.</param>
        /// <returns>The adapted type name hierarchy.</returns>
        public override Collection<string> GetTypeNameHierarchy(object baseObject)
        {
            var typeNames = base.GetTypeNameHierarchy(baseObject);

            var record = baseObject as Record;
            if (null != record)
            {
                var properties = this.EnsurePropertyCache(record);
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

        private PropertySet EnsurePropertyCache(Record record)
        {
            var columns = record.Columns;
            if (null != columns)
            {
                PropertySet properties;
                if (!this.cache.TryGetValue(columns.QueryString, out properties))
                {
                    properties = new PropertySet();

                    for (int i = 0; i < columns.Count; ++i)
                    {
                        var column = columns[i];
                        properties.Add(new PSAdaptedProperty(column.Key, column));
                    }

                    // Format a suitable type name if only a single table was selected.
                    if (null != columns.TableNames && 1 == columns.TableNames.Length)
                    {
                        properties.TypeName = typeof(Record).FullName + "#" + columns.TableNames[0];
                    }

                    this.cache.Add(columns.QueryString, properties);
                }

                return properties;
            }

            return null;
        }

        internal static object GetPropertyValue(PSAdaptedProperty adaptedProperty, Record record)
        {
            var column = adaptedProperty.Tag as Column;
            if (null != column && null != record)
            {
                return record.Data[column.Index];
            }

            throw new InvalidOperationException();
        }
    }
}
