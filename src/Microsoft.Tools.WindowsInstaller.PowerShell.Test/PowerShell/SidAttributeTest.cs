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
    /// Unit tests for the <see cref="SidAttribute"/> class.
    /// </summary>
    [TestClass]
    public sealed class SidAttributeTest
    {
        private RunspaceConfiguration config;
        private TestContext context;

        /// <summary>
        /// Creates a new instance of the <see cref="SidAttributeTest"/> class.
        /// </summary>
        public SidAttributeTest()
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
            this.config.Cmdlets.Append(new CmdletConfigurationEntry("test-sidattribute", typeof(TestSidAttributeCommand), null));
        }

        /// <summary>
        /// Tests the <see cref="SidAttribute.Transform"/> method.
        /// </summary>
        [TestMethod]
        [Description("A test for SidAttribute.Transform")]
        public void TransformTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(this.config))
            {
                rs.Open();

                // Test null input.
                using (Pipeline p = rs.CreatePipeline(@"test-sidattribute $null"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.IsNull(objs[0]);
                }

                // Test another object.
                using (Pipeline p = rs.CreatePipeline(@"test-sidattribute 0"))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);

                    // Even though the SidAttribute returned an int, PowerShell still converts it after.
                    Assert.AreEqual<string>("0", objs[0].BaseObject as string);
                }

                // Test a string.
                string expression = string.Format(@"test-sidattribute ""{0}""", TestProject.CurrentUsername);
                using (Pipeline p = rs.CreatePipeline(expression))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>(TestProject.CurrentSID, (string)objs[0].BaseObject);
                }
            }
        }

        /// <summary>
        /// Tests the <see cref="SidAttribute.TryParseUsername"/> method.
        /// </summary>
        [TestMethod]
        [Description("A test for SidAttribute.TryParseUsername")]
        public void TryParseUsernameTest()
        {
            string param = null;

            // Test a string without backslashes.
            Assert.IsFalse(SidAttribute.TryParseUsername(@"foo", out param));
            Assert.IsNull(param);

            // Test a string with backslashes but not a valid username.
            Assert.IsFalse(SidAttribute.TryParseUsername(@"foo\bar\baz", out param));
            Assert.IsNull(param);

            // Test a valid username.
            Assert.IsTrue(SidAttribute.TryParseUsername(TestProject.CurrentUsername, out param));
            Assert.AreEqual<string>(TestProject.CurrentSID, param);
        }

        [Cmdlet(VerbsDiagnostic.Test, "SidAttribute")]
        private sealed class TestSidAttributeCommand : PSCmdlet
        {
            private string userSid = null;

            [Parameter(Position = 0), Sid]
            public string UserSid
            {
                get { return this.userSid; }
                set { this.userSid = value; }
            }

            protected override void EndProcessing()
            {
                this.WriteObject(this.userSid);
            }
        }
    }
}
