// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="GetTableCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class GetTableCommandTests : CommandTestBase
    {
        [TestMethod]
        public void GetTableFromPath()
        {
            using (var p = TestRunspace.CreatePipeline("get-msitable example.msi -table File"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Deployment.WindowsInstaller.Record#File", item.TypeNames[0], "The first type name is incorrect.");

                string value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("F_Source", value, "The File property is incorrect.");
            }
        }

        [TestMethod]
        public void GetTableFromLiteralPath()
        {
            using (var p = TestRunspace.CreatePipeline("get-item example.msi | get-msitable -table File"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Deployment.WindowsInstaller.Record#File", item.TypeNames[0], "The first type name is incorrect.");

                string value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("F_Source", value, "The File property is incorrect.");
            }
        }

        [TestMethod]
        public void GetQueryFromPath()
        {
            using (var p = TestRunspace.CreatePipeline("get-msitable example.msi -query 'SELECT File, ComponentId FROM File, Component WHERE Component_ = Component'"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Deployment.WindowsInstaller.Record", item.TypeNames[0], "The first type name is incorrect.");

                string value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("F_Source", value, "The File property is incorrect.");

                value = item.GetPropertyValue<string>("ComponentId");
                Assert.AreEqual<string>("{49DD49AB-0AEE-48F9-BDED-CE2A1CDB8C16}", value, "The ComponentId property is incorrect.");
            }
        }

        [TestMethod]
        public void GetQueryFromLiteralPath()
        {
            using (var p = TestRunspace.CreatePipeline("get-item example.msi | get-msitable -query 'SELECT File, ComponentId FROM File, Component WHERE Component_ = Component'"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Deployment.WindowsInstaller.Record", item.TypeNames[0], "The first type name is incorrect.");

                string value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("F_Source", value, "The File property is incorrect.");

                value = item.GetPropertyValue<string>("ComponentId");
                Assert.AreEqual<string>("{49DD49AB-0AEE-48F9-BDED-CE2A1CDB8C16}", value, "The ComponentId property is incorrect.");
            }
        }

        [TestMethod]
        public void GetMissingTableFromPath()
        {
            using (var p = TestRunspace.CreatePipeline("get-msitable example.msi -table NonexistentTable"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null == output || 0 == output.Count, "Output is incorrect.");
                Assert.AreEqual<int>(1, p.Error.Count, "The error count is incorrect.");

                var obj = p.Error.Read() as PSObject;
                Assert.IsNotNull(obj, "The error stream is not correct.");

                var error = obj.BaseObject as ErrorRecord;
                Assert.IsNotNull(error, "The error record is incorrect.");
                Assert.IsTrue(error.Exception.Message.Contains("The table \"NonexistentTable\" was not found"), "The error message is incorrect.");
            }
        }
    }
}
