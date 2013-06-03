// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Collections.ObjectModel;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// A collection of of type <see cref="Column"/> indexed by <see cref="Column.Name"/>.
    /// </summary>
    internal sealed class ColumnCollection : KeyedCollection<string, Column>
    {
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

            // Column all required column data.
            for (int i = 0; i < view.Columns.Count; ++i)
            {
                var column = new Column(view, i);
                this.Add(column);
            }
        }

        /// <summary>
        /// Gets a single able named used within the <see cref="QueryString"/>.
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
            return item.Name;
        }
    }
}
