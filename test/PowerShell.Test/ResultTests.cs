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

namespace Microsoft.Tools.WindowsInstaller
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void CannotResetResultRebootInitiated()
        {
            var result = new Result();
            Assert.IsFalse(result.RebootInitiated);

            result.RebootInitiated = true;
            Assert.IsTrue(result.RebootInitiated);

            result.RebootInitiated = false;
            Assert.IsTrue(result.RebootInitiated);
        }

        [TestMethod]
        public void CannotResetResultRebootRequired()
        {
            var result = new Result();
            Assert.IsFalse(result.RebootRequired);

            result.RebootRequired = true;
            Assert.IsTrue(result.RebootRequired);

            result.RebootRequired = false;
            Assert.IsTrue(result.RebootRequired);
        }

        [TestMethod]
        public void ResultRebootInitiatedAndRequired()
        {
            var result = new Result();
            Assert.IsFalse(result.RebootInitiated);
            Assert.IsFalse(result.RebootRequired);

            result.RebootInitiated = true;
            Assert.IsTrue(result.RebootInitiated);
            Assert.IsTrue(result.RebootRequired);
        }

        [TestMethod]
        public void ResultCombineDefault()
        {
            var x = new Result();
            var y = new Result();

            x |= y;
            Assert.IsFalse(x.RebootInitiated);
            Assert.IsFalse(x.RebootRequired);
        }

        [TestMethod]
        public void ResultCombineTrue()
        {
            var x = new Result();
            var y = new Result()
            {
                RebootInitiated = true,
            };

            x |= y;
            Assert.IsTrue(x.RebootInitiated);
            Assert.IsTrue(x.RebootRequired);
        }
    }
}
