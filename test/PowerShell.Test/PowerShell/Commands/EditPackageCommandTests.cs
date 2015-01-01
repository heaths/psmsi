// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
