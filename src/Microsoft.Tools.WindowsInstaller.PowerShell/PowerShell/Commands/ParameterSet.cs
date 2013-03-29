// Parameter set name constants.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.


namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Helps avoid misspellings throughout the code.
    /// </summary>
    internal static class ParameterSet
    {
        /// <summary>
        /// Parameter set for component information.
        /// </summary>
        internal const string Component = "Component";

        /// <summary>
        /// Parameter set for feature information.
        /// </summary>
        internal const string Feature = "Feature";

        /// <summary>
        /// Parameter set for product information.
        /// </summary>
        internal const string Product = "Product";

        /// <summary>
        /// Parameter set for patch information.
        /// </summary>
        internal const string Patch = "Patch";

        /// <summary>
        /// Parameter set for either product or patch information.
        /// </summary>
        internal const string Installation = "Installation";

        /// <summary>
        /// Parameter set for a path supporting wildcards.
        /// </summary>
        internal const string Path = "Path";

        /// <summary>
        /// Parameter set for a literal path.
        /// </summary>
        internal const string LiteralPath = "LiteralPath";

        /// <summary>
        /// Parameter set for a Name parameter.
        /// </summary>
        internal const string Name = "Name";
    }
}
