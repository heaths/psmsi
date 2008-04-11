// Unit test class for the Location class.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="Location"/> support class.
    /// </summary>
    [TestClass]
    public class LocationTest
    {
        /// <summary>
        /// A cmdlet that provides context for the <see cref="Location"/> class.
        /// </summary>
        [Cmdlet(VerbsDiagnostic.Test, "Location")]
        private class TestLocationCommand : PSCmdlet
        {
            public enum TestCase
            {
                GetProviderQualifiedPath,
                GetOneResolvedProviderPathFromPSObject,
                AddPSPath
            }

            string[] paths;
            TestCase test;

            protected override void  EndProcessing()
            {
                switch (test)
                {
                    case TestCase.GetProviderQualifiedPath:
                        foreach (string path in paths)
                        {
                            // Without a provider-qualified path.
                            ProviderInfo provider = SessionState.Provider.GetOne("FileSystem");
                            string value = Location.GetProviderQualifiedPath(path, provider);
                            WriteObject(value);

                            // Now with a full provider-qualified path.
                            value = Location.GetProviderQualifiedPath(value, provider);
                            WriteObject(value);
                        }
                        break;

                    case TestCase.GetOneResolvedProviderPathFromPSObject:
                        foreach (string path in paths)
                        {
                            PSObject obj = PSObject.AsPSObject(new object());
                            string value = Location.GetOneResolvedProviderPathFromPSObject(obj, this);
                            WriteObject(value);

                            obj.Properties.Add(new PSNoteProperty("PSPath", path));
                            value = Location.GetOneResolvedProviderPathFromPSObject(obj, this);
                            WriteObject(value);
                        }
                        break;

                    case TestCase.AddPSPath:
                        foreach (string path in paths)
                        {
                            PSObject obj = PSObject.AsPSObject(new object());
                            Location.AddPSPath(path, obj, this);
                            WriteObject(obj);
                        }
                        break;
                }
            }

            /// <summary>
            /// Gets or sets the path or paths to input items.
            /// </summary>
            [Parameter(Mandatory = true)]
            public string[] Path
            {
                get { return paths; }
                set { paths = value; }
            }

            /// <summary>
            /// Gets or sets the test case to execute.
            /// </summary>
            [Parameter(Mandatory = true)]
            public TestCase Test
            {
                get { return test; }
                set { test = value; }
            }
        }

        private TestContext testContext;
        private RunspaceConfiguration config;
        private string examplePath;
        private string qualifiedPath;

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            config = RunspaceConfiguration.Create();
            config.Cmdlets.Append(new CmdletConfigurationEntry("Test-Location", typeof(TestLocationCommand), null));

            // In some cases a real path is required, so calculate the test path to an existing file.
            examplePath = Path.Combine(testContext.TestDeploymentDir, @"example.txt");
            qualifiedPath = string.Format(@"Microsoft.PowerShell.Core\FileSystem::{0}", examplePath);
        }

        /// <summary>
        /// A test for <see cref="Location.GetProviderQualifiedPath"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for Location.GetProviderQualifiedPath")]
        [DeploymentItem(@"data\example.txt")]
        public void GetProviderQualifiedPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                string command = string.Format(@"test-location -path ""{0}"" -test ""GetProviderQualifiedPath""", examplePath);

                using (Pipeline p = rs.CreatePipeline(command))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(2, objs.Count);

                    foreach (PSObject obj in objs)
                    {
                        Assert.IsInstanceOfType(obj.BaseObject, typeof(string));

                        string path = (string)obj.BaseObject;
                        Assert.AreEqual<string>(qualifiedPath, path);
                    }
                }
            }
        }

        /// <summary>
        /// A test for <see cref="Location.GetOneResolvedProviderPathFromPSObject"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for Location.GetOneResolvedProviderPathFromPSObject")]
        [DeploymentItem(@"data\example.txt")]
        public void GetOneResolvedProviderPathFromPSObjectTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                string command = string.Format(@"test-location -path ""{0}"" -test ""GetOneResolvedProviderPathFromPSObject""", qualifiedPath);

                using (Pipeline p = rs.CreatePipeline(command))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(2, objs.Count);

                    PSObject obj = objs[0];
                    Assert.IsNull(obj);

                    obj = objs[1];
                    Assert.IsInstanceOfType(obj.BaseObject, typeof(string));

                    string path = (string)obj.BaseObject;
                    Assert.AreEqual<string>(examplePath, path);
                }
            }
        }

        /// <summary>
        /// A test for <see cref="Location.AddPSPath"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for Location.AddPSPath")]
        [DeploymentItem(@"data\example.txt")]
        public void AddPSPathTest()
        {
            using (Runspace rs = RunspaceFactory.CreateRunspace(config))
            {
                rs.Open();

                string command = string.Format(@"test-location -path ""{0}"" -test ""AddPSPath""", examplePath);
                string qualifiedParentPath = string.Format(@"Microsoft.PowerShell.Core\FileSystem::{0}", Path.GetDirectoryName(examplePath));
                string qualifiedChildName = Path.GetFileName(examplePath);

                using (Pipeline p = rs.CreatePipeline(command))
                {
                    Collection<PSObject> objs = p.Invoke();
                    Assert.AreEqual<int>(1, objs.Count);

                    PSObject obj = objs[0];
                    PSPropertyInfo PSPath = obj.Properties["PSPath"];
                    PSPropertyInfo PSParentPath = obj.Properties["PSParentPath"];
                    PSPropertyInfo PSChildName = obj.Properties["PSChildName"];

                    Assert.IsNotNull(PSPath);
                    Assert.IsNotNull(PSParentPath);
                    Assert.IsNotNull(PSChildName);

                    Assert.AreEqual<string>(qualifiedPath, (string)PSPath.Value);
                    Assert.AreEqual<string>(qualifiedParentPath, (string)PSParentPath.Value);
                    Assert.AreEqual<string>(qualifiedChildName, (string)PSChildName.Value);
                }
            }
        }
    }
}
