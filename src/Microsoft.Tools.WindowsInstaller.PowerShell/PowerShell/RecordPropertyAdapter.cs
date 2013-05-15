// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Adapts row columns as object properties for a <see cref="Record"/>.
    /// </summary>
    public sealed class RecordPropertyAdapter : PSPropertyAdapter
    {
        private Cache<string, PropertySet> cache = new Cache<string, PropertySet>();

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
            var field = adaptedProperty.Tag as FieldInfo;
            if (null != field)
            {
                return field.ColumnType.FullName;
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
            var view = GetView(record);
            if (view != null)
            {
                PropertySet properties;
                if (!this.cache.TryGetValue(view.QueryString, out properties))
                {
                    properties = new PropertySet();
                    for (int i = 0; i < view.Columns.Count; ++i)
                    {
                        var column = view.Columns[i];
                        properties.Add(new PSAdaptedProperty(column.Name, new FieldInfo(view, i)));
                    }

                    this.cache.Add(view.QueryString, properties);
                }

                return properties;
            }

            return null;
        }

        private static View GetView(Record record)
        {
            var field = typeof(Record).GetField("view", BindingFlags.Instance | BindingFlags.NonPublic);
            if (null != field)
            {
                return field.GetValue(record) as View;
            }

            return null;
        }

        internal static object GetPropertyValue(PSAdaptedProperty adaptedProperty, Record record)
        {
            // Caller will check, so simply assert for testing.
            Debug.Assert(null != adaptedProperty);

            var field = adaptedProperty.Tag as FieldInfo;
            if (null != field && null != record)
            {
                return field.GetValue(record);
            }

            throw new InvalidOperationException();
        }

        private class FieldInfo
        {
            private bool required;

            internal FieldInfo(View view, int index)
            {
                this.Index = index;
                this.required = false;

                // Remember the column type.
                var column = view.Columns[index];
                var type = column.Type;

                this.SimpleType = type;

                if (typeof(string) == type)
                {
                    this.ColumnType = typeof(string);
                }
                else if (typeof(Stream) == type)
                {
                    this.ColumnType = typeof(byte[]);
                }
                else if (column.IsRequired)
                {
                    this.required = true;
                    this.ColumnType = type;
                }
                else if (typeof(short) == type)
                {
                    this.ColumnType = typeof(Nullable<short>);
                }
                else
                {
                    this.ColumnType = typeof(Nullable<int>);
                }
            }

            internal int Index { get; private set; }
            internal Type SimpleType { get; private set; }
            internal Type ColumnType { get; private set; }

            internal object GetValue(Record record)
            {
                // Windows Installer records use 1-based indices.
                var index = this.Index + 1;

                if (null == record)
                {
                    return null;
                }
                else if (typeof(string) == this.SimpleType)
                {
                    return record.GetString(index);
                }
                else if (typeof(Stream) == this.SimpleType)
                {
                    // TODO: Return byte array for stream.
                    return new byte[] { };
                }
                else if (this.required)
                {
                    return record.GetInteger(index);
                }
                else
                {
                    return record.GetNullableInteger(index);
                }
            }
        }

        private class PropertySet : KeyedCollection<string, PSAdaptedProperty>
        {
            protected override string GetKeyForItem(PSAdaptedProperty item)
            {
                if (null == item)
                {
                    throw new ArgumentNullException("item");
                }

                return item.Name;
            }

            internal Collection<PSAdaptedProperty> ToCollection()
            {
                return new Collection<PSAdaptedProperty>(this.Items);
            }
        }
    }
}
