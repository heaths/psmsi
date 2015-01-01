// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Service provider for the Windows Installer logging policy.
    /// </summary>
    internal interface ILoggingPolicyService
    {
        /// <summary>
        /// Gets the logging policy.
        /// </summary>
        /// <returns>The system logging policy.</returns>
        string GetLoggingPolicy();

        /// <summary>
        /// Sets the logging policy
        /// </summary>
        /// <param name="value">The logging policy to set.</param>
        void SetLoggingPolicy(string value);

        /// <summary>
        /// Removes the logging policy.
        /// </summary>
        void RemoveLoggingPolicy();
    }
}
