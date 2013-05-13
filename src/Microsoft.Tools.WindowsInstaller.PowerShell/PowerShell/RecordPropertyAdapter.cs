// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.ObjectModel;
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
                var view = GetView(record);
                if (null != view)
                {
                    var properties = new Collection<PSAdaptedProperty>();
                    foreach (var column in view.Columns)
                    {
                        properties.Add(GetProperty(view, record, column.Name));
                    }

                    return properties;
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
                var view = GetView(record);
                if (null != view)
                {
                    return GetProperty(view, record, propertyName);
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
                var type = field.GetColumn().Type;
                if (typeof(Stream) == type)
                {
                    // Will return a byte array instead.
                    type = typeof(byte[]);
                }

                return type.FullName;
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
            var field = adaptedProperty.Tag as FieldInfo;
            if (null != field)
            {
                var type = field.GetColumn().Type;
                if (typeof(string) == type)
                {
                    return field.GetStringValue();
                }
                else if (typeof(Stream) == type)
                {
                    return field.GetBinaryValue();
                }
                else
                {
                    return field.GetIntegerValue();
                }
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

        private static View GetView(Record record)
        {
            var field = typeof(Record).GetField("view", BindingFlags.Instance | BindingFlags.NonPublic);
            if (null != field)
            {
                return field.GetValue(record) as View;
            }

            return null;
        }

        private static PSAdaptedProperty GetProperty(View view, Record record, string name)
        {
            return new PSAdaptedProperty(name, new FieldInfo() { View = view, Record = record, Name = name });
        }

        private class FieldInfo
        {
            internal View View;
            internal Record Record;
            internal string Name;

            internal ColumnInfo GetColumn()
            {
                return this.View.Columns[this.Name];
            }

            internal byte[] GetBinaryValue()
            {
                throw new NotImplementedException();
            }

            internal int GetIntegerValue()
            {
                return this.Record.GetInteger(this.Name);
            }

            internal string GetStringValue()
            {
                return this.Record.GetString(this.Name);
            }
        }
    }
}
