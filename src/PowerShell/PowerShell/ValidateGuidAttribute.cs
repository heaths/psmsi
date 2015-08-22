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
using System.Collections;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Validates that the argument or collection of arguments are all GUIDs in the
    /// format {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx} that Windows Installer APIs accept.
    /// </summary>
    /// <remarks>
    /// This does not extend ValidateEnumeratedArgumentsAttribute since that throws if the
    /// actual argument is null, which some cmdlets in this project allow.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ValidateGuidAttribute : ValidateArgumentsAttribute
    {
        /// <summary>
        /// Validates that the argument contains acceptable GUIDs if not null.
        /// </summary>
        /// <param name="arguments">The arguments to validate.</param>
        /// <param name="engineIntrinsics">Provides functionality from the engine.</param>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Let other validators check for null arguments.
            if (arguments != null)
            {
                // Validate one or all objects.
                IEnumerable e = LanguagePrimitives.GetEnumerable(arguments);
                if (e == null)
                {
                    ValidateGuidAttribute.ValidateElement(arguments);
                }
                else
                {
                    foreach (object obj in e)
                    {
                        ValidateGuidAttribute.ValidateElement(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Validates that the element is a string in the format {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}.
        /// </summary>
        /// <param name="element">The element to validate.</param>
        private static void ValidateElement(object element)
        {
            // Let other validators check for null elements.
            if (null != element)
            {
                if (!Microsoft.Tools.WindowsInstaller.Validate.IsGuid(element as string))
                {
                    throw new ValidationMetadataException(Properties.Resources.Error_InvalidGuid);
                }
            }
        }
    }
}
