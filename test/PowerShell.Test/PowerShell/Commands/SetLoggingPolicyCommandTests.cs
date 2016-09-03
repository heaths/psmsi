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
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
