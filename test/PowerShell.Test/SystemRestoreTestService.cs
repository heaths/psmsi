// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Service provider used for testing system restore points without actually creating them (requires elevation).
    /// </summary>
    internal class SystemRestoreTestService : ISystemRestoreService
    {
        private int nextErrorCode = 0;

        /// <summary>
        /// Creates a new instance of the test <see cref="ISystemRestoreService"/> provider.
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
