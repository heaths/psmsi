// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Provides information about Windows Installer components.
    /// </summary>
    public static class ComponentProvider
    {
        /// <summary>
        /// Gets an provider path for the given component key path.
        /// </summary>
        /// <param name="session">The <see cref="SessionState"/> for resolving key paths.</param>
        /// <param name="path">The component key path to resolve.</param>
        /// <returns>A provider path for the given component path.</returns>
        public static string ResolveKeyPath(SessionState session, string path)
        {
            if (session != null)
            {
                return session.Path.GetUnresolvedPSPathFromKeyPath(path);
            }

            return null;
        }
    }
}
