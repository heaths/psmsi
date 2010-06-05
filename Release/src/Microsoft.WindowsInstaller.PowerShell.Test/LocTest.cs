// Support methods and properties for the test project.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsInstaller
{
    /// <summary>
    /// Tests localization of the project using pseudo-localization files.
    /// </summary>
    /// <remarks>
    /// To enable this on Windows Vista and newer, see http://msdn.microsoft.com/en-us/library/aa366908.aspx.
    /// </remarks>
    [TestClass]
    public sealed class LocTest
    {
        /// <summary>
        /// Tests pseudo-localized help for cmdlets.
        /// </summary>
        [TestMethod]
        [Description("Tests PLOC help for cmdlets.")]
        public void PlocCmdletHelpTest()
        {
            using (Pipeline p = TestProject.TestRunspace.CreatePipeline(@"set-uiculture 'qps-ploc'; get-help get-msiproductinfo"))
            {
                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<string>(@"{Gééts próódüüçt ííñformââtioñ for registered produçts.}", objs[0].Properties["Synopsis"].Value as string);
            }
        }

        /// <summary>
        /// Tests pseudo-localized help for functions.
        /// </summary>
        [TestMethod]
        [Description("Tests PLOC help for functions.")]
        public void PlocFunctionHelpTest()
        {
            using (Pipeline p = TestProject.TestRunspace.CreatePipeline(@"set-uiculture 'qps-ploc'; get-help get-msisharedcomponentinfo"))
            {
                Collection<PSObject> objs = p.Invoke();

                Assert.AreEqual<int>(1, objs.Count);
                Assert.AreEqual<string>(@"{Gééts ííñfóórmââtioñ aboüüt shared çompoñeñts iñstalled or registered for the çurreñt user or the maçhiñe.}", objs[0].Properties["Synopsis"].Value as string);
            }
        }
    }
}
