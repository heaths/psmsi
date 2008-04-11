// Unit test class for the Help class.
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer.PowerShell
{
    /// <summary>
    /// Unit tests for the <see cref="Help"/> support class.
    /// </summary>
    [TestClass]
    public class HelpTest
    {
        /// <summary>
        /// A test for <see cref="Help.FormatDebugMessage"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for Help.FormatDebugMessage")]
        public void FormatDebugMessageTest()
        {
            string message;

            message = Help.FormatDebugMessage("MsiEnumProductsEx", "{0CABECAC-4E23-4928-871A-6E65CD370F9F}", null, (int)InstallContext.Machine, 0);
            Assert.AreEqual<string>(@"MsiEnumProductsEx(""{0CABECAC-4E23-4928-871A-6E65CD370F9F}"", """", 0x04, 0, ...)", message);

            message = Help.FormatDebugMessage("StgOpenStorageEx");
            Assert.AreEqual<string>("StgOpenStorageEx", message);
        }
    }
}
