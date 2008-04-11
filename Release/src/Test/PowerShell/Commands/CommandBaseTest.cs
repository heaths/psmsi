// Unit test class for the get-msiproductinfo cmdlet.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional test for <see cref="CommandBaseTest"/>.
    /// </summary>
    [TestClass]
    public class CommandBaseTest
    {
        RunspaceConfiguration config;

        [Cmdlet(VerbsDiagnostic.Test, "MSICommand")]
        private class TestCommand : CommandBase
        {
            TestCase test;

            public enum TestCase
            {
                HandleAccessDenied,
                HandleBadConfiguration
            }

            [Parameter(Mandatory = true)]
            public TestCase Test
            {
                get { return test; }
                set { test = value; }
            }

            protected override void EndProcessing()
            {
                switch (test)
                {
                    case TestCase.HandleAccessDenied:
                    {
                        int ret = HandleCommonErrors(NativeMethods.ERROR_ACCESS_DENIED);
                        WriteObject(ret);
                        break;
                    }

                    case TestCase.HandleBadConfiguration:
                    {
                        int ret = HandleCommonErrors(NativeMethods.ERROR_BAD_CONFIGURATION);
                        WriteObject(ret);
                        break;
                    }
                }
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            config = RunspaceConfiguration.Create();
            config.Cmdlets.Append(new CmdletConfigurationEntry("Test-MSICommand", typeof(TestCommand), null));
        }

        /// <summary>
        /// A test for <see cref="CommandBase.HandleCommonErrors"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for CommandBase.HandleCommonErrors")]
        public void HandleCommonErrorsTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                // Test ERROR_ACCESS_DENIED
                using (Pipeline p = rs.CreatePipeline(@"test-msicommand -test ""HandleAccessDenied"""))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<int>(1, p.Error.Count);

                    // Check the return code
                    PSObject obj = objs[0];
                    Assert.IsInstanceOfType(obj.BaseObject, typeof(int));
                    Assert.AreEqual<int>(NativeMethods.ERROR_NO_MORE_ITEMS, (int)obj.BaseObject);

                    // Check the error data
                    Collection<object> errors = p.Error.ReadToEnd();
                    Assert.AreEqual<int>(1, errors.Count);

                    obj = errors[0] as PSObject;
                    Assert.IsNotNull(obj);

                    ErrorRecord error = obj.BaseObject as ErrorRecord;
                    Assert.AreEqual<string>("AccessDenied,Microsoft.Windows.Installer.PowerShell.Commands.CommandBaseTest+TestCommand", error.FullyQualifiedErrorId);
                    Assert.AreEqual<ErrorCategory>(ErrorCategory.PermissionDenied, error.CategoryInfo.Category);
                    Assert.AreEqual<string>("Access denied.", error.ErrorDetails.Message);
                    Assert.AreEqual<string>("Run the expression again in an elevated process.", error.ErrorDetails.RecommendedAction);
                }

                // Test ERROR_BAD_CONFIGURATION
                using (Pipeline p = rs.CreatePipeline(@"test-msicommand -test ""HandleBadConfiguration"""))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<int>(1, p.Error.Count);

                    // Check the return code
                    PSObject obj = objs[0];
                    Assert.IsInstanceOfType(obj.BaseObject, typeof(int));
                    Assert.AreEqual<int>(NativeMethods.ERROR_NO_MORE_ITEMS, (int)obj.BaseObject);

                    // Check the error data
                    Collection<object> errors = p.Error.ReadToEnd();
                    Assert.AreEqual<int>(1, errors.Count);

                    obj = errors[0] as PSObject;
                    Assert.IsNotNull(obj);

                    ErrorRecord error = obj.BaseObject as ErrorRecord;
                    Assert.AreEqual<string>("BadConfiguration,Microsoft.Windows.Installer.PowerShell.Commands.CommandBaseTest+TestCommand", error.FullyQualifiedErrorId);
                    Assert.IsNull(error.ErrorDetails);
                }
            }
        }
    }
}
