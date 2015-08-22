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

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Information about a table in a <see cref="Database"/>.
    /// </summary>
    public sealed class TableInfo
    {
        private static readonly IList<string> Empty = new List<string>(0).AsReadOnly();

        /// <summary>
        /// Creates a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="path">The full path to the <see cref="Database"/> containing the table.</param>
        /// <param name="transform">The <see cref="TransformView"/> containing information about operations performed on the table.</param>
        /// <param name="patches">A list of patches applied to the <see cref="Database"/> containing the table.</param>
        /// <param name="transforms">A list of transformed applied to the <see cref="Database"/> containing the table.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or an empty string.</exception>
        internal TableInfo(string name, string path, TransformView transform, IEnumerable<string> patches, IEnumerable<string> transforms)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.Table = name;
            this.Path = path;

            if (null != transform)
            {
                this.Operation = transform.GetTableOperation(name);
            }
            else
            {
                this.Operation = TableOperation.None;
            }

            if (null != patches)
            {
                this.Patch = new List<string>(patches).AsReadOnly();
            }
            else
            {
                // Parameter binding requires non-null list.
                this.Patch = TableInfo.Empty;
            }

            if (null != transforms)
            {
                this.Transform = new List<string>(transforms).AsReadOnly();
            }
            else
            {
                this.Transform = TableInfo.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Table { get; private set; }

        /// <summary>
        /// Gets the operation performed on the table by a patch or transform.
        /// </summary>
        public TableOperation Operation { get; private set; }

        /// <summary>
        /// Gets the full path to the <see cref="Database"/> containing the table.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the list of patches applied to the <see cref="Database"/> containing the table.
        /// </summary>
        public IList<string> Patch { get; private set; }

        /// <summary>
        /// Gets the list of transforms applied to the <see cref="Database"/> containing the table.
        /// </summary>
        public IList<string> Transform { get; private set; }
    }
}
