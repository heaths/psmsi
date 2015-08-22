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
