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
using System.Collections;
using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="EditPackageCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class EditPackageCommandTests : TestBase
    {
        [TestMethod]
        public void EditPackageInOrca()
        {
            using (var p = CreatePipeline(@"edit-msipackage example.msi -wv Warnings"))
            {
                using (OverrideRegistry())
                {
                    var items = p.Invoke();

                    Assert.AreEqual<int>(0, items.Count, "Output was not expected.");
                    Assert.AreEqual<int>(0, p.Error.Count, "Errors were not expected.");

                    var warnings = p.Runspace.SessionStateProxy.GetVariable("Warnings") as ICollection;
                    Assert.IsNotNull(warnings, "Expected a Warnings variable.");
                    Assert.AreEqual<int>(0, warnings.Count, "Warnings were not expected.");
                }
            }
        }

        [TestMethod]
        public void EditPackageWithVerb()
        {
            using (var p = CreatePipeline(@"edit-msipackage example.msp -wv Warnings"))
            {
                var path = Path.Combine(this.TestContext.DeploymentDirectory, "NoOrca.xml");
                using (OverrideRegistry(path))
                {
                    p.Invoke();

                    Assert.AreEqual<int>(0, p.Output.Count, "Output was not expected.");
                    Assert.AreEqual<int>(0, p.Error.Count, "Errors were not expected.");

                    var warnings = p.Runspace.SessionStateProxy.GetVariable("Warnings") as ICollection;
                    Assert.IsNotNull(warnings, "Expected a Warnings variable.");
                    Assert.AreEqual<int>(1, warnings.Count, "Expected a warning.");
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletInvocationException))]
        public void EditPackageUnavailable()
        {
            using (var p = CreatePipeline(@"edit-msipackage example.msi -wv Warnings"))
            {
                var path = Path.Combine(this.TestContext.DeploymentDirectory, "NoEditVerb.xml");
                using (OverrideRegistry(path))
                {
                    // Should throw terminating exception.
                    p.Invoke();
                }
            }
        }

        [TestMethod]
        public void EditNonPackage()
        {
            using (var p = CreatePipeline(@"edit-msipackage example.txt -wv Warnings"))
            {
                var path = Path.Combine(this.TestContext.DeploymentDirectory, "NoEditVerb.xml");
                using (OverrideRegistry(path))
                {
                    var items = p.Invoke();

                    Assert.AreEqual<int>(0, items.Count, "Output was not expected.");
                    Assert.AreEqual<int>(1, p.Error.Count, "Expected an error.");

                    var warnings = p.Runspace.SessionStateProxy.GetVariable("Warnings") as ICollection;
                    Assert.IsNotNull(warnings, "Expected a Warnings variable.");
                    Assert.AreEqual<int>(1, warnings.Count, "Expected a warning.");
                }
            }
        }
    }
}
