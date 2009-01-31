// Base class for testing cmdlets.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsInstaller;

namespace Microsoft.WindowsInstaller.PowerShell.Commands
{
    public abstract class CmdletTestBase
    {
        private TestContext context;
        private RunspaceConfiguration config;

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// Initializes the test class. Overrides should call this method fist
        /// to set up the runspace configuration.
        /// </summary>
        public virtual void Initialize()
        {
            this.config = RunspaceConfiguration.Create();
        }

        /// <summary>
        /// Gets the <see cref="RunspaceConfiguration"/> for the cmdlet test class.
        /// </summary>
        protected RunspaceConfiguration Configuration
        {
            get { return this.config; }
        }

        /// <summary>
        /// Add cmdlet types for use in the cmdlet test class.
        /// </summary>
        /// <param name="cmdlets">The types of cmdlets to use in the cmdlet test class.</param>
        protected void AddCmdlet(params Type[] cmdlets)
        {
            foreach (Type cmdlet in cmdlets)
            {
                // Get verb-noun pairs from the cmdlet type.
                CmdletAttribute[] attribs = (CmdletAttribute[])cmdlet.GetCustomAttributes(typeof(CmdletAttribute), false);
                if (attribs != null & attribs.Length > 0)
                {
                    // Just add the first attribute (does not allow multiples).
                    string name = string.Concat(attribs[0].VerbName, "-", attribs[0].NounName);
                    config.Cmdlets.Append(new CmdletConfigurationEntry(name, cmdlet, "Microsoft.WindowsInstaller.PowerShell.dll-Help.xml"));
                }
            }
        }
    }
}
