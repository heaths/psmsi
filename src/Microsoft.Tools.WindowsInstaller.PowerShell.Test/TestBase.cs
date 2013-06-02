// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// A delegate that takes no parameters and returns no value.
    /// </summary>
    internal delegate void Action();

    /// <summary>
    /// Base class for all tests within the assembly.
    /// </summary>
    [TestClass]
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
        /// Initializes state before each test is run.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // Always make sure the DefaultRunspace is set before each test.
            Runspace.DefaultRunspace = TestRunspace;
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
                properties = new Dictionary<string, string>()
                {
                    { "CurrentSID", CurrentSID },
                    { "CurrentUsername", CurrentUsername },
                    { "TestDeploymentDirectory", this.TestContext.DeploymentDirectory },
                    { "TestRunDirectory", this.TestContext.TestResultsDirectory },
                };
            }

            var reg = new MockRegistry();
            reg.Import(path, properties);

            return reg;
        }

        /// <summary>
        /// Asserts that the expected exception type was caught.
        /// </summary>
        /// <typeparam name="T">The type of the parameter for the <paramref name="action"/>.</typeparam>
        /// <param name="exceptionType">The type of the exception to expect.</param>
        /// <param name="innerType">Optionally, the type of the inner exception to expect.</param>
        /// <param name="action">The action that should throw the exception.</param>
        internal static void ExpectException(Type exceptionType, Type innerType, Action action)
        {
            try
            {
                action.Invoke();

                // Should not have made it this far so throw.
                Assert.Fail(string.Format("No exception thrown; expected exception type {0}.", exceptionType.FullName));
            }
            catch (Exception ex)
            {
                // Check the exception type and, if given, the inner exception type.
                Assert.IsInstanceOfType(ex, exceptionType);
                if (innerType != null)
                {
                    Assert.IsInstanceOfType(ex.InnerException, innerType);
                }
            }
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
