// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Creates a system restore point.
    /// </summary>
    internal sealed class SystemRestorePoint
    {
        private NativeMethods.RestorePointInfo info;

        /// <summary>
        /// Begins a system restore point.
        /// </summary>
        /// <param name="type">The type of restore point to create.</param>
        /// <param name="description">Optional description for the restore point. The default value is the assembly title.</param>
        /// <returns>Information about the restore point or null if the service is disabled.</returns>
        /// <exception cref="Win32Exception">Failed to create the system restore point.</exception>
        internal static SystemRestorePoint Create(RestorePointType type, string description = null)
        {
            if (RestorePointType.CancelledOperation == type)
            {
                throw new InvalidEnumArgumentException("type", (int)type, type.GetType());
            }

            if (string.IsNullOrEmpty(description))
            {
                // Get the assembly title metadata value.
                var title = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyTitleAttribute), false)
                    .FirstOrDefault();

                if (null != title)
                {
                    description = title.Title;
                }
            }

            var info = new NativeMethods.RestorePointInfo()
            {
                Description = description,
                EventType = NativeMethods.RestorePointEventType.BeginSystemChange,
                SequenceNumber = 0,
                Type = type,
            };

            // Create the system restore point.
            NativeMethods.StateManagerStatus status;
            if (!NativeMethods.SRSetRestorePoint(ref info, out status))
            {
                throw new Win32Exception(status.ErrorCode);
            }

            // Update the sequence number.
            info.SequenceNumber = status.SequenceNumber;

            return new SystemRestorePoint(info);
        }

        /// <summary>
        /// Gets the description of the restore point.
        /// </summary>
        internal string Description
        {
            get { return this.info.Description; }
        }

        /// <summary>
        /// Cancels the system restore point.
        /// </summary>
        internal void Rollback()
        {
            if (NativeMethods.RestorePointEventType.EndSystemChange != info.EventType)
            {
                info.Type = RestorePointType.CancelledOperation;
                info.EventType = NativeMethods.RestorePointEventType.EndSystemChange;

                NativeMethods.StateManagerStatus status;
                if (!NativeMethods.SRSetRestorePoint(ref info, out status))
                {
                    throw new Win32Exception(status.ErrorCode);
                }
            }
        }

        /// <summary>
        /// Completes the system restore point.
        /// </summary>
        internal void Commit()
        {
            if (NativeMethods.RestorePointEventType.EndSystemChange != info.EventType)
            {
                info.EventType = NativeMethods.RestorePointEventType.EndSystemChange;

                NativeMethods.StateManagerStatus status;
                if (!NativeMethods.SRSetRestorePoint(ref info, out status))
                {
                    throw new Win32Exception(status.ErrorCode);
                }
            }
        }

        private SystemRestorePoint(NativeMethods.RestorePointInfo info)
        {
            this.info = info;
        }
    }

    /// <summary>
    /// The type of restore point to create.
    /// </summary>
    internal enum RestorePointType
    {
        /// <summary>
        /// An application is being installed.
        /// </summary>
        ApplicationInstall,

        /// <summary>
        /// An application is being uninstalled.
        /// </summary>
        ApplicationUninstall,

        /// <summary>
        /// An application is being modified.
        /// </summary>
        ModifySettings = 12,

        /// <summary>
        /// The restore point is to be canceled. Call <see cref="SystemRestorePoint.Rollback"/> instead.
        /// </summary>
        CancelledOperation,
    }
}
