// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.Generic;
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
        /// Creates a new instance of the <see cref="Record"/> class copying values
        /// from the <see cref="Deployment.WindowsInstaller.Record"/> object.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> from which to copy values.</param>
        /// <param name="columns">The <see cref="ColumnCollection"/> for a <see cref="Deployment.WindowsInstaller.View"/>.</param>
        /// <param name="path">The path to the package that contains the record.</param>
        internal Record(Deployment.WindowsInstaller.Record record, ColumnCollection columns, string path = null)
        {
            // Internal constructor will assume valid parameters.

            // Shared reference to the column collection.
            this.Columns = columns;

            // Store the path to the package.
            this.Path = path;

            // Cache the data from the record.
            this.Data = new List<object>(record.FieldCount);
            for (int i = 0; i < record.FieldCount; ++i)
            {
                // Windows Installer uses 1-based indices.
                var offset = i + 1;

                var type = columns[i].Type;
                if (type.IsEnum)
                {
                    var value = record.GetNullableInteger(offset);
                    this.Data.Add(new AttributeColumn(type, value));
                }
                else if (typeof(Stream) == type)
                {
                    var buffer = CopyStream(record, offset);
                    this.Data.Add(buffer);
                }
                else
                {
                    this.Data.Add(record[offset]);
                }
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
        /// Gets the path to the package that contains this <see cref="Record"/>.
        /// </summary>
        internal string Path { get; private set; }

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
