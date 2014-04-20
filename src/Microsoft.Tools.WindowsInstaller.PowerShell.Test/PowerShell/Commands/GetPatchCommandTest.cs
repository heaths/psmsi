// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Management.Automation.Runspaces;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetPatchCommand"/>.
    ///</summary>
    [TestClass]
    public class GetPatchCommandTest : TestBase
    {
        [TestMethod]
        public void EnumeratePatches()
        {
            using (var p = CreatePipeline(@"get-msipatchinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(2, objs.Count);
                    Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].GetPropertyValue<string>("PatchCode"));
                }
            }
        }

        [TestMethod]
        public void EnumerateProductPatches()
        {
            using (var p = CreatePipeline(@"get-msiproductinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" | get-msipatchinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].GetPropertyValue<string>("PatchCode"));
                }
            }
        }

        [TestMethod]
        public void GetSpecificPatch()
        {
            using (var p = CreatePipeline(@"get-msipatchinfo -productcode ""{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}"" -patchcode ""{6E52C409-0D0D-4B84-AB63-463438D4D33B}"""))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].GetPropertyValue<string>("PatchCode"));
                }
            }
        }

        [TestMethod]
        public void GetSupersededPatches()
        {
            using (var p = CreatePipeline(@"get-msipatchinfo -filter superseded"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(0, objs.Count);
                }
            }
        }

        [TestMethod]
        public void UserSidTest()
        {
            var cmdlet = new GetPatchCommand();
            cmdlet.UserSid = "S-1-5-21-2127521184-1604012920-1887927527-2039434";
            Assert.AreEqual<string>("S-1-5-21-2127521184-1604012920-1887927527-2039434", cmdlet.UserSid);
        }

        [TestMethod]
        public void InstallContextTest()
        {
            // Test that None is not supported.
            var cmdlet = new GetPatchCommand();
            ExceptionAssert.Throws<ArgumentException>(() =>
            {
                cmdlet.UserContext = UserContexts.None;
            });

            // Test that "Context" is a supported alias.
            var cmd = string.Format(@"get-msipatchinfo -context ""machine""");
            using (var p = CreatePipeline(cmd))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].GetPropertyValue<string>("PatchCode"));
                }
            }
        }

        [TestMethod]
        public void FilterTest()
        {
            var cmdlet = new GetPatchCommand();

            // Test the default is "Applied".
            Assert.AreEqual<PatchStates>(PatchStates.Applied, cmdlet.Filter);

            // Test that Invalid is not supported.
            ExceptionAssert.Throws<ArgumentException>(() =>
            {
                cmdlet.Filter = PatchStates.None;
            });
        }

        [TestMethod]
        public void EveryoneTest()
        {
            var cmdlet = new GetPatchCommand();

            // Test that the default is false / not present.
            Assert.AreEqual<bool>(false, cmdlet.Everyone);
            Assert.AreEqual<bool>(false, cmdlet.Everyone.IsPresent);

            // Test that we can set it to true.
            cmdlet.Everyone = true;
            Assert.AreEqual<bool>(true, cmdlet.Everyone);
            Assert.AreEqual<string>(NativeMethods.World, cmdlet.UserSid);

            // Test that explicitly setting it to false nullifies the UserSid.
            cmdlet.Everyone = false;
            Assert.AreEqual<bool>(false, cmdlet.Everyone);
            Assert.AreEqual<string>(null, cmdlet.UserSid);
        }

        [TestMethod]
        [WorkItem(9464)]
        public void GetPatchChainedExecution()
        {
            using (Pipeline p = CreatePipeline(@"get-msipatchinfo -productCode '{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}' | get-msipatchinfo"))
            {
                using (OverrideRegistry())
                {
                    var objs = p.Invoke();

                    Assert.AreEqual(1, objs.Count);
                    Assert.AreEqual<string>("{6E52C409-0D0D-4B84-AB63-463438D4D33B}", objs[0].GetPropertyValue<string>("PatchCode"));
                }
            }
        }
    }
}
