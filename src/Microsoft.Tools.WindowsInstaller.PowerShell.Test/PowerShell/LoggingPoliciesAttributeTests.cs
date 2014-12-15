// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    [TestClass]
    public sealed class LoggingPoliciesAttributeTests : TestBase
    {
        private const LoggingPolicies Default = LoggingPoliciesConverter.Normal;

        [TestMethod]
        public void TransformNullLoggingValue()
        {
            var attr = new LoggingPoliciesAttribute();
            Assert.IsNull(attr.Transform(null, null));
        }

        [TestMethod]
        public void TransformStringLoggingValue()
        {
            var attr = new LoggingPoliciesAttribute();
            Assert.AreEqual<LoggingPolicies>(Default, (LoggingPolicies)attr.Transform(null, "oiceWARMUP"));
        }

        [TestMethod]
        public void TransformUnsupportedLoggingValue()
        {
            var attr = new LoggingPoliciesAttribute();
            Assert.AreEqual<int>(0, (int)attr.Transform(null, 0));
        }
    }
}
