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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Information about change made to a package by a transform or patch.
    /// </summary>
    internal sealed class TransformView
    {
        private class Table
        {
            internal Table()
            {
                this.Rows = new RowCollection();
            }

            internal TableOperation Operation { get; private set; }

            internal RowCollection Rows { get; private set; }

            internal void SetPrincipalOperation(TableOperation operation)
            {
                if (this.Operation < operation)
                {
                    this.Operation = operation;
                }
            }
        }

        private class Row
        {
            internal Row(string primarKey)
            {
                this.PrimaryKey = primarKey;
            }

            internal RowOperation Operation { get; private set; }

            internal string PrimaryKey { get; private set; }

            internal void SetPrincipalOperation(RowOperation operation)
            {
                if (this.Operation < operation)
                {
                    this.Operation = operation;
                }
            }
        }

        private class RowCollection : KeyedCollection<string, Row>
        {
            protected override string GetKeyForItem(Row item)
            {
                if (null == item)
                {
                    throw new ArgumentNullException("item");
                }

                return item.PrimaryKey;
            }
        }

        /// <summary>
        /// Gets the name of the table containing transform information.
        /// </summary>
        internal static readonly string TableName = "_TransformView";

        private static readonly string Space = " ";
        private static readonly string Tab = "\t";
        private static readonly IList<string> TableOperations = new string[] { "CREATE", "INSERT", "DELETE", "DROP" };

        private readonly IDictionary<string, Table> tables;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformView"/> class.
        /// </summary>
        /// <param name="db">The <see cref="Database"/> containing the _TransformView table to process.</param>
        internal TransformView(Database db)
        {
            if (null == db)
            {
                throw new ArgumentNullException("db");
            }

            this.tables = new Dictionary<string, Table>();

            var table = db.Tables[TransformView.TableName];
            if (null != table)
            {
                this.Initialize(db, table);
            }
        }

        /// <summary>
        /// Gets the names of tables affected by the transform.
        /// </summary>
        internal ICollection<string> Tables
        {
            get { return this.tables.Keys; }
        }

        /// <summary>
        /// Gets the <see cref="RowOperation"/> for the identified row in the specific table.
        /// </summary>
        /// <param name="tableName">The name of the table containing the row.</param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <returns>The <see cref="RowOperation"/> for the identified row.</returns>
        internal RowOperation GetRowOperation(string tableName, string primaryKey)
        {
            if (!string.IsNullOrEmpty(tableName) && this.tables.ContainsKey(tableName))
            {
                var table = this.tables[tableName];
                if (TableOperation.Create == table.Operation)
                {
                    return RowOperation.Insert;
                }
                else if (TableOperation.Drop == table.Operation)
                {
                    return RowOperation.Delete;
                }
                else if (TableOperation.Modify == table.Operation)
                {
                    if (!string.IsNullOrEmpty(primaryKey) && table.Rows.Contains(primaryKey))
                    {
                        return table.Rows[primaryKey].Operation;
                    }
                }
            }

            return RowOperation.None;
        }

        /// <summary>
        /// Gets the <see cref="TableOperation"/> for the specific table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableOperation"/> for the specific table.</returns>
        internal TableOperation GetTableOperation(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName) && this.tables.ContainsKey(tableName))
            {
                var table = this.tables[tableName];
                return table.Operation;
            }

            return TableOperation.None;
        }

        private void Initialize(Database db, Microsoft.Deployment.WindowsInstaller.TableInfo info)
        {
            using (var view = db.OpenView(info.SqlSelectString))
            {
                view.Execute();

                var record = view.Fetch();
                while (null != record)
                {
                    using (record)
                    {
                        var table = record.GetString(1);
                        var column = record.GetString(2);
                        var key = record.GetString(3);

                        Table tbl;
                        if (!this.tables.ContainsKey(table))
                        {
                            tbl = new Table();
                            this.tables.Add(table, tbl);
                        }
                        else
                        {
                            tbl = this.tables[table];
                        }

                        Row row = null;
                        if (!string.IsNullOrEmpty(key))
                        {
                            var primaryKey = key
                                .Replace(TransformView.Tab, Record.KeySeparator)
                                .Replace(TransformView.Space, string.Empty);

                            if (!tbl.Rows.Contains(primaryKey))
                            {
                                row = new Row(primaryKey);
                                tbl.Rows.Add(row);
                            }
                            else
                            {
                                row = tbl.Rows[primaryKey];
                            }
                        }

                        switch (TransformView.TableOperations.IndexOf(column))
                        {
                            case 0:
                                tbl.SetPrincipalOperation(TableOperation.Create);
                                break;

                            case 1:
                                tbl.SetPrincipalOperation(TableOperation.Modify);
                                row.SetPrincipalOperation(RowOperation.Insert);
                                break;

                            case 2:
                                tbl.SetPrincipalOperation(TableOperation.Modify);
                                row.SetPrincipalOperation(RowOperation.Delete);
                                break;

                            case 3:
                                tbl.SetPrincipalOperation(TableOperation.Drop);
                                break;

                            default:
                                tbl.SetPrincipalOperation(TableOperation.Modify);

                                if (null != row)
                                {
                                    row.SetPrincipalOperation(RowOperation.Modify);
                                }

                                break;
                        }
                    }

                    record = view.Fetch();
                }
            }
        }
    }
}
