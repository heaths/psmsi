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

using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Creates a system restore point.
    /// </summary>
    internal sealed class SystemRestorePoint
    {
        private static ISystemRestoreService defaultService;
        private ISystemRestoreService service;
        private RestorePointInfo info;

        /// <summary>
        /// Begins a system restore point.
        /// </summary>
        /// <param name="type">The type of restore point to create.</param>
        /// <param name="description">Optional description for the restore point. The default value is the assembly title.</param>
        /// <param name="service">The service provider to create or modify restore points. The default calls info the system service.</param>
        /// <returns>Information about the restore point or null if the service is disabled.</returns>
        /// <exception cref="Win32Exception">Failed to create the system restore point.</exception>
        internal static SystemRestorePoint Create(RestorePointType type, string description = null, ISystemRestoreService service = null)
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

            var info = new RestorePointInfo()
            {
                Description = description,
                EventType = RestorePointEventType.BeginSystemChange,
                SequenceNumber = 0,
                Type = type,
            };

            var instance = new SystemRestorePoint()
            {
                service = service ?? SystemRestorePoint.DefaultServiceProvider,
                info = info,
            };

            // Create the system restore point.
            var status = instance.SetRestorePoint(info);

            // Update the sequence number.
            info.SequenceNumber = status.SequenceNumber;
            instance.info = info;

            return instance;
        }

        /// <summary>
        /// Gets or sets the default implementation of the <see cref="ISystemRestoreService"/>.
        /// </summary>
        /// <remarks>
        /// If not already set or reset to null, the default system implementation is used.
        /// </remarks>
        internal static ISystemRestoreService DefaultServiceProvider
        {
            get { return defaultService ?? DefaultSystemRestoreService.GetInstance(); }
            set { defaultService = value; }
        }

        /// <summary>
        /// Gets the description of the restore point.
        /// </summary>
        internal string Description
        {
            get { return this.info.Description; }
        }

        /// <summary>
        /// Gets the sequence number for the restore point.
        /// </summary>
        internal long SequenceNumber
        {
            get { return this.info.SequenceNumber; }
        }

        /// <summary>
        /// Completes the system restore point.
        /// </summary>
        internal void Commit()
        {
            if (RestorePointEventType.EndSystemChange != this.info.EventType)
            {
                // Copy information for next call.
                var info = this.info;
                info.EventType = RestorePointEventType.EndSystemChange;

                this.SetRestorePoint(info);
            }
        }

        /// <summary>
        /// Cancels the system restore point.
        /// </summary>
        internal void Rollback()
        {
            if (RestorePointEventType.EndSystemChange != this.info.EventType)
            {
                // Copy information for next call.
                var info = this.info;
                info.Type = RestorePointType.CancelledOperation;
                info.EventType = RestorePointEventType.EndSystemChange;

                this.SetRestorePoint(info);
            }
        }

        private SystemRestorePoint()
        {
        }

        private StateManagerStatus SetRestorePoint(RestorePointInfo info)
        {
            StateManagerStatus status;
            if (!this.service.SetRestorePoint(info, out status))
            {
                throw new Win32Exception(status.ErrorCode);
            }

            return status;
        }

        private class DefaultSystemRestoreService : ISystemRestoreService
        {
            private static volatile object monitor = new object();
            private static ISystemRestoreService instance = null;

            internal static ISystemRestoreService GetInstance()
            {
                if (null == instance)
                {
                    lock (monitor)
                    {
                        if (null == instance)
                        {
                            instance = new DefaultSystemRestoreService();
                        }
                    }
                }

                return instance;
            }

            public bool SetRestorePoint(RestorePointInfo info, out StateManagerStatus status)
            {
                return NativeMethods.SRSetRestorePoint(ref info, out status);
            }
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
