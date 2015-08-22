// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Supports conversion between the short form REINSTALLMODE and the <see cref="Microsoft.Deployment.WindowsInstaller.ReinstallModes"/> enumeration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ReinstallModeAttribute : ArgumentTransformationAttribute
    {
        /// <summary>
        /// Transforms a string in the short form of REINSTALLMODE to a <see cref="Microsoft.Deployment.WindowsInstaller.ReinstallModes"/> enumeration.
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
