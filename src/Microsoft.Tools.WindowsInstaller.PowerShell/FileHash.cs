// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
