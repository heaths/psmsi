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
