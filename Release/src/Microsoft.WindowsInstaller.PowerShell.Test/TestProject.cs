// Suppoort methods and properties for the test project.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Security.Principal;

namespace Microsoft.WindowsInstaller
{
    /// <summary>
    /// Support methods and properties for the test project.
    /// </summary>
    internal static class TestProject
    {
        /// <summary>
        /// Gets the SID in SDDL form for the current user.
        /// </summary>
        internal static string CurrentSID
        {
            get
            {
                using (WindowsIdentity id = WindowsIdentity.GetCurrent())
                {
                    SecurityIdentifier sid = id.User;
                    return sid.Value;
                }
            }
        }
    }
}
