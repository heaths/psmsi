// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="MeasureProductCommandTests"/> class.
    /// </summary>
    [TestClass]
    public sealed class MeasureProductCommandTests : TestBase
    {
        [TestMethod]
        public void MeasureProductOnSystemDrive()
        {
            using (SubstituteDrive.Next(this.TestContext.TestRunDirectory))
            {
                var command = string.Format(@"measure-msiproduct example.msi -destination ""{0}""", Path.Combine(Environment.SystemDirectory, @"\TEST"));
                using (var p = CreatePipeline(command))
                {
                    var output = p.Invoke();
                    Assert.IsTrue(2 <= output.Count());

                    var systemDrive = Environment.SystemDirectory.Substring(0, 2);
                    foreach (var drive in output)
                    {
                        var name = drive.GetPropertyValue<string>("Name") + ":";

                        if (name.Equals(systemDrive, StringComparison.OrdinalIgnoreCase))
                        {
                            Assert.IsTrue(0 < drive.GetPropertyValue<int>("MSISpaceRequired"));
                            Assert.IsTrue(0 < drive.GetPropertyValue<int>("MSITemporarySpaceRequired"));
                        }
                        else
                        {
                            Assert.IsTrue(0 == drive.GetPropertyValue<int>("MSISpaceRequired"));
                            Assert.IsTrue(0 == drive.GetPropertyValue<int>("MSITemporarySpaceRequired"));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void MeasureProductOnOtherDrive()
        {
            using (var d = SubstituteDrive.Next(this.TestContext.TestRunDirectory))
            {
                var command = string.Format(@"measure-msiproduct example.msi -destination ""{0}""", d.DriveLetter + @"\TEST");
                using (var p = CreatePipeline(command))
                {
                    var output = p.Invoke();
                    Assert.IsTrue(2 <= output.Count());

                    var systemDrive = Environment.SystemDirectory.Substring(0, 2);
                    foreach (var drive in output)
                    {
                        var name = drive.GetPropertyValue<string>("Name") + ":";

                        if (name.Equals(systemDrive, StringComparison.OrdinalIgnoreCase))
                        {
                            Assert.IsTrue(0 <= drive.GetPropertyValue<int>("MSISpaceRequired"));
                            Assert.IsTrue(0 < drive.GetPropertyValue<int>("MSITemporarySpaceRequired"));
                        }
                        else if (name.Equals(d.DriveLetter, StringComparison.OrdinalIgnoreCase))
                        {
                            Assert.IsTrue(0 < drive.GetPropertyValue<int>("MSISpaceRequired"));
                            Assert.IsTrue(0 <= drive.GetPropertyValue<int>("MSITemporarySpaceRequired"));
                        }
                        else
                        {
                            Assert.IsTrue(0 == drive.GetPropertyValue<int>("MSISpaceRequired"));
                            Assert.IsTrue(0 == drive.GetPropertyValue<int>("MSITemporarySpaceRequired"));
                        }
                    }
                }
            }
        }
    }
}
