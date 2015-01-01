// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="ValidateGuidAttribute"/> class.
    /// </summary>
    [TestClass]
    public sealed class ValidateGuidAttributeTest : TestBase
    {
        [TestMethod]
        public void ValidateElementTest()
        {
            var script = string.Format(@"&{{[CmdletBinding()]param([Parameter(Position=0)][{0}()]$Guid)process{{$Guid}}}} ", typeof(ValidateGuidAttribute).FullName);

            // Test non-string input.
            using (var p = CreatePipeline(script + "1"))
            {
                // Actual outer exception type is ParameterBindingValidationException.
                ExceptionAssert.Throws<ParameterBindingException, ValidationMetadataException>(() =>
                {
                    p.Invoke();
                });
            }

            // Test non-GUID string input != 38 characters.
            using (var p = CreatePipeline(script + "'test'"))
            {
                // Actual outer exception type is ParameterBindingValidationException.
                ExceptionAssert.Throws<ParameterBindingException, ValidationMetadataException>(() =>
                {
                    p.Invoke();
                });
            }

            // Test non-GUID string input == 38 characters.
            using (var p = CreatePipeline(script + "'{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}'"))
            {
                // Actual outer exception type is ParameterBindingValidationException.
                ExceptionAssert.Throws<ParameterBindingException, ValidationMetadataException>(() =>
                {
                    p.Invoke();
                });
            }

            // Test valid GUID string input.
            using (var p = CreatePipeline(script + "'{01234567-89ab-cdef-0123-456789ABCDEF}'"))
            {
                var objs = p.Invoke();

                // Validate count and output.
                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<string>(@"{01234567-89ab-cdef-0123-456789ABCDEF}", objs[0].BaseObject as string);
            }
        }
    }
}
