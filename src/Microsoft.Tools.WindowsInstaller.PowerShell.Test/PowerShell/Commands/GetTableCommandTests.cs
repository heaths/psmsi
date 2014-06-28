// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="GetTableCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class GetTableCommandTests : TestBase
    {
        [TestMethod]
        public void GetTableFromPath()
        {
            using (var p = CreatePipeline("get-msitable example.msi -table File"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#File", item.TypeNames[0], "The first type name is incorrect.");

                string value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("product.wxs", value, "The File property is incorrect.");
            }
        }

        [TestMethod]
        public void GetTableFromLiteralPath()
        {
            using (var p = CreatePipeline("get-item example.msi | get-msitable -table File"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#File", item.TypeNames[0], "The first type name is incorrect.");

                string value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("product.wxs", value, "The File property is incorrect.");
            }
        }

        [TestMethod]
        public void GetQueryFromPath()
        {
            var query = "SELECT File, ComponentId, File.Attributes FROM File, Component WHERE Component_ = Component";
            using (var p = CreatePipeline(string.Format("get-msitable example.msi -query '{0}'", query)))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record", item.TypeNames[0], "The first type name is incorrect.");

                var value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("product.wxs", value, "The File property is incorrect.");

                value = item.GetPropertyValue<string>("ComponentId");
                Assert.AreEqual<string>("{B88B6441-D16B-4308-B03A-A4BBC0F8F022}", value, "The ComponentId property is incorrect.");

                var attributes = item.GetPropertyValue<int>("File.Attributes");
                Assert.AreEqual<int>(512, attributes, "The File.Attributes property is incorrect.");

                // Check for additional attached properties.
                value = item.GetPropertyValue<string>("MSIPath");
                var expectedPath = p.Runspace.SessionStateProxy.Path.GetUnresolvedProviderPathFromPSPath("example.msi");
                Assert.AreEqual(expectedPath, value, true, "The MSIPath property is incorrect.");

                value = item.GetPropertyValue<string>("MSIQuery");
                Assert.AreEqual(query, value, true, "The MSIQuery property is incorrect.");
            }
        }

        [TestMethod]
        public void GetQueryFromLiteralPath()
        {
            using (var p = CreatePipeline("get-item example.msi | get-msitable -query 'SELECT File, ComponentId, File.Attributes FROM File, Component WHERE Component_ = Component'"))
            {
                var output = p.Invoke();

                Assert.IsNotNull(output, "No output was returned.");
                Assert.AreEqual<int>(1, output.Count, "The output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record", item.TypeNames[0], "The first type name is incorrect.");

                var value = item.GetPropertyValue<string>("File");
                Assert.AreEqual<string>("product.wxs", value, "The File property is incorrect.");

                value = item.GetPropertyValue<string>("ComponentId");
                Assert.AreEqual<string>("{B88B6441-D16B-4308-B03A-A4BBC0F8F022}", value, "The ComponentId property is incorrect.");

                var attributes = item.GetPropertyValue<int>("File.Attributes");
                Assert.AreEqual<int>(512, attributes, "The File.Attributes property is incorrect.");
            }
        }

        [TestMethod]
        public void GetMissingTableFromPath()
        {
            using (var p = CreatePipeline("get-msitable example.msi -table NonexistentTable"))
            {
                var output = p.Invoke();
                Assert.AreEqual<long>(0, output.Count());
                Assert.AreEqual<int>(1, p.Error.Count);

                var obj = p.Error.Peek() as PSObject;
                Assert.IsNotNull(obj);

                var error = obj.BaseObject as ErrorRecord;
                Assert.IsNotNull(error);
                StringAssert.StartsWith(error.Exception.Message, @"The table ""NonexistentTable"" was not found");
            }
        }

        [TestMethod]
        public void GetRegistryKeyPathComponents()
        {
            using (var p = CreatePipeline(@"get-msitable example.msi -table Component | where { $_.Attributes.HasRegistryKeyPath }"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null != output && 1 == output.Count, "Output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#Component", item.TypeNames[0], "The first type name is incorrect.");

                var component = item.GetPropertyValue<string>("Component");
                Assert.AreEqual<string>("Registry", component, "The Component property is incorrect.");

                var attributes = item.GetPropertyValue<int>("Attributes");
                Assert.AreEqual<int>(4, attributes, "The Attributes property is incorrect.");
            }
        }

        [TestMethod]
        public void GetPatchedRegistryTable()
        {
            using (var p = CreatePipeline(@"get-msitable example.msi -table Registry -patch example.msp"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null != output && 1 == output.Count);

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#Registry", item.TypeNames[0]);

                // Work around a possible bug in PowerShell where adapted property values are cached.
                var record = item.BaseObject as Record;
                var value = (string)record.Data[record.Columns["Value"].Index];
                Assert.AreEqual<string>("1.0.1", value);
                Assert.AreEqual<RowOperation>(RowOperation.Modify, item.GetPropertyValue<RowOperation>("MSIOperation"));
            }

            // Make sure the record data does not cache the patched value.
            using (var p = CreatePipeline(@"get-msitable example.msi -table Registry"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null != output && 1 == output.Count);

                var item = output[0];

                // Work around a possible bug in PowerShell where adapted property values are cached.
                // This does not reproduce when running in powershell.exe.
                var record = item.BaseObject as Record;
                var value = (string)record.Data[record.Columns["Value"].Index];
                Assert.AreEqual<string>("1.0.0", value);
                Assert.AreEqual<RowOperation>(RowOperation.None, item.GetPropertyValue<RowOperation>("MSIOperation"));
            }
        }

        [TestMethod]
        public void GetAllPatchedRecords()
        {
            Collection<PSObject> tables = null;
            using (var p = CreatePipeline(@"get-msitable -path example.msi -patch example.msp"))
            {
                tables = p.Invoke();
                Assert.IsNotNull(tables);

                // Only persistent tables are piped which excludes _TransformView.
                Assert.AreEqual<int>(16, tables.Count);
            }

            using (var p = CreatePipeline(@"$input | get-msitable | where-object { $_.MSIOperation -ne 'None' }"))
            {
                var output = p.Invoke(tables);
                Assert.IsNotNull(output);
                Assert.AreEqual<int>(9, output.Count);

                var e = from obj in output
                        from prop in obj.Properties
                        where prop.Name == "DiskId" && 100 == (int)prop.Value
                        select obj;

                Assert.IsNotNull(e);
                Assert.AreEqual<long>(1, e.Count());
            }
        }

        [TestMethod]
        public void GetTransformViewTable()
        {
            using (var p = CreatePipeline(@"get-msitable example.msi -transform example.mst -table _TransformView"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null != output && 0 < output.Count);
            }
        }

        [TestMethod]
        public void QueryInstalledProduct()
        {
            using (var p = CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msitable -table Registry"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();
                    Assert.IsTrue(null != output && 1 == output.Count, "Output is incorrect.");

                    var item = output[0];
                    Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#Registry", item.TypeNames[0], "The first type name is incorrect.");

                    // Work around a possible bug in PowerShell where adapted property values are cached.
                    var record = item.BaseObject as Record;
                    var value = (string)record.Data[record.Columns["Value"].Index];
                    Assert.AreEqual<string>("1.0.1", value, "The Value property is incorrect.");
                }
            }
        }

        [TestMethod]
        public void QueryInstalledProductIgnoreMachineState()
        {
            using (var p = CreatePipeline(@"get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msitable -table Registry -ignoremachinestate"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();
                    Assert.IsTrue(null != output && 1 == output.Count, "Output is incorrect.");

                    var item = output[0];
                    Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#Registry", item.TypeNames[0], "The first type name is incorrect.");

                    // Work around a possible bug in PowerShell where adapted property values are cached.
                    var record = item.BaseObject as Record;
                    var value = (string)record.Data[record.Columns["Value"].Index];
                    Assert.AreEqual<string>("1.0.0", value, "The Value property is incorrect.");
                }
            }
        }

        [TestMethod]
        public void QueryAllColumns()
        {
            using (var p = CreatePipeline(@"get-msitable example.msi -query 'select * from Property'"))
            {
                using (OverrideRegistry())
                {
                    var output = p.Invoke();
                    Assert.IsTrue(null != output && 0 < output.Count);

                    var item = output[0];
                    Assert.IsNotNull(item.Properties.Match("Property"));
                    Assert.IsNotNull(item.Properties.Match("Value"));
                }
            }
        }

        [TestMethod]
        public void GetClassificationFromPatchMetadata()
        {
            using (var p = CreatePipeline("get-msitable example.msp -table MsiPatchMetadata | where { $_.Property -eq 'Classification' }"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null != output && 1 == output.Count, "Output is incorrect.");

                var item = output[0];
                Assert.AreEqual<string>("Microsoft.Tools.WindowsInstaller.Record#MsiPatchMetadata", item.TypeNames[0], "The first type name is incorrect.");

                var value = item.GetPropertyValue<string>("Value");
                Assert.AreEqual<string>("Update", value, "The Classification property value is incorrect.");
            }
        }

        [TestMethod]
        public void FormatComponentAttributes()
        {
            using (var p = CreatePipeline(@"get-msitable example.msi -table Component | where { $_.Attributes.HasRegistryKeyPath }"))
            {
                var output = p.Invoke();
                Assert.IsTrue(null != output && 1 == output.Count);

                var item = output[0];

                var attributes = item.Members.Match("Attributes", PSMemberTypes.Properties).FirstOrDefault();
                Assert.IsNotNull(attributes);

                var obj = PSObject.AsPSObject(attributes.Value);
                var toString = obj.Members.Match("ToString", PSMemberTypes.Methods).FirstOrDefault() as PSMethodInfo;
                Assert.IsNotNull(toString);

                // Default formatting.
                var value = toString.Invoke().ToString();
                Assert.AreEqual<string>("4", value, "The default formatting is incorrect.");

                // Hexadecimal formatting.
                p.Runspace.SessionStateProxy.SetVariable("MsiAttributeColumnFormat", "X");
                value = toString.Invoke().ToString();
                Assert.AreEqual<string>("0x00000004", value, "The hexadecimal formatting is incorrect.");

                // Enumeration name formatting.
                p.Runspace.SessionStateProxy.SetVariable("MsiAttributeColumnFormat", "F");
                value = toString.Invoke().ToString();
                Assert.AreEqual<string>("RegistryKeyPath", value, "The enumeration name formatting is incorrect.");

                // Exceptions use default formatting.
                p.Runspace.SessionStateProxy.SetVariable("MsiAttributeColumnFormat", "Z");
                value = toString.Invoke().ToString();
                Assert.AreEqual<string>("4", value, "The exceptional formatting is incorrect.");
            }
        }

        [TestMethod]
        public void TableFromAdvertisedProduct()
        {
            using (var p = CreatePipeline("get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msitable -table Property -wv Warnings"))
            {
                var path = Path.Combine(this.TestContext.DeploymentDirectory, "Corrupt.xml");
                using (OverrideRegistry("Corrupt.xml"))
                {
                    p.Invoke();

                    Assert.AreEqual<int>(0, p.Output.Count, "Output was not expected.");
                    Assert.AreEqual<int>(0, p.Error.Count, "Errors were not expected.");

                    var warnings = p.Runspace.SessionStateProxy.GetVariable("Warnings") as ICollection;
                    Assert.IsNotNull(warnings, "Expected a Warnings variable.");
                    Assert.AreEqual<int>(1, warnings.Count, "Expected a warning.");
                }
            }
        }

        [TestMethod]
        public void TableFromCorruptProduct()
        {
            using (var p = CreatePipeline("get-msiproductinfo '{9AC08E99-230B-47e8-9721-4577B7F124EA}' | get-msitable -table Property -wv Warnings"))
            {
                var path = Path.Combine(this.TestContext.DeploymentDirectory, "Corrupt.xml");
                using (OverrideRegistry("Corrupt.xml"))
                {
                    p.Invoke();

                    Assert.AreEqual<int>(0, p.Output.Count, "Output was not expected.");
                    Assert.AreEqual<int>(1, p.Error.Count, "Expected an error.");

                    var warnings = p.Runspace.SessionStateProxy.GetVariable("Warnings") as ICollection;
                    Assert.IsNotNull(warnings, "Expected a Warnings variable.");
                    Assert.AreEqual<int>(0, warnings.Count, "Warnings were not expected.");
                }
            }
        }
    }
}
