// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
