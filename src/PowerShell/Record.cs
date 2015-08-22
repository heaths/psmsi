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

using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains locally cached values from a <see cref="Deployment.WindowsInstaller.Record"/> object
    /// since it requires an open handle to the storage file.
    /// </summary>
    public sealed class Record
    {
        /// <summary>
        /// Gets the separator string for primary key columns.
        /// </summary>
        internal static readonly string KeySeparator = "/";

        /// <summary>
        /// Creates a new instance of the <see cref="Record"/> class copying values
        /// from the <see cref="Deployment.WindowsInstaller.Record"/> object.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> from which to copy values.</param>
        /// <param name="columns">The <see cref="ColumnCollection"/> for a <see cref="Deployment.WindowsInstaller.View"/>.</param>
        /// <param name="transform">The <see cref="TransformView"/> containing information about operations performed on this record.</param>
        /// <param name="path">The path to the package that contains the record.</param>
        internal Record(Deployment.WindowsInstaller.Record record, ColumnCollection columns, TransformView transform = null, string path = null)
        {
            // Internal constructor will assume valid parameters.

            // Shared reference to the column collection.
            this.Columns = columns;

            this.Path = path;

            // Cache the data from the record.
            var primaryKeys = new List<string>();
            this.Data = new List<object>(columns.Count);
            for (int i = 0; i < columns.Count; ++i)
            {
                // Windows Installer uses 1-based indices.
                var offset = i + 1;

                var type = columns[i].Type;
                if (type.IsEnum)
                {
                    var value = record.GetNullableInteger(offset);
                    this.Data.Add(new AttributeColumn(type, value));

                    if (columns[i].IsPrimaryKey)
                    {
                        primaryKeys.Add(null != value ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
                    }
                }
                else if (typeof(Stream) == type)
                {
                    var buffer = CopyStream(record, offset);
                    this.Data.Add(buffer);

                    // Binary column types cannot be primary keys.
                }
                else
                {
                    var data = record[offset];
                    this.Data.Add(data);

                    if (columns[i].IsPrimaryKey)
                    {
                        primaryKeys.Add(null != data ? data.ToString() : string.Empty);
                    }
                }
            }

            if (0 < primaryKeys.Count)
            {
                this.PrimaryKey = primaryKeys.Join(Record.KeySeparator);
            }

            // Can only reliably get row operations performed on rows in a single table.
            if (null != transform && 1 == columns.TableNames.Count())
            {
                var tableName = columns.TableNames[0];
                this.Operation = transform.GetRowOperation(tableName, this.PrimaryKey);
            }
            else
            {
                this.Operation = RowOperation.None;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="Column"/> information for this <see cref="Record"/>.
        /// </summary>
        internal ColumnCollection Columns { get; private set; }

        /// <summary>
        /// Gets the locally cached record data.
        /// </summary>
        internal List<object> Data { get; private set; }

        /// <summary>
        /// Gets the operation performed in the record by a patch or transform.
        /// </summary>
        internal RowOperation Operation { get; private set; }

        /// <summary>
        /// Gets the path to the package that contains this <see cref="Record"/>.
        /// </summary>
        internal string Path { get; private set; }

        /// <summary>
        /// Gets the primary key for the row.
        /// </summary>
        /// <value>The primary key with multiple fields separated by forward slashes.</value>
        internal string PrimaryKey { get; private set; }

        private static byte[] CopyStream(Deployment.WindowsInstaller.Record record, int index)
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[4096];
                int read = 0;
                var stream = record.GetStream(index);

                do
                {
                    read = stream.Read(buffer, 0, buffer.Length);
                    if (0 < read)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }
                while (0 < read);

                return ms.ToArray();
            }
        }
    }
}
