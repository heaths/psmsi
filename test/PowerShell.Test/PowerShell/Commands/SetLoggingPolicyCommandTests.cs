// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    [TestClass]
    public class SetLoggingPolicyCommandTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void SetLoggingPolicyUnathorized()
        {
            var service = new TestLoggingPolicyService()
            {
                SetLoggingPolicyAction = (value) =>
                    {
                        throw new UnauthorizedAccessException();
                    },
            };

            var cmd = new SetLoggingPolicyCommand(service);
            var output = cmd.Invoke<string>();

            // Need to enumerate output to call BeginProcessing().
            Assert.IsNotNull(output);
            Assert.IsNull(output.FirstOrDefault());
        }

        [TestMethod]
        public void SetLoggingPolicy()
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
            };

            var service = new TestLoggingPolicyService()
            {
                SetLoggingPolicyAction = (value) => { },
            };

            var cmd = new SetLoggingPolicyCommand(service)
            {
                LoggingPolicy = LoggingPolicies.All & ~LoggingPolicies.ExtraDebug,
            };
            var output = cmd.Invoke<string>();

            Assert.IsNotNull(output);
            Assert.AreEqual<int>(0, output.Count());

            cmd = new SetLoggingPolicyCommand(service)
            {
                LoggingPolicy = LoggingPolicies.All & ~LoggingPolicies.ExtraDebug,
                PassThru = true,
            };
            output = cmd.Invoke<string>();

            Assert.IsNotNull(output);
            CollectionAssert.AreEquivalent(expected, output.ToArray());

            cmd = new SetLoggingPolicyCommand(service)
            {
                LoggingPolicy = LoggingPolicies.All & ~LoggingPolicies.ExtraDebug,
                PassThru = true,
                Raw = true,
            };
            output = cmd.Invoke<string>();

            Assert.IsNotNull(output);

            var actual = output.FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(actual));

            CollectionAssert.AreEquivalent("voicewarmup".ToArray(), actual.ToArray());
        }
    }
}
