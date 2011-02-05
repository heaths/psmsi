// Base class for testing cmdlets.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The base class for command tests.
    /// </summary>
    public abstract class CommandTestBase
    {
        private TestContext context;

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// Gets the <see cref="Runspace"/> for all the cmdlet tests.
        /// </summary>
        protected Runspace TestRunspace
        {
            get
            {
                if (TestProject.TestRunspace == null)
                {
                    Assert.Fail("The test Runspace has not been initialized.");
                }

                return TestProject.TestRunspace;
            }
        }
    }
}
