// Unit test classes for exceptions.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell
{
    /// <summary>
    /// A test for the <see cref="PSInvalidParameterException"/> class.
    /// </summary>
    [TestClass]
    public class PSInvalidParameterExceptionTest
    {
        /// <summary>
        /// Serialization test for <see cref="PSInvalidParameterException"/>.
        /// </summary>
        [TestMethod]
        [Description("Serialization test for PSInvalidParameterException")]
        public void SerializationTest()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Serialize.
                PSInvalidParameterException ex = new PSInvalidParameterException("prop1", "value1");
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, ex);

                // Deserialize and compare.
                ms.Seek(0, SeekOrigin.Begin);
                ex = (PSInvalidParameterException)formatter.Deserialize(ms);
                Assert.AreEqual<string>(@"""value1"" is not valid for the prop1 parameter.", ex.Message);
                Assert.AreEqual<string>("prop1", ex.ParamName);
            }
        }
    }
}
