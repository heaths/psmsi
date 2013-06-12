// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Returns registered paths for specified components.
    /// </summary>
    internal static class ComponentSearcher
    {
        /// <summary>
        /// The known component to find.
        /// </summary>
        internal enum KnownComponent
        {
            Orca,
            Darice,
        }

        /// <summary>
        /// Returns the component path if any component is installed.
        /// </summary>
        /// <param name="what">What component to find.</param>
        /// <returns>The component path if any component is installed.</returns>
        internal static string Find(KnownComponent what)
        {
            ICollection<string> componentIds = null;
            switch (what)
            {
                case KnownComponent.Orca:
                    componentIds = new string[]
                    {
                        "{958A3933-8CE7-6189-F0EF-CAE467FABFF4}", // Orca > 8.0
                        "{BE928E10-272A-11D2-B2E4-006097C99860}", // Orca <= 5.0
                    };
                    break;

                case KnownComponent.Darice:
                    componentIds = new string[]
                    {
                        "{D865CA5E-9B46-B345-B3A6-43C5EAF209E0}", // Orca > 8.0
                        "{EAB27DFE-90C6-11D2-88AC-00A0C981B015}", // Orca <= 5.0
                    };
                    break;
            }

            foreach (var componentId in componentIds)
            {
                var component = new ComponentInstallation(componentId);
                if (InstallState.Local == component.State)
                {
                    return component.Path;
                }
            }

            return null;
        }
    }
}
