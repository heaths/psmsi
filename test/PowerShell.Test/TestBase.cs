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

using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Base class for all tests within the assembly.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"MSI.psd1")]
    [DeploymentItem(@"MSI.psm1")]
    [DeploymentItem(@"MSI.types.ps1xml")]
    [DeploymentItem(@"MSI.formats.ps1xml")]
    [DeploymentItem(@"Data\")]
    public abstract class TestBase
    {
        #region Runspace Configuration

        /// <summary>
        /// Gets the <see cref="Runspace"/> to use for all derived tests.
        /// </summary>
        protected static Runspace TestRunspace { get; private set; }

        /// <summary>
        /// Initializes the test assembly and loads the PowerShell module.
        /// </summary>
        /// <param name="context">Context for tests within this assembly.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            var state = InitialSessionState.CreateDefault();

            // Allow all scripts to run while testing.
            state.AuthorizationManager = null;

            // Add global test variables to the global scope.
            state.Variables.Add(new SessionStateVariableEntry("TestDeploymentDirectory", context.DeploymentDirectory, "Gets the directory for files deployed for the test run.", ScopedItemOptions.AllScope | ScopedItemOptions.Constant));
            state.Variables.Add(new SessionStateVariableEntry("TestRunDirectory", context.TestRunDirectory, "Gets the top-level directory for the test run that contains deployed files and result files.", ScopedItemOptions.AllScope | ScopedItemOptions.Constant));

            // Import the module to test and our own test module.
            state.ImportPSModule(new string[]
            {
                Path.Combine(context.TestDeploymentDir, "MSI.psd1"),
                Path.Combine(context.TestDeploymentDir, "Test.psm1")
            });

            // Create and configure the runspaces.
            TestBase.TestRunspace = RunspaceFactory.CreateRunspace(state);
            Runspace.DefaultRunspace = TestBase.TestRunspace;

            // Open the runspace so we can invoke pipelines.
            TestBase.TestRunspace.Open();
        }

        /// <summary>
        /// Cleans up the test configuration.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (null != TestBase.TestRunspace)
            {
                using (TestBase.TestRunspace)
                {
                    TestBase.TestRunspace.Close();
                }
            }
        }

        /// <summary>
        /// Initializes state before each test is run.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // Always make sure the DefaultRunspace is set before each test.
            Runspace.DefaultRunspace = TestRunspace;
        }
        #endregion

        #region System Information

        /// <summary>
        /// Gets the SID in SDDL form for the current user.
        /// </summary>
        protected static string CurrentSID
        {
            get
            {
                using (var id = WindowsIdentity.GetCurrent())
                {
                    var sid = id.User;
                    return sid.Value;
                }
            }
        }

        /// <summary>
        /// Gets the username for the current user.
        /// </summary>
        protected static string CurrentUsername
        {
            get
            {
                using (var id = WindowsIdentity.GetCurrent())
                {
                    return id.Name;
                }
            }
        }

        /// <summary>
        /// Gets the default replacement properties for the mock registry file.
        /// </summary>
        protected Dictionary<string, string> DefaultRegistryProperties
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "CurrentSID", CurrentSID },
                    { "CurrentUsername", CurrentUsername },
                    { "TestDeploymentDirectory", this.TestContext.DeploymentDirectory },
                    { "TestRunDirectory", this.TestContext.TestResultsDirectory },
                };
            }
        }

        #endregion

        #region Test Execution

        /// <summary>
        /// Gets or sets the <see cref="TestContext"/> for all derived tests.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Creates a new <see cref="MockRegistry"/> and loads either the default or specified registry data file and properties.
        /// </summary>
        /// <param name="path">Optional path to the registry data file. If null, "Regstry.xml" is used from the <see cref="TestContext.DesloymentDirectory"/>.</param>
        /// <param name="properties">Additional properties to populate in the specified registry data file.</param>
        /// <returns>A new <see cref="MockRegistry"/> that must be disposed.</returns>
        internal MockRegistry OverrideRegistry(string path = null, Dictionary<string, string> properties = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(this.TestContext.DeploymentDirectory, "Registry.xml");
            }

            if (null == properties)
            {
                properties = this.DefaultRegistryProperties;
            }

            var reg = new MockRegistry();
            reg.Import(path, properties);

            return reg;
        }

        /// <summary>
        /// Creates a new pipeline with the given command.
        /// </summary>
        /// <param name="command">The command to invoke.</param>
        /// <returns>A new <see cref="Pipeline"/> that must be disposed.</returns>
        protected static Pipeline CreatePipeline(string command)
        {
            return TestRunspace.CreatePipeline(command);
        }
        #endregion
    }
}
