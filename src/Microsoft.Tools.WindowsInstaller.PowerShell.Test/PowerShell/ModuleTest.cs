// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="Module"/> class.
    /// </summary>
    [TestClass]
    public sealed class ModuleTest
    {
        private TestContext context;

        /// <summary>
        /// Creates a new instance of the <see cref="ModuleTest"/> class.
        /// </summary>
        public ModuleTest()
        {
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

        [TestMethod]
        [Description("A test for Module.Use")]
        public void UseTest()
        {
            using (MockRegistry reg = new MockRegistry())
            {
                reg.Import("registry.xml");

                // Set initial usage data.
                Module.Use();

                // Initial use should be empty (null).
                FeatureInstallation feature = new FeatureInstallation("Module", "{B4EA7821-1AC1-41B5-8021-A2FC77D1B7B7}");
                Assert.IsNotNull(feature.Usage);
                Assert.AreEqual<int>(1, feature.Usage.UseCount);
            }
        }
    }
}
