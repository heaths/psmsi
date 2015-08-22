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
