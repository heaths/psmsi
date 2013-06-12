// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;
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
        internal Column(View view, int index)
        {
            // Internal constructor will assume valid parameters.

            this.Index = index;

            var column = view.Columns[index];
            this.Name = column.Name;

            // Get the name of the table that contains the column.
            this.Table = Column.GetTableName(view, this.Name);

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

        private static string GetTableName(View view, string columnName)
        {
            string tableName = null;
            foreach (var table in view.Tables)
            {
                // Determine which table contains the column.
                if (table.Columns.Contains(columnName))
                {
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        // TODO: Automatically disambiguate columns names.
                        string message = string.Format(Resources.Error_AmbiguousColumn, columnName, tableName, table.Name);
                        throw new ArgumentException(message);
                    }

                    tableName = table.Name;
                }
            }

            return tableName;
        }

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
