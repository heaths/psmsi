// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// A collection of of type <see cref="Column"/> indexed by <see cref="Column.Name"/>.
    /// </summary>
    internal sealed class ColumnCollection : KeyedCollection<string, Column>
    {
        private static readonly string[] ColumnSeparators = new string[] { ",", " " };
        private static readonly char[] NameSeparators = new char[] { '.' };

        /// <summary>
        /// Creates a new instance of the <see cref="Column"/> class from the specified <see cref="View"/>.
        /// </summary>
        /// <param name="view">The <see cref="View"/> from which column information is retrieved.</param>
        internal ColumnCollection(View view)
        {
            // Internal constructor will assume valid parameter.

            // Save the table names
            if (null != view.Tables)
            {
                this.TableNames = view.Tables.Select(table => table.Name).ToArray();
            }

            // Save the query string for indexing.
            this.QueryString = view.QueryString;

            // Windows Installer would've already checked for disambiguous column names,
            // so get the column names as specified since the user may expect those property names.
            var columns = ColumnCollection.GetColumns(view);

            // Column all required column data.
            for (int i = 0; i < columns.Length; ++i)
            {
                var column = ColumnCollection.GetColumn(view, i, columns[i]);
                this.Add(column);
            }
        }

        /// <summary>
        /// Gets the table names used within the <see cref="QueryString"/>.
        /// </summary>
        internal string[] TableNames { get; private set; }

        /// <summary>
        /// Gets the query string used to create the original <see cref="View"/>.
        /// </summary>
        internal string QueryString { get; private set; }

        /// <summary>
        /// Gets the <see cref="Column.Name"/> of the <see cref="Column"/>.
        /// </summary>
        /// <param name="item">The <see cref="Column"/> to index.</param>
        /// <returns>The <see cref="Column.Name"/> of the <see cref="Column"/>.</returns>
        protected override string GetKeyForItem(Column item)
        {
            return item.Key;
        }

        private static string[] GetColumns(View view)
        {
            var query = view.QueryString;

            // Parse the column names for table references.
            var start = query.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) + 7;
            var end = query.IndexOf("FROM", StringComparison.OrdinalIgnoreCase) - 1;
            return query.Substring(start, end - start)
                        .Replace("`", "")
                        .Split(ColumnCollection.ColumnSeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        private static Column GetColumn(View view, int index, string name)
        {
            if (0 < name.IndexOf(ColumnCollection.NameSeparators[0]))
            {
                var pair = name.Split(ColumnCollection.NameSeparators, 2);
                return new Column(view, index, pair[0], pair[1], name);
            }
            else
            {
                // Find the table to which the unique column name belongs.
                foreach (var table in view.Tables)
                {
                    if (table.Columns.Contains(name))
                    {
                        return new Column(view, index, table.Name, name, name);
                    }
                }
            }

            // This should never happen since Windows Installer validates the query.
            throw new InvalidOperationException();
        }
    }
}
