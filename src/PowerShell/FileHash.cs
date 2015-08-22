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
using System.Runtime.InteropServices;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Represents a nullable file hash.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class FileHash : IEnumerable<int>
    {
        [MarshalAs(UnmanagedType.U4)]
        private int dwFileHashInfoSie;
        [MarshalAs(UnmanagedType.U4)]
        private int dwData0;
        [MarshalAs(UnmanagedType.U4)]
        private int dwData1;
        [MarshalAs(UnmanagedType.U4)]
        private int dwData2;
        [MarshalAs(UnmanagedType.U4)]
        private int dwData3;

        /// <summary>
        /// Creates a new instance of the <see cref="FileHash"/> class.
        /// </summary>
        internal FileHash()
        {
            this.dwFileHashInfoSie = Marshal.SizeOf(this);
        }

        /// <summary>
        /// Gets the first hash part.
        /// </summary>
        public int MSIHashPart1
        {
            get { return this.dwData0; }
        }

        /// <summary>
        /// Gets the second hash part.
        /// </summary>
        public int MSIHashPart2
        {
            get { return this.dwData1; }
        }

        /// <summary>
        /// Gets the third hash part.
        /// </summary>
        public int MSIHashPart3
        {
            get { return this.dwData2; }
        }

        /// <summary>
        /// Gets the fourth hash part.
        /// </summary>
        public int MSIHashPart4
        {
            get { return this.dwData3; }
        }

        /// <summary>
        /// Gets an enumerator over the four nullable integer hash parts.
        /// </summary>
        /// <returns>An enumerator over the four nullable integer hash parts.</returns>
        public IEnumerator<int> GetEnumerator()
        {
            var list = new List<int>() { this.MSIHashPart1, this.MSIHashPart2, this.MSIHashPart3, this.MSIHashPart4 };
            return list.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator over the four hash parts.
        /// </summary>
        /// <returns>An enumerator over the four hash parts.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
