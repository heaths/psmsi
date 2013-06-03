// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="RecordPropertyAdapter"/> class.
    /// </summary>
    [TestClass]
    public sealed class RecordPropertyAdapterTests : TestBase
    {
        [TestMethod]
        public void FileTableRecordAdapted()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            using (var db = new Database(path, DatabaseOpenMode.ReadOnly))
            {
                var query = db.Tables["File"].SqlSelectString;
                using (var view = db.OpenView(query))
                {
                    view.Execute();
                    var columns = ViewManager.GetColumns(view);

                    // Fetch and test a single record.
                    using (var record = view.Fetch())
                    {
                        Assert.IsNotNull(record, "No record was found.");
                        var copy = new Record(record, columns);

                        var adapter = new RecordPropertyAdapter();
                        var properties = adapter.GetProperties(copy);
                        Assert.IsNotNull(properties, "The properties were not adapted.");
                        Assert.AreEqual<int>(8, properties.Count, "The number of columns are incorrect.");

                        var property = adapter.GetProperty(copy, "FileName");
                        var type = typeof(string).FullName;
                        Assert.IsNotNull(property, "The FileName property was not adapted.");
                        Assert.IsTrue(adapter.IsGettable(property), "The FileName property is not gettable.");
                        Assert.AreEqual(type, adapter.GetPropertyTypeName(property), true, "The FileName property type is incorrect.");
                        Assert.AreEqual("product.wxs", RecordPropertyAdapter.GetPropertyValue(property, copy) as string, "The FileName propert value is incorrect.");

                        property = adapter.GetProperty(copy, "Attributes");
                        type = typeof(Nullable<short>).FullName;
                        Assert.IsNotNull(property, "The Attributes property was not adapted.");
                        Assert.AreEqual(type, adapter.GetPropertyTypeName(property), true, "The Attributes property type is incorrect.");
                        Assert.AreEqual<short>(512, Convert.ToInt16(RecordPropertyAdapter.GetPropertyValue(property, copy)), "The Attributes propert value is incorrect.");

                        property = adapter.GetProperty(copy, "Sequence");
                        type = typeof(int).FullName;
                        Assert.IsNotNull(property, "The Sequence property was not adapted.");
                        Assert.AreEqual("System.Int32", adapter.GetPropertyTypeName(property), true, "The Sequence property type is incorrect.");
                        Assert.AreEqual<int>(1, Convert.ToInt32(RecordPropertyAdapter.GetPropertyValue(property, copy)), "The Sequence propert value is incorrect.");
                    }
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void RecordIsReadOnly()
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msi");
            using (var db = new Database(path, DatabaseOpenMode.ReadOnly))
            {
                var query = db.Tables["File"].SqlSelectString;
                using (var view = db.OpenView(query))
                {
                    view.Execute();
                    var columns = ViewManager.GetColumns(view);

                    // Fetch and test a single record.
                    using (var record = view.Fetch())
                    {
                        Assert.IsNotNull(record, "No record was found.");
                        var copy = new Record(record, columns);

                        var adapter = new RecordPropertyAdapter();
                        var property = adapter.GetProperty(copy, "FileName");
                        Assert.IsFalse(adapter.IsSettable(property), "The FileName property is settable.");

                        // Throws NotSupportedException.
                        adapter.SetPropertyValue(property, "test.wxs");
                    }
                }
            }
        }
    }
}
