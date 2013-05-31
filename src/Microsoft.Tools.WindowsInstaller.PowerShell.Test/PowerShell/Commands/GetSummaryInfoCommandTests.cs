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
using System.Management.Automation.Runspaces;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="GetSummaryInfoCommand"/> class.
    /// </summary>
    [TestClass]
    public sealed class GetSummaryInfoCommandTests : CommandTestBase
    {
        [TestMethod]
        public void GetSummaryInfoFromMsi()
        {
            using (var p = this.TestRunspace.CreatePipeline(@"get-msisummaryinfo ""$TestDeploymentDirectory\Example.msi"""))
            {
                var items = p.Invoke();
                Assert.AreEqual<int>(1, items.Count);

                // Assert values for ETS properties.
                var info = items[0];
                Runspace.DefaultRunspace = p.Runspace;
                
                Assert.IsTrue(info.GetPropertyValue<bool>("IsPackage"));
                Assert.AreEqual<ReadOnly>(ReadOnly.Recommended, info.GetPropertyValue<ReadOnly>("ReadOnly"));
                Assert.AreEqual<string>("Intel", info.GetPropertyValue<string>("Platform"));
                Assert.AreEqual<string>("1033", info.GetPropertyValue<string>("Language"));
                Assert.AreEqual<string>("{BB960DDA-CC6E-4B2C-8A89-F0344814A5B2}", info.GetPropertyValue<string>("PackageCode"));
                Assert.AreEqual<Version>(new Version(3, 1), info.GetPropertyValue<Version>("MinimumVersion"));
            }
        }

        [TestMethod]
        public void GetSummaryInfoFromMsp()
        {
            using (var p = this.TestRunspace.CreatePipeline(@"get-msisummaryinfo ""$TestDeploymentDirectory\Example.msp"""))
            {
                var items = p.Invoke();
                Assert.AreEqual<int>(1, items.Count);

                // Assert values for ETS properties.
                var info = items[0];
                Runspace.DefaultRunspace = p.Runspace;

                Assert.IsTrue(info.GetPropertyValue<bool>("IsPatch"));
                Assert.AreEqual<ReadOnly>(ReadOnly.Enforced, info.GetPropertyValue<ReadOnly>("ReadOnly"));
                CollectionAssert.AreEqual(new string[] { string.Empty }, info.GetPropertyValue<string[]>("Sources"));
                CollectionAssert.AreEqual(new string[] { ":MSP.1", ":#MSP.1" }, info.GetPropertyValue<string[]>("Transforms"));
                Assert.AreEqual<string>("{FF63D787-26E2-49CA-8FAA-28B5106ABD3A}", info.GetPropertyValue<string>("PackageCode"));
                Assert.IsNotNull(info.GetPropertyValue<object>("ReplacedPatchCodes"));
                Assert.AreEqual<string>("{877EF582-78AF-4D84-888B-167FDC3BCC11}", info.GetPropertyValue<string>("TargetProductCodes"));
                Assert.AreEqual<Version>(new Version(3, 1), info.GetPropertyValue<Version>("MinimumVersion"));
            }
        }

        [TestMethod]
        public void GetSummaryInfoFromMspTransforms()
        {
            using (var p = this.TestRunspace.CreatePipeline(@"get-msisummaryinfo ""$TestDeploymentDirectory\Example.msp"" -transforms | where { $_.IsTransform -and $_.Name -notlike ""#*"" }"))
            {
                var items = p.Invoke();
                Assert.AreEqual<int>(1, items.Count);

                // Assert values for ETS properties.
                var info = items[0];
                Runspace.DefaultRunspace = p.Runspace;

                Assert.IsTrue(info.GetPropertyValue<bool>("IsTransform"));
                Assert.AreEqual<string>("MSP.1", info.GetPropertyValue<string>("Name"));

                var path = Path.Combine(this.TestContext.DeploymentDirectory, "Example.msp");
                var transform = path + ":" + info.GetPropertyValue<string>("Name");
                Assert.AreEqual<string>(transform, info.GetPropertyValue<string>("Transform"));

                var validations = TransformValidations.Product | TransformValidations.UpdateVersion | TransformValidations.NewEqualBaseVersion | TransformValidations.UpgradeCode;
                Assert.AreEqual<TransformValidations>(validations, info.GetPropertyValue<TransformValidations>("Validations"));
            }
        }

        [TestMethod]
        public void GetSummaryInfoFromMst()
        {
            using (var p = this.TestRunspace.CreatePipeline(@"get-msisummaryinfo ""$TestDeploymentDirectory\Example.mst"""))
            {
                var items = p.Invoke();
                Assert.AreEqual<int>(1, items.Count);

                // Assert values for ETS properties.
                var info = items[0];
                Runspace.DefaultRunspace = p.Runspace;

                Assert.IsTrue(info.GetPropertyValue<bool>("IsTransform"));
                Assert.AreEqual<ReadOnly>(ReadOnly.Enforced, info.GetPropertyValue<ReadOnly>("ReadOnly"));

                var validations = TransformValidations.Product | TransformValidations.UpdateVersion | TransformValidations.NewEqualBaseVersion | TransformValidations.UpgradeCode;
                Assert.AreEqual<TransformValidations>(validations, info.GetPropertyValue<TransformValidations>("Validations"));

                var conditions = TransformErrors.AddExistingRow | TransformErrors.AddExistingTable | TransformErrors.DelMissingRow | TransformErrors.DelMissingTable | TransformErrors.UpdateMissingRow;
                Assert.AreEqual<TransformErrors>(conditions, info.GetPropertyValue<TransformErrors>("ErrorConditions"));

                Assert.AreEqual<string>("{000C1109-0000-0000-C000-000000000046}", info.GetPropertyValue<string>("TargetProductCode"));
                Assert.AreEqual<Version>(new Version(0, 0, 0, 0), info.GetPropertyValue<Version>("TargetProductVersion"));
                Assert.AreEqual<string>("{F400B367-33CF-429E-B571-0FDCF253ABC2}", info.GetPropertyValue<string>("UpgradeCode"));
                Assert.AreEqual<string>("Intel", info.GetPropertyValue<string>("UpgradePlatform"));
                Assert.AreEqual<string>("1033", info.GetPropertyValue<string>("UpgradeLanguage"));
                Assert.AreEqual<string>("{000C1109-0000-0000-C000-000000000046}", info.GetPropertyValue<string>("UpgradeProductCode"));
                Assert.AreEqual<Version>(new Version(0, 0, 0, 0), info.GetPropertyValue<Version>("UpgradeProductVersion"));
                Assert.AreEqual<Version>(new Version(2, 0), info.GetPropertyValue<Version>("MinimumVersion"));
            }
        }
    }
}
