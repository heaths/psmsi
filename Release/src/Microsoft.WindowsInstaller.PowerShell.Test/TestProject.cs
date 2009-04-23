// Suppoort methods and properties for the test project.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller
{
    /// <summary>
    /// Support methods and properties for the test project.
    /// </summary>
    [TestClass]
    internal static class TestProject
    {
        private static Runspace testRunspace;

        /// <summary>
        /// A delegate that takes no parameters and returns no value.
        /// </summary>
        internal delegate void Action();

        /// <summary>
        /// Gets the SID in SDDL form for the current user.
        /// </summary>
        internal static string CurrentSID
        {
            get
            {
                using (WindowsIdentity id = WindowsIdentity.GetCurrent())
                {
                    SecurityIdentifier sid = id.User;
                    return sid.Value;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Runspace"/> for all the cmdlet tests.
        /// </summary>
        internal static Runspace TestRunspace
        {
            get { return TestProject.testRunspace; }
        }

        /// <summary>
        /// Gets the username for the current user.
        /// </summary>
        internal static string CurrentUsername
        {
            get
            {
                using (WindowsIdentity id = WindowsIdentity.GetCurrent())
                {
                    return id.Name;
                }
            }
        }

        /// <summary>
        /// Initializes the test assembly and loads the PowerShell module.
        /// </summary>
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // Create the initial state with the module specified.
            InitialSessionState state = InitialSessionState.CreateDefault();
            state.ImportPSModule(new string[] { Path.Combine(context.TestDeploymentDir, "WindowsInstaller.psd1") });

            // Create and configure the runspace.
            TestProject.testRunspace = RunspaceFactory.CreateRunspace(state);
            TestProject.testRunspace.ThreadOptions = PSThreadOptions.UseNewThread;

            TestProject.testRunspace.Open();
        }

        /// <summary>
        /// Cleans up the test assembly.
        /// </summary>
        [AssemblyCleanup]
        public static void Cleanup()
        {
            // Close and dispose of the pool.
            using (TestProject.testRunspace)
            {
                TestProject.testRunspace.Close();
            }
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
    }
}
