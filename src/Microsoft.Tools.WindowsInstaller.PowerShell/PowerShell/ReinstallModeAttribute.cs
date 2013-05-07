// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Supports conversion between the short form REINSTALLMODE and the <see cref="ReinstallModes"/> enumeration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ReinstallModeAttribute : ArgumentTransformationAttribute
    {
        /// <summary>
        /// Transforms user account identifiers to security identifiers.
        /// </summary>
        /// <param name="engineIntrinsics">Provides access to the APIs for managing the transformation context.</param>
        /// <param name="inputData">The parameter argument that is to be transformed.</param>
        /// <returns>The transformed object.</returns>
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null == inputData)
            {
                return null;
            }

            var converter = new ReinstallModesConverter();
            if (converter.CanConvertFrom(inputData.GetType()))
            {
                var mode = converter.ConvertFrom(inputData);
                return mode;
            }

            // Return the source data for other transformations in the chain.
            return inputData;
        }
    }
}
