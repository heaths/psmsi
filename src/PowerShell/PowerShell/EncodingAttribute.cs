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
    /// Transforms string and integer representations of encodings to an <see cref="System.Text.Encoding"/>-type parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class EncodingAttribute : ArgumentTransformationAttribute
    {
        /// <summary>
        /// Transforms a string or integer to an <see cref="System.Text.Encoding"/>.
        /// </summary>
        /// <param name="engineIntrinsics">Provides access to the APIs for managing the transformation context.</param>
        /// <param name="inputData">The parameter argument that is to be transformed.</param>
        /// <returns>The transformed object.</returns>
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null != inputData)
            {
                var converter = new EncodingConverter();
                if (converter.CanConvertFrom(inputData.GetType()))
                {
                    return converter.ConvertFrom(inputData);
                }
            }

            return inputData;
        }
    }
}
