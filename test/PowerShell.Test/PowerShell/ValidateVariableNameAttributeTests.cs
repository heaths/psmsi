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
    public class ValidateVariableNameAttributeTests
    {
        [TestMethod]
        public void VariableNameValueNull()
        {
            var sut = new ValidateVariableNameAttribute();
            Assert.IsFalse(sut.Validate(null));
        }

        [TestMethod]
        public void VariableNameValueEmpty()
        {
            var sut = new ValidateVariableNameAttribute();
            Assert.IsFalse(sut.Validate(null));
        }

        [TestMethod]
        public void VariableNameValueNumeric()
        {
            var sut = new ValidateVariableNameAttribute();
            Assert.IsFalse(sut.Validate(0));
        }

        [TestMethod]
        public void VariableNameValueValid()
        {
            var sut = new ValidateVariableNameAttribute();
            Assert.IsTrue(sut.Validate("a"));
        }

        [TestMethod]
        public void VariableNameValueValidAppend()
        {
            var sut = new ValidateVariableNameAttribute();
            Assert.IsTrue(sut.Validate("+a"));
        }
    }
}
