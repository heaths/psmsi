// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using Microsoft.Deployment.WindowsInstaller.Package;

namespace Microsoft.Tools.WindowsInstaller
{
    [TestClass]
    public class TransformViewTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformViewNullArguments()
        {
            var view = new TransformView(null);
        }

        [TestMethod]
        public void NoTransformViewChanges()
        {
            var package = Path.Combine(base.TestContext.DeploymentDirectory, "Example.msi");
            using (var db = new InstallPackage(package, DatabaseOpenMode.ReadOnly))
            {
                var view = new TransformView(db);
                Assert.AreEqual<int>(0, view.Tables.Count);
            }
        }

        [TestMethod]
        public void TransfomViewChanges()
        {
            var package = Path.Combine(base.TestContext.DeploymentDirectory, "Example.msi");
            var patch = Path.Combine(base.TestContext.DeploymentDirectory, "Example.msp");
            using (var db = new InstallPackage(package, DatabaseOpenMode.ReadOnly))
            {
                var applicator = new PatchApplicator(db);
                applicator.Add(patch);

                applicator.Apply(true);

                var view = new TransformView(db);

                // Despite Orca showing 5 tables, the _TransformView table does not show the created, empty "Patch" table.
                Assert.AreEqual<int>(4, view.Tables.Count);

                Assert.AreEqual<TableOperation>(TableOperation.Modify, view.GetTableOperation("Media"));
                Assert.AreEqual<RowOperation>(RowOperation.None, view.GetRowOperation("Media", "1"));
                Assert.AreEqual<RowOperation>(RowOperation.Insert, view.GetRowOperation("Media", "100"));

                Assert.AreEqual<TableOperation>(TableOperation.Create, view.GetTableOperation("PatchPackage"));
                Assert.AreEqual<RowOperation>(RowOperation.Insert, view.GetRowOperation("PatchPackage", "{FF63D787-26E2-49CA-8FAA-28B5106ABD3A}"));

                Assert.AreEqual<TableOperation>(TableOperation.Modify, view.GetTableOperation("Property"));
                Assert.AreEqual<RowOperation>(RowOperation.None, view.GetRowOperation("Property", "ProductCode"));
                Assert.AreEqual<RowOperation>(RowOperation.Modify, view.GetRowOperation("Property", "ProductVersion"));
                Assert.AreEqual<RowOperation>(RowOperation.Insert, view.GetRowOperation("Property", "Example.PatchCode"));
                Assert.AreEqual<RowOperation>(RowOperation.Insert, view.GetRowOperation("Property", "Example.AllowRemoval"));

                Assert.AreEqual<TableOperation>(TableOperation.Modify, view.GetTableOperation("Registry"));
                Assert.AreEqual<RowOperation>(RowOperation.Modify, view.GetRowOperation("Registry", "reg302A797C45AD3AD1EC816DDC58DF65F3"));

                // Negative assertions.
                Assert.AreEqual<TableOperation>(TableOperation.None, view.GetTableOperation("File"));
                Assert.AreEqual<RowOperation>(RowOperation.None, view.GetRowOperation(null, null));
                Assert.AreEqual<RowOperation>(RowOperation.None, view.GetRowOperation("File", null));
                Assert.AreEqual<RowOperation>(RowOperation.None, view.GetRowOperation("File", "product.wxs"));
            }
        }
    }
}
