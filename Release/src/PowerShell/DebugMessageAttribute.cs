// Attribute debugger messages with methods (intended for P/Invoke methods).
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Mon, 17 Mar 2008 15:45:35 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Windows.Installer.PowerShell
{
    /// <summary>
    /// Message to display in debug output for the attributed method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    sealed class DebugMessageAttribute : Attribute
    {
        string format;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")] int paramCount;

        /// <summary>
        /// Creates a new instance of the <see cref="DebugMessageAttribute"/> class.
        /// </summary>
        /// <param name="format">The format string associated with the attributed method.</param>
        /// <param name="paramCount">The number of parameters in the <paramref name="format"/> string.</param>
        internal DebugMessageAttribute(string format, int paramCount)
        {
            this.format = format;
            this.paramCount = paramCount;
        }

        /// <summary>
        /// Formats the attached format string with the given arguments.
        /// </summary>
        /// <param name="args">Arguments to pass into the format string.</param>
        /// <returns>The formatted string using <see cref="CultureInfo.InvariantCulture"/>.</returns>
        internal string Format(params object[] args)
        {
            Debug.Assert(this.paramCount == args.Length);
            return string.Format(CultureInfo.InvariantCulture, this.format, args);
        }
    }
}
