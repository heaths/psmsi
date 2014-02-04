// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Abstraction of the System Restore service.
    /// </summary>
    internal interface ISystemRestoreService
    {
        /// <summary>
        /// Creates or modifies a system restore point.
        /// </summary>
        /// <param name="info">Information about the restore point to create or modify.</param>
        /// <param name="status">Status information about the restore point created or modified.</param>
        /// <returns>True of the restore point was created or modified successfully; otherwise, false.</returns>
        bool SetRestorePoint(RestorePointInfo info, out StateManagerStatus status);
    }
}
