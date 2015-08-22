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

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains information about a column from a <see cref="View"/>.
    /// </summary>
    internal sealed class Column
    {
        private static readonly Dictionary<string, Type> AttributeTypes = new Dictionary<string, Type>()
        {
            { "CompLocator.Type", typeof(LocatorTypes) },
            { "Component.Attributes", typeof(ComponentAttributes) },
            { "Control.Attributes", typeof(ControlAttributes) },
            { "CustomAction.Type", typeof(CustomActionTypes) },
            { "Dialog.Attributes", typeof(DialogAttributes) },
            { "Feature.Attributes", typeof(FeatureAttributes) },
            { "File.Attributes", typeof(Deployment.WindowsInstaller.FileAttributes) },
            { "IniLocator.Type", typeof(LocatorTypes) },
            { "RegLocator.Type", typeof(LocatorTypes) },
            { "RemoveFile.InstallMode", typeof(RemoveFileModes) },
            { "ServiceControl.Event", typeof(ServiceControlEvents) },
            { "ServiceInstall.ErrorControl", typeof(ServiceAttributes) },
            { "ServiceInstall.ServiceType", typeof(ServiceAttributes) },
            { "ServiceInstall.StartType", typeof(ServiceAttributes) },
            { "TextStyle.StyleBits", typeof(TextStyles) },
            { "Upgrade.Attributes", typeof(UpgradeAttributes) },
        };

        /// <summary>
        /// Creates a new instance of the <see cref="Column"/> class from a <see cref="View"/> and column index.
        /// </summary>
        /// <param name="view">The <see cref="View"/> from which column information is retrieved.</param>
        /// <param name="index">The index of the column within the <see cref="View"/>.</param>
        /// <param name="table">The table name to which the column belongs.</param>
        /// <param name="name">The name of the column.</param>
        /// <param name="key">The unique key containing the specified column name from the original query.</param>
        internal Column(View view, int index, string table, string name, string key)
        {
            // Internal constructor will assume valid parameters.

            this.Index = index;
            var column = view.Columns[index];

            // Set column information from the collection.
            this.Table = table;
            this.Name = name;
            this.Key = key;

            this.IsPrimaryKey = view.Tables
                .Where(t => t.Name == table)
                .Any(t => t.PrimaryKeys.Contains(name));

            // BUG: PatchPackage - typically added by patches - is not identified as having a primary key (PatchId).

            // Determine the simple type that defines the field value.
            var type = Column.GetEnumerationType(this.Table, this.Name) ?? column.Type;
            this.Type = type;

            // Determine the property type based on the simple type.
            if (type.IsEnum)
            {
                this.ColumnType = typeof(AttributeColumn);
            }
            else if (typeof(string) == type)
            {
                this.ColumnType = typeof(string);
            }
            else if (typeof(Stream) == type)
            {
                this.ColumnType = typeof(byte[]);
            }
            else if (column.IsRequired)
            {
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

        /// <summary>
        /// Gets the column <see cref="Type"/>.
        /// </summary>
        internal Type ColumnType { get; private set; }

        /// <summary>
        /// Gets the index of the column within a <see cref="View"/>.
        /// </summary>
        internal int Index { get; private set; }

        /// <summary>
        /// Gets whether the column is a primary key in its containing table.
        /// </summary>
        internal bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// Gets the unique key containing the specified column name from the original query.
        /// </summary>
        internal string Key { get; private set; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets the name of the table to which the column belongs.
        /// </summary>
        internal string Table { get; private set; }

        /// <summary>
        /// Gets the basic <see cref="Type"/> of the column.
        /// </summary>
        internal Type Type { get; private set; }

        private static Type GetEnumerationType(string tableName, string columnName)
        {
            var key = string.Concat(tableName, ".", columnName);
            if (AttributeTypes.ContainsKey(key))
            {
                return AttributeTypes[key];
            }

            return null;
        }
    }
}
