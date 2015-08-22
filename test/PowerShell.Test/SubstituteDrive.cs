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

            driveLetter = Char.ToUpperInvariant(driveLetter);
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
