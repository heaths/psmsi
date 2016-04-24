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

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Different logging modes supported by Windows Installer.
    /// </summary>
    /// <remarks>
    /// This enumeration is defined in lieu of <see cref="InstallLogModes"/> to support
    /// only the subset of logging modes supported by Windows Installer, instead of all
    /// possible messages that can be sent to a user interface handler.
    /// </remarks>
    [Flags]
    public enum LoggingPolicies
    {
        /// <summary>
        /// No logging.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Out-of-memory or fatal exit information.
        /// </summary>
        FatalExit = 0x1,

        /// <summary>
        /// All error messages.
        /// </summary>
        Error = 0x2,

        /// <summary>
        /// Non-fatal warnings.
        /// </summary>
        Warning = 0x4,

        /// <summary>
        /// User requests.
        /// </summary>
        User = 0x8,

        /// <summary>
        /// Status messages.
        /// </summary>
        Information = 0x10,

        /// <summary>
        /// Out-of-disk-space messages.
        /// </summary>
        OutOfDiskSpace = 0x80,

        /// <summary>
        /// Start up of actions.
        /// </summary>
        ActionStart = 0x100,

        /// <summary>
        /// Action-specific records.
        /// </summary>
        ActionData = 0x200,

        /// <summary>
        /// Dump properties on exit.
        /// </summary>
        PropertyDump = 0x400,

        /// <summary>
        /// Initial user interface parameters.
        /// </summary>
        CommonData = 0x800,

        /// <summary>
        /// Verbose output.
        /// </summary>
        Verbose = 0x1000,

        /// <summary>
        /// Extra debugging information.
        /// </summary>
        ExtraDebug = 0x2000,

        /// <summary>
        /// Log everything.
        /// </summary>
        /// <remarks>
        /// This is equivalent to "*vx" or "voicewarmupx" on the command line.
        /// </remarks>
        All = Verbose | OutOfDiskSpace | Information | CommonData | Error |
                          Warning | ActionStart | ActionData | FatalExit | User | PropertyDump | ExtraDebug,

        /// <summary>
        /// Flush each line to the log file.
        /// </summary>
        /// <remarks>
        /// This can cause performance degradation and is not recommended.
        /// </remarks>
        FlushEachLine = 0x10000000,
    }
}
