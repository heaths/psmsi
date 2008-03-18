// Provides location information for PowerShell items.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Tue, 13 Mar 2007 10:33:09 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Management.Automation;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer.PowerShell
{
    internal static class Location
    {
        internal static string GetProviderQualifiedPath(string path, ProviderInfo provider)
        {
            Debug.Assert(null != path);
            Debug.Assert(null != provider);

            // Determine if the path already has a provider reference in it.
            int pos = path.IndexOf("::", StringComparison.Ordinal);
            if (pos >= 0)
            {
                // Adding a provider where one seems to already exists seems pointless.
                return path;
            }
            else
            {
                // If the provider is not referenced already, prefix the path with the provider.
                return string.Concat(provider.PSSnapIn.Name, "\\", provider.Name, "::", path);
            }
        }

        internal static string GetOneResolvedProviderPathFromPSObject(PSObject psObject, PSCmdlet cmd)
        {
            // Get the file system path.
            PSPropertyInfo psPathInfo = psObject.Properties["PSPath"];
            if (null != psPathInfo && null != psPathInfo.Value)
            {
                string psPath = psPathInfo.Value.ToString();
                if (null != psPath)
                {
                    ProviderInfo provider;
                    Collection<string> fsPaths = cmd.GetResolvedProviderPathFromPSPath(psPath, out provider);
                    if (null != fsPaths && 0 < fsPaths.Count)
                    {
                        return fsPaths[0];
                    }
                }
            }

            return null;
        }

        internal static void AddPSPath(string path, PSObject obj, PSCmdlet cmd)
        {
            Debug.Assert(null != path);
            Debug.Assert(null != obj);
            Debug.Assert(null != cmd);

            ProviderInfo provider = cmd.SessionState.Provider.GetOne("FileSystem");

            // Assumes incoming path is already resolved, since the actual path
            // may not exist, especially in the case of advertised products.
            obj.Properties.Add(new PSNoteProperty("PSPath",
                GetProviderQualifiedPath(path, provider)));
            obj.Properties.Add(new PSNoteProperty("PSParentPath",
                GetProviderQualifiedPath(Path.GetDirectoryName(path), provider)));
            obj.Properties.Add(new PSNoteProperty("PSChildName",
                Path.GetFileName(path)));
        }
    }
}
