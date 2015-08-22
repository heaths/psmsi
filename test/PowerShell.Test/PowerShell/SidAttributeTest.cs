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
