// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Tools.WindowsInstaller
{
    internal class SubstituteDrive : IDisposable
    {
        private const int MAX_PATH = 260;

        [Flags]
        private enum DefineDosDeviceFlags
        {
            None = 0,
            RawTargetPath = 1,
            RemoveDefinition = 2,
            ExactMatchOnRemove = 4,
            NoBroadcastSystem = 8,

            RemoveExactMatch = RemoveDefinition | ExactMatchOnRemove,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefineDosDeviceW", SetLastError = true)]
        private static extern bool DefineDosDevice(
            [In, MarshalAs(UnmanagedType.U4)] DefineDosDeviceFlags dwFlags,
            [In] string lpDeviceName,
            [In] string lpTargetPath
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "QueryDosDeviceW", SetLastError = true)]
        private static extern int QueryDosDevice(
            [In] string lpDeviceName,
            [Out] StringBuilder lpTargetPath,
            [In, MarshalAs(UnmanagedType.U4)] int ucchMax
            );

        /// <summary>
        /// Creates a new drive substitution.
        /// </summary>
        /// <param name="driveLetter">The drive letter to use for the substitution.</param>
        /// <param name="targetPath">The path to the folder to map to a drive letter.</param>
        /// <exception cref="ArgumentException">The <paramref name="driveLetter"/> is already defined.</exception>
        /// <exception cref="Win32Exception">An error occurred when creating a drive subistution.</exception>
        internal SubstituteDrive(char driveLetter, string targetPath)
        {
            Contract.Requires('C' <= driveLetter && 'Z' >= driveLetter);
            Contract.Requires(!string.IsNullOrEmpty(targetPath));

            if (SubstituteDrive.IsDefined(driveLetter))
            {
                throw new ArgumentException("The drive letter is already defined.", "driveLetter");
            }

            this.DriveLetter = driveLetter + ":";
            this.TargetPath = targetPath;

            if (!SubstituteDrive.DefineDosDevice(DefineDosDeviceFlags.None, this.DriveLetter, this.TargetPath))
            {
                // Throw last error.
                throw new Win32Exception();
            }
        }

        ~SubstituteDrive()
        {
            this.Dispose();
        }

        /// <summary>
        /// Gets the drive letter for the substitution.
        /// </summary>
        /// <value>The drive letter for the substitution, or null if the substitution was already removed.</value>
        internal string DriveLetter { get; private set; }

        /// <summary>
        /// Gets the target path for the substitution.
        /// </summary>
        internal string TargetPath { get; private set; }

        /// <summary>
        /// Remove the drive substitution.
        /// </summary>
        public void Dispose()
        {
            if (null != this.DriveLetter)
            {
                SubstituteDrive.DefineDosDevice(DefineDosDeviceFlags.RemoveExactMatch, this.DriveLetter, this.TargetPath);

                this.DriveLetter = null;
                this.TargetPath = null;

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Gets a <see cref="SubstitutionDrve"/> for the next available drive letter.
        /// </summary>
        /// <param name="targetPath">The path to the folder to map to a drive letter.</param>
        /// <returns>A <see cref="SubstitutionDrive"/> for the <paramref name="targetPath"/>.</returns>
        internal static SubstituteDrive Next(string targetPath)
        {
            for (var c = 'C'; c <= 'Z'; ++c)
            {
                if (!SubstituteDrive.IsDefined(c))
                {
                    return new SubstituteDrive(c, targetPath);
                }
            }

            throw new InvalidOperationException("No drive letters are available.");
        }

        private static bool IsDefined(char driveLetter)
        {
            var drive = driveLetter + ":";
            var target = new StringBuilder(SubstituteDrive.MAX_PATH);

            return 0 != SubstituteDrive.QueryDosDevice(drive, target, target.Capacity);
        }
    }
}
