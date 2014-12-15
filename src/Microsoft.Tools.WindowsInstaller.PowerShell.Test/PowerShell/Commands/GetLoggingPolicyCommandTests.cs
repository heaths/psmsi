// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    [TestClass]
    public class GetLoggingPolicyCommandTests : TestBase
    {
        [TestMethod]
        public void GetMissingLoggingPolicy()
        {
            var service = new TestLoggingPolicyService()
            {
                GetLoggingPolicyAction = () => null,
            };

            var cmd = new GetLoggingPolicyCommand(service);
            var output = cmd.Invoke<string>();

            Assert.IsNotNull(output);
            Assert.AreEqual<int>(0, output.Count());
        }

        [TestMethod]
        public void GetLoggingPolicy()
        {
            var expected = new string[]
            {
                "FatalExit",
                "Error",
                "Warning",
                "User",
                "Information",
                "OutOfDiskSpace",
                "ActionStart",
                "ActionData",
                "PropertyDump",
                "CommonData",
                "Verbose",
                "ExtraDebug",
            };

            var service = new TestLoggingPolicyService()
            {
                GetLoggingPolicyAction = () => "voicewarmupx",
            };

            var cmd = new GetLoggingPolicyCommand(service);
            var output = cmd.Invoke<string>();

            Assert.IsNotNull(output);
            CollectionAssert.AreEquivalent(expected, output.ToArray());

            cmd = new GetLoggingPolicyCommand(service)
            {
                Raw = true,
            };
            output = cmd.Invoke<string>();

            Assert.IsNotNull(output);
            Assert.AreEqual("voicewarmupx", output.FirstOrDefault());
        }
    }
}
