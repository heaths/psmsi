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
using System;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="IceMessage"/> class.
    /// </summary>
    [TestClass]
    public sealed class IceMessageTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidIceMessage()
        {
            var ice = new IceMessage("ICE00\t1");
        }

        [TestMethod]
        public void MinimumIceMessage()
        {
            var ice = new IceMessage("ICE00\t3\tTest description.");
            Assert.AreEqual<string>("ICE00", ice.Name, "The ICE name is incorrect.");
            Assert.AreEqual<IceMessageType>(IceMessageType.Information, ice.Type, "The ICE type is incorrect.");
            Assert.AreEqual<string>("Test description.", ice.Description, "The ICE description is incorrect.");
        }

        [TestMethod]
        public void MaximumIceMessage()
        {
            var ice = new IceMessage("ICE00\t3\tTest description.\thttp://psmsi.codeplex.com\tTable\tColumn\tKey1\tKey2");
            Assert.AreEqual<string>("ICE00", ice.Name, "The ICE name is incorrect.");
            Assert.AreEqual<IceMessageType>(IceMessageType.Information, ice.Type, "The ICE type is incorrect.");
            Assert.AreEqual<string>("Test description.", ice.Description, "The ICE description is incorrect.");
            Assert.AreEqual<string>("http://psmsi.codeplex.com", ice.Url, "The ICE URL is incorrect.");
            Assert.AreEqual<string>("Table", ice.Table, "The ICE table is incorrect.");
            Assert.AreEqual<string>("Column", ice.Column, "The ICE column is incorrect.");
            CollectionAssert.AreEqual(new string[] { "Key1", "Key2" }, ice.PrimaryKeys, "The ICE primary keys are incorrect.");
        }
    }
}
