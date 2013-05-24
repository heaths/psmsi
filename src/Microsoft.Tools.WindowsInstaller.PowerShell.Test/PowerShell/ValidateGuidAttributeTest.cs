// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="ValidateGuidAttribute"/> class.
    /// </summary>
    [TestClass]
    public sealed class ValidateGuidAttributeTest
    {
        private RunspaceConfiguration config;
        private TestContext context;

        /// <summary>
        /// Creates a new instance of the <see cref="ValidateGuidAttribute"/> class.
        /// </summary>
        public ValidateGuidAttributeTest()
        {
            this.config = null;
            this.context = null;
        }

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// Initializes the test classes.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.config = RunspaceConfiguration.Create();
            this.config.Cmdlets.Append(new CmdletConfigurationEntry("test-validateguidattribute", typeof(TestValidateGuidAttributeCommand), null));
        }

        /// <summary>
        /// Tests the <see cref="ValidateGuidAttribute.ValidateElement"/> method.
        /// </summary>
        [TestMethod]
        [Description("A test for ValidateGuidAttribute.ValidateElement")]
        public void ValidateElementTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(this.config))
            {
                rs.Open();

                // Test non-string input.
                using (Pipeline p = rs.CreatePipeline(@"test-validateguidattribute 1"))
                {
                    // Actual outer exception type is ParameterBindingValidationException.
                    TestProject.ExpectException(typeof(ParameterBindingException), typeof(ValidationMetadataException), delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Test non-GUID string input != 38 characters.
                using (Pipeline p = rs.CreatePipeline(@"test-validateguidattribute 'test'"))
                {
                    // Actual outer exception type is ParameterBindingValidationException.
                    TestProject.ExpectException(typeof(ParameterBindingException), typeof(ValidationMetadataException), delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Test non-GUID string input == 38 characters.
                using (Pipeline p = rs.CreatePipeline(@"test-validateguidattribute '{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}'"))
                {
                    // Actual outer exception type is ParameterBindingValidationException.
                    TestProject.ExpectException(typeof(ParameterBindingException), typeof(ValidationMetadataException), delegate()
                    {
                        Collection<PSObject> objs = p.Invoke();
                    });
                }

                // Test valid GUID string input.
                using (Pipeline p = rs.CreatePipeline(@"test-validateguidattribute '{01234567-89ab-cdef-0123-456789ABCDEF}'"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    // Validate count and output.
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>(@"{01234567-89ab-cdef-0123-456789ABCDEF}", objs[0].BaseObject as string);
                }
            }
        }

        [Cmdlet(VerbsDiagnostic.Test, "ValidateGuidAttribute")]
        private sealed class TestValidateGuidAttributeCommand : PSCmdlet
        {
            private object[] ids = null;

            [Parameter(Position = 0, Mandatory = true), ValidateGuid]
            public object[] Ids
            {
                get { return this.ids; }
                set { this.ids = value; }
            }

            protected override void EndProcessing()
            {
                foreach (object id in this.ids)
                {
                    this.WriteObject(id);
                }
            }
        }
    }
}
