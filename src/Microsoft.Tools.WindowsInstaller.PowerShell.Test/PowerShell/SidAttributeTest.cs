// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="SidAttribute"/> class.
    /// </summary>
    [TestClass]
    public sealed class SidAttributeTest : TestBase
    {
        [TestMethod]
        public void TransformTest()
        {
            var attr = new SidAttribute();

            // Test null input.
            Assert.IsNull(attr.Transform(null, null));

            // Test another object.
            Assert.AreEqual(1, attr.Transform(null, 1));

            // Test a string.
            Assert.AreEqual(CurrentSID, attr.Transform(null, CurrentUsername));
        }

        [TestMethod]
        public void TryParseUsernameTest()
        {
            string param = null;

            // Test a string without backslashes.
            Assert.IsFalse(SidAttribute.TryParseUsername(@"foo", out param));
            Assert.IsNull(param);

            // Test a string with backslashes but not a valid username.
            Assert.IsFalse(SidAttribute.TryParseUsername(@"foo\bar\baz", out param));
            Assert.IsNull(param);

            // Test a valid username.
            Assert.IsTrue(SidAttribute.TryParseUsername(CurrentUsername, out param));
            Assert.AreEqual<string>(CurrentSID, param);
        }
    }
}
