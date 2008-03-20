// Methods for debugging and diagnostics.
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Mon, 17 Mar 2008 15:46:32 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

namespace Microsoft.Windows.Installer.PowerShell
{
    /// <summary>
    /// Methods for outputting debug information.
    /// </summary>
    internal static class Help
    {
        static int Timeout = 100;

        static Dictionary<string, DebugMessageAttribute> debugMessages = new Dictionary<string, DebugMessageAttribute>();
        static ReaderWriterLock debugMessagesLock = new ReaderWriterLock();

        /// <summary>
        /// Formats the messages attributed to methods using the <see cref="DebugMessageAttribute"/> class.
        /// </summary>
        /// <param name="functionName">The name of the function.</param>
        /// <param name="args">The arguments to pass to the formatter.</param>
        /// <returns>The formatted string if available; otherwise, the function name.</returns>
        /// <remarks>
        /// All methods are assumed to be in the NativeMethods class.
        /// </remarks>
        internal static string FormatDebugMessage(string functionName, params object[] args)
        {
            // Check first if the message is already cached.
            if (!debugMessages.ContainsKey(functionName))
            {
                debugMessagesLock.AcquireWriterLock(Timeout);
                try
                {
                    // Get the method.
                    Type t = typeof(NativeMethods);
                    MethodInfo method = t.GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Static);

                    // Check for the DebugMessageAttribute. Multiples are not allowed by declaration.
                    DebugMessageAttribute[] attrs = (DebugMessageAttribute[])method.GetCustomAttributes(typeof(DebugMessageAttribute), false);
                    if (1 == attrs.Length)
                    {
                        debugMessages.Add(functionName, attrs[0]);
                    }
                    else
                    {
                        // Add attribute with just function name and no parameters.
                        debugMessages.Add(functionName, new DebugMessageAttribute(functionName, 0));
                    }
                }
                finally
                {
                    debugMessagesLock.ReleaseWriterLock();
                }
            }

            // At this point with no exceptions thrown, the dictionary contains our attribute
            // so just format the message for the given function name.
            debugMessagesLock.AcquireReaderLock(Timeout);
            try
            {
                return debugMessages[functionName].Format(args);
            }
            finally
            {
                debugMessagesLock.ReleaseReaderLock();
            }
        }
    }
}
