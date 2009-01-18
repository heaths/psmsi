// Tests the PackageSource class.
//
// Author: Heath Stewart
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Installer
{
    /// <summary>
    /// A test for the <see cref="PackageSource"/> class.
    /// </summary>
    [TestClass]
    public class PackageSourceTest
    {
        /// <summary>
        /// A test for <see cref="PackageSource.PackageSource"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for PackageSource.PackageSource(string)")]
        public void CreateFromStringTest()
        {
            PackageSource source;

            // Url
            source = new PackageSource(@"u;1;http://www.codeplex.com/psmsi");
            Assert.AreEqual<SourceTypes>(SourceTypes.Url, source.SourceType);
            Assert.AreEqual<int>(1, source.Index);
            Assert.AreEqual<string>(@"http://www.codeplex.com/psmsi", source.Path);
            Assert.AreEqual<char>('u', source.TypeChar);

            // Network
            source = new PackageSource(@"N;2;\\server\share");
            Assert.AreEqual<SourceTypes>(SourceTypes.Network, source.SourceType);
            Assert.AreEqual<int>(2, source.Index);
            Assert.AreEqual<string>(@"\\server\share", source.Path);
            Assert.AreEqual<char>('n', source.TypeChar);

            // Media
            source = new PackageSource(@"m;3;Product Name");
            Assert.AreEqual<SourceTypes>(SourceTypes.Media, source.SourceType);
            Assert.AreEqual<int>(3, source.Index);
            Assert.AreEqual<string>(@"Product Name", source.Path);
            Assert.AreEqual<char>('m', source.TypeChar);

            // Invalid
            try
            {
                source = new PackageSource(@"n;a;\\server\share");
                Assert.Fail("Expected PSArgumentException was not thrown.");
            }
            catch (PSArgumentException ex)
            {
                Assert.AreEqual<string>(@"The string ""n;a;\\server\share"" does not specify a valid source.", ex.Message);
            }
        }

        /// <summary>
        /// A test for <see cref="PackageSource.PackageSource"/>
        /// </summary>
        [TestMethod]
        [Description("A test for PackageSource.PackageSource")]
        public void CreateTest()
        {
            PackageSource source;

            // Valid
            source = new PackageSource(SourceTypes.Network, 2, @"\\server\share");
            Assert.AreEqual<SourceTypes>(SourceTypes.Network, source.SourceType);
            Assert.AreEqual<int>(2, source.Index);
            Assert.AreEqual<string>(@"\\server\share", source.Path);
            Assert.AreEqual<char>('n', source.TypeChar);

            // Invalid source type
            try
            {
                source = new PackageSource(SourceTypes.None, 2, @"\\server\share");
                Assert.Fail("Expected PSNotSupportedException was not thrown.");
            }
            catch (NotSupportedException ex)
            {
                Assert.AreEqual<string>(@"The type None is not a valid source type.", ex.Message);
            }

            // Invalid index
            try
            {
                source = new PackageSource(SourceTypes.Network, -1, @"\\server\share");
                Assert.Fail("Expected PSArgumentOutOfRangeException was not thrown.");
            }
            catch (PSArgumentOutOfRangeException ex)
            {
                Assert.AreEqual<string>("index", ex.ParamName);
            }

            // Invalid path
            try
            {
                source = new PackageSource(SourceTypes.Network, 2, null);
                Assert.Fail("Expected PSArgumentNullException was not thrown.");
            }
            catch (PSArgumentNullException ex)
            {
                Assert.AreEqual<string>("path", ex.ParamName);
            }
        }

        /// <summary>
        /// A test for <see cref="PackageSource.ToString"/>.
        /// </summary>
        [TestMethod]
        [Description("A test for PackageSource.ToString")]
        public void ToStringTest()
        {
            PackageSource source = new PackageSource(SourceTypes.Network, 2, @"\\server\share");
            Assert.AreEqual<string>(@"n;2;\\server\share", source.ToString());
        }
    }
}
