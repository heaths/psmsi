// Validates that the argument or collection of arguments are all GUIDs.
//
// Created: Sun, 08 Mar 2009 22:02:15 GMT
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Management.Automation;

namespace Microsoft.WindowsInstaller.PowerShell
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
        // Define and compile the regular expression to validate GUIDs in a format Windows Installer understands.
        private static readonly Regex re = new Regex(@"\{[A-F0-9]{8}-([A-F0-9]{4}-){3}[A-F0-9]{12}\}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Creates a new instance of the <see cref="ValidateGuidAttribute"/> class.
        /// </summary>
        public ValidateGuidAttribute()
        {
        }
        
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
            if (element != null)
            {
                string input = element as string;

                // Validate simple checks before performing a more exhaustive regex match.
                if (input == null || input.Length != 38 || !re.IsMatch(input))
                {
                    throw new ValidationMetadataException(Properties.Resources.Error_InvalidGuid);
                }
            }
        }
    }
}