// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains information about a column from a <see cref="View"/>.
    /// </summary>
    internal sealed class Column
    {
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

            // Get the basic type and the type for properties.
            var type = column.Type;
            this.Type = type;

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
        /// Gets the index of the column within a <see cref="View"/>.
        /// </summary>
        internal int Index { get; private set; }

        /// <summary>
        /// Ges the name of the column.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets the basic <see cref="Type"/> of the column.
        /// </summary>
        internal Type Type { get; private set; }

        /// <summary>
        /// Gets the column <see cref="Type"/>.
        /// </summary>
        internal Type ColumnType { get; private set; }
    }
}
