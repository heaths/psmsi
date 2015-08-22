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
    /// Attribute to Validate that a variable name is valid with or without optional append character "+".
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ValidateVariableNameAttribute : ValidateArgumentsAttribute
    {
        /// <summary>
        /// The character that signifies the variable value should be appended to an existing variable.
        /// </summary>
        internal static readonly string AppendPrefix = "+";

        /// <summary>
        /// Validates that a variable name is valid with or without optional append character "+".
        /// </summary>
        /// <param name="arguments">The arguments to validate.</param>
        /// <returns>True if the variable name is valid; otherwise, false.</returns>
        internal bool Validate(object arguments)
        {
            var name = arguments as string;
            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith(ValidateVariableNameAttribute.AppendPrefix))
                {
                    name = name.Substring(1);
                }
            }

            return !string.IsNullOrEmpty(name);
        }

        /// <summary>
        /// Validates that a variable name is valid with or without optional append character "+".
        /// </summary>
        /// <param name="arguments">The arguments to validate.</param>
        /// <param name="engineIntrinsics"><see cref="EngineIntrinsics"/> for additional information.</param>
        /// <exception cref="ValidationMetadataException">The variable name is invalid.</exception>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (!this.Validate(arguments))
            {
                throw new ValidationMetadataException(Properties.Resources.Error_InvalidVariableName);
            }
        }
    }
}
