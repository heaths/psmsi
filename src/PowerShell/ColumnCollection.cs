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
using Microsoft.Tools.WindowsInstaller.Properties;
using System;
using System.Collections.Generic;
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
        /// <exception cref="InvalidOperationException">A column name was defined by multiple tables.</exception>
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

                if (this.Contains(column.Name))
                {
                    var message = string.Format(Resources.Error_AmbiguousColumn, column.Name);
                    throw new InvalidOperationException(message);
                }

                this.Add(column);
            }

            this.PrimaryKeys = this.Where(column => column.IsPrimaryKey).Select(column => column.Name);
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
        /// Gets the primary key columns.
        /// </summary>
        internal IEnumerable<string> PrimaryKeys { get; private set; }

        /// <summary>
        /// Gets the <see cref="Column.Name"/> of the <see cref="Column"/>.
        /// </summary>
        /// <param name="item">The <see cref="Column"/> to index.</param>
        /// <returns>The <see cref="Column.Name"/> of the <see cref="Column"/>.</returns>
        protected override string GetKeyForItem(Column item)
        {
            return item.Key;
        }

        private static IEnumerable<string> GetAllColumns(View view)
        {
            foreach (var table in view.Tables)
            {
                foreach (var column in table.Columns)
                {
                    // Windows Installer does not support wildcard column names
                    // with multiple tables so only need to return the column name.
                    yield return column.Name;
                }
            }
        }

        private static string[] GetColumns(View view)
        {
            var query = view.QueryString;

            // Parse the column names for table references.
            var start = query.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) + 7;
            var end = query.IndexOf("FROM", StringComparison.OrdinalIgnoreCase) - 1;
            var columns = query.Substring(start, end - start);

            if ("*" == columns.Trim())
            {
                // Return all columns from all tables.
                return ColumnCollection.GetAllColumns(view).ToArray();
            }
            else
            {
                // Return only specified columns.
                return columns
                    .Replace("`", "")
                    .Split(ColumnCollection.ColumnSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
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
