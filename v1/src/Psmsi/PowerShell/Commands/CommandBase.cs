// Base Cmdlet for this snap-in.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Mon, 17 Mar 2008 02:58:53 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Abstract base class which provides useful methods for outputting objects to the pipeline
    /// and diagnosing problems.
    /// </summary>
    public abstract class CommandBase : PSCmdlet
    {
        /// <summary>
        /// Creates a <see cref="PSObject"/> that contains the actual object.
        /// </summary>
        /// <param name="value">The object to output to the pipeline.</param>
        protected void WritePSObject(object value)
        {
            if (!this.Stopping && null != value)
            {
                PSObject psobj = PSObject.AsPSObject(value);
                AddMembers(psobj);

                WriteObject(psobj);
            }
        }

        /// <summary>
        /// Adds properties and methods to the output object before writing it to the pipeline.
        /// </summary>
        /// <param name="psobj">The <see cref="PSObject"/> to add properties and methods.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "psobj")]
        protected virtual void AddMembers(PSObject psobj)
        {
        }

        /// <summary>
        /// Logs a debug message useful for diagnosing the native enumeration function calls.
        /// </summary>
        /// <param name="functionName">The name of the native function to be called.</param>
        /// <param name="args">Input parameters to the enumeration function.</param>
        protected void CallingNativeFunction(string functionName, params object[] args)
        {
            string message = Help.FormatDebugMessage(functionName, args);
            WriteDebug(message);
        }

        /// <summary>
        /// Writes non-terminating errors records to the pipeline for common errors.
        /// </summary>
        /// <param name="returnCode">The return code from a native Windows Installer function.</param>
        /// <returns>The original or changed return code.</returns>
        /// <remarks>
        /// Common error cases are handled, but inheritors can override <see cref="GetErrorDetails"/>
        /// to provide cmdlet-specific error details and recommended action.
        /// </remarks>
        protected int HandleCommonErrors(int returnCode)
        {
            switch (returnCode)
            {
                case NativeMethods.ERROR_ACCESS_DENIED:
                    {
                        Win32Exception ex = new Win32Exception(returnCode);
                        ErrorRecord err = new ErrorRecord(ex, "AccessDenied", ErrorCategory.PermissionDenied, null);
                        err.ErrorDetails = GetErrorDetails(returnCode);

                        this.WriteError(err);
                    }
                    return NativeMethods.ERROR_NO_MORE_ITEMS;

                case NativeMethods.ERROR_BAD_CONFIGURATION:
                    {
                        Win32Exception ex = new Win32Exception(returnCode);
                        ErrorRecord err = new ErrorRecord(ex, "BadConfiguration", ErrorCategory.ReadError, null);
                        err.ErrorDetails = GetErrorDetails(returnCode);

                        this.WriteError(err);
                    }
                    return NativeMethods.ERROR_NO_MORE_ITEMS;
            }

            return returnCode;
        }

        /// <summary>
        /// Returns <see cref="ErrorDetails"/> containing error information.
        /// </summary>
        /// <param name="returnCode">The return code for which <see cref="ErrorDetails"/> should be retrieved.</param>
        /// <returns>If the <paramref name="returnCode"/> is handled, an <see cref="ErrorDetails"/> object
        /// with additional information; otherwise, null.</returns>
        protected virtual ErrorDetails GetErrorDetails(int returnCode)
        {
            switch (returnCode)
            {
                case NativeMethods.ERROR_ACCESS_DENIED:
                    {
                        ErrorDetails err = new ErrorDetails(Properties.Resources.Error_AccessDenied);
                        err.RecommendedAction = Properties.Resources.Recommend_RunElevated;
                        return err;
                    }

                default:
                    return null;
            }
        }
    }
}
