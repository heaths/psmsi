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

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Service provider used for testing system restore points without actually creating them (requires elevation).
    /// </summary>
    internal class SystemRestoreTestService : ISystemRestoreService
    {
        private int nextErrorCode = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRestoreTestService"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number to return to the caller. The default is 1.</param>
        internal SystemRestoreTestService(long sequenceNumber = 1)
        {
            this.SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Gets the sequence number used for the restore point created.
        /// </summary>
        internal long SequenceNumber { get; private set; }

        /// <summary>
        /// Creates or modifies the system restore point.
        /// </summary>
        /// <param name="info">Information about the restore point to create or modify.</param>
        /// <param name="status">Status information of the restore point created or modified.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>
        /// The error code is reset to 0 (success) after each call.
        /// </remarks>
        /// <seealso cref="SetNextErrorCode"/>
        public bool SetRestorePoint(RestorePointInfo info, out StateManagerStatus status)
        {
            status.SequenceNumber = this.SequenceNumber;
            status.ErrorCode = this.nextErrorCode;

            // Reset next error code.
            this.nextErrorCode = 0;

            return 0 == status.ErrorCode;
        }

        /// <summary>
        /// Sets the next error code.
        /// </summary>
        /// <param name="errorCode">The next error code to set.</param>
        internal void SetNextErrorCode(int errorCode)
        {
            this.nextErrorCode = errorCode;
        }
    }
}
