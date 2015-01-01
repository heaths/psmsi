// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    [TestClass]
    public class RemoveLoggingPolicyCommandTests : TestBase
    {
        [TestMethod]
        public void RemoveLoggingPolicy()
        {
            var called = false;
            var service = new TestLoggingPolicyService()
            {
                RemoveLoggingPolicyAction = () => { called = true; }
            };

            var cmd = new RemoveLoggingPolicyCommand(service);
            var output = cmd.Invoke<PSObject>();

            // Need to enumerate output to call BeginProcessing().
            Assert.IsNotNull(output);
            Assert.IsNull(output.FirstOrDefault());

            Assert.IsTrue(called);
        }
    }
}
