// Unit test class for the PathConverter class.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="PathConverter"/> class.
    /// </summary>
    [TestClass]
    public sealed class PathConverterTest
    {
        private RunspaceConfiguration config;
        private TestContext context;

        /// <summary>
        /// Creates a new instance of the <see cref="PathConverterTest"/> class.
        /// </summary>
        public PathConverterTest()
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
            this.config.Cmdlets.Append(new CmdletConfigurationEntry("test-pathconverter", typeof(TestPathConverterCommand), null));
        }

        /// <summary>
        /// Tests the <see cref="PathConverter.ToPSPath"/> method.
        /// </summary>
        [TestMethod]
        [Description("A test for PathConverter.ToPSPath")]
        public void ToPSPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(this.config))
            {
                rs.Open();

                // Test null session.
                TestProject.ExpectException(typeof(ArgumentNullException), null, delegate()
                {
                    PathConverter_Accessor.ToPSPath(null, null);
                });

                // Test using a provider path.
                using (Pipeline p = rs.CreatePipeline(@"test-pathconverter $null -topspath"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.IsNull(objs[0]);
                }

                // Test using a provider-qualified path.
                string pspath = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
                string expression = string.Format(@"test-pathconverter ""{0}"" -topspath", pspath);
                using (Pipeline p = rs.CreatePipeline(expression))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>(pspath, objs[0].BaseObject as string);
                }

                // Test using a provider path with a drive.
                expression = @"test-pathconverter ""C:\foo"" -topspath";
                using (Pipeline p = rs.CreatePipeline(expression))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>(pspath, objs[0].BaseObject as string);
                }

                // TODO: Test using a provider path without a drive. Currently fails because Combine does not consider starting backslash.
                //expression = @"test-pathconverter ""\foo"" -topspath";
                //using (Pipeline p = rs.CreatePipeline(expression))
                //{
                //    Collection<PSObject> objs = p.Invoke();

                //    Assert.AreEqual<int>(1, objs.Count);
                //    Assert.AreEqual<string>(pspath, objs[0].BaseObject as string);
                //}
            }
        }

        /// <summary>
        /// Tests the <see cref="PathConverter.ToProviderPath"/> method.
        /// </summary>
        [TestMethod]
        [Description("A test for PathConverter.ToProviderPath")]
        public void ToProviderPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(this.config))
            {
                rs.Open();

                // Test null session.
                TestProject.ExpectException(typeof(ArgumentNullException), null, delegate()
                {
                    PathConverter_Accessor.ToProviderPath(null, null);
                });
                
                // Test null path.
                using (Pipeline p = rs.CreatePipeline(@"test-pathconverter $null -toproviderpath"))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.IsNull(objs[0]);
                }

                // Test using a PSPath.
                string pspath = @"Microsoft.PowerShell.Core\FileSystem::C:\foo";
                string expression = string.Format(@"test-pathconverter ""{0}"" -toproviderpath", pspath);
                using (Pipeline p = rs.CreatePipeline(expression))
                {
                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>(@"C:\foo", objs[0].BaseObject as string);
                }
            }
        }

        [Cmdlet(VerbsDiagnostic.Test, "PathConverter", DefaultParameterSetName = "ToPSPath")]
        private sealed class TestPathConverterCommand : PSCmdlet
        {
            private string path = null;

            [Parameter(ParameterSetName = "ToPSPath", Position = 0)]
            [Parameter(ParameterSetName = "ToProviderPath", Position = 0)]
            public string Path
            {
                get { return this.path; }
                set { this.path = value; }
            }

            [Parameter(ParameterSetName = "ToPSPath", Mandatory = true)]
            public SwitchParameter ToPSPath
            {
                get { return true; }
                set { ; }
            }

            [Parameter(ParameterSetName = "ToProviderPath", Mandatory = true)]
            public SwitchParameter ToProviderPath
            {
                get { return true; }
                set { ; }
            }

            protected override void EndProcessing()
            {
                if (this.ParameterSetName == "ToPSPath")
                {
                    this.WriteObject(PathConverter_Accessor.ToPSPath(this.SessionState, this.path));
                }
                else if (this.ParameterSetName == "ToProviderPath")
                {
                    this.WriteObject(PathConverter_Accessor.ToProviderPath(this.SessionState, this.path));
                }
            }
        }
    }
}
