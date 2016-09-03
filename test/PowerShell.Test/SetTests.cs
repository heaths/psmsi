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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Tests for the <see cref="Set&lt;T&gt;"/> class.
    /// </summary>
    [TestClass]
    public sealed class SetTests : TestBase
    {
        [TestMethod]
        public void DefaultSet()
        {
            var set = new Set<string>();
            set.Add("A");
            set.Add("B");
            set.Add("a");

            Assert.AreEqual<int>(3, set.Count, "The number of items in the set are not correct.");
        }

        [TestMethod]
        public void CaseInsensitiveSet()
        {
            var set = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            set.Add("A");
            set.Add("B");
            set.Add("a");

            Assert.AreEqual<int>(2, set.Count, "The number of items in the set are not correct.");
        }
    }
}
