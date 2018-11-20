// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                    }
                }
            }
        }
    }
}
