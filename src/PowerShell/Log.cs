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
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Generates log file names.
    /// </summary>
    internal sealed class Log
    {
        private static readonly string FileTemplate = "_{0:yyyyMMddhhmmss}_{1:000}";
        private static readonly string DefaultPrefix = "MSI";
        private static readonly string DefaultExtension = ".log";

        private string path;
        private string filename;
        private string extension;
        private int index;
        private DateTime start;
        private bool initialized;

        /// <summary>
        /// Creates a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="path">Optional path to use for directory and file base names.</param>
        internal Log(string path)
            : this(path, DateTime.Now)
        {
            // Uses local time for easy identification.
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="path">Optional path to use for directory and file base names.</param>
        /// <param name="start">The <see cref="DateTime"/> for the log identifier.</param>
        internal Log(string path, DateTime start)
        {
            this.path = path;
            this.filename = null;
            this.extension = null;
            this.index = 0;
            this.start = start;
            this.initialized = false;

            // Default is "voicewarmup".
            this.Mode = InstallLogModes.Verbose | InstallLogModes.OutOfDiskSpace | InstallLogModes.Info | InstallLogModes.CommonData
                        | InstallLogModes.Error | InstallLogModes.Warning | InstallLogModes.ActionStart | InstallLogModes.ActionData
                        | InstallLogModes.FatalExit | InstallLogModes.User | InstallLogModes.PropertyDump;

            // If a path was specified, log additional information.
            if (!string.IsNullOrEmpty(path))
            {
                this.Mode |= InstallLogModes.ExtraDebug;
            }
        }

        /// <summary>
        /// Gets the <see cref="InstallLogModes"/> to log.
        /// </summary>
        /// <value>The default is equivalent to "voicewarmup". Providing a path to the constructor will add the equivalent of "x".</value>
        internal InstallLogModes Mode { get; private set; }

        /// <summary>
        /// Gets the next log file name in the set.
        /// </summary>
        /// <param name="extra">Extra information to add to the log file name, such as a ProductCode or file name. Can be null.</param>
        /// <returns>The next log file name in the set.</returns>
        internal string Next(string extra)
        {
            this.Initialize();

            // Always add the groupable, sortable identifier to the file path.
            string filename = this.filename + string.Format(CultureInfo.InvariantCulture, FileTemplate, this.start, this.index++);
            if (!string.IsNullOrEmpty(extra))
            {
                return filename + "_" + extra + this.extension;
            }
            else
            {
                return filename + this.extension;
            }
        }

        private void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.path))
            {
                string temp = Path.GetTempPath();
                string name = DefaultPrefix + DefaultExtension;

                this.path = Path.Combine(temp, name);
            }

            if (string.IsNullOrEmpty(this.filename))
            {
                string path = Path.GetDirectoryName(this.path);
                string name = Path.GetFileNameWithoutExtension(this.path);

                this.filename = Path.Combine(path, name);
            }

            if (string.IsNullOrEmpty(this.extension))
            {
                this.extension = Path.GetExtension(this.path);
            }

            // If the log mode is not already set up for debugging, check the system policy.
            if (InstallLogModes.ExtraDebug != (InstallLogModes.ExtraDebug & this.Mode))
            {
                string policy = GetLoggingPolicy();
                if (!string.IsNullOrEmpty(policy) && 0 <= policy.IndexOf("X", StringComparison.OrdinalIgnoreCase))
                {
                    this.Mode |= InstallLogModes.ExtraDebug;
                }
            }

            this.initialized = true;
        }

        private static string GetLoggingPolicy()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"Software\Policies\Microsoft\Windows\Installer");
            if (null != key)
            {
                using (key)
                {
                    return key.GetValue("Logging") as string;
                }
            }

            return null;
        }
    }
}