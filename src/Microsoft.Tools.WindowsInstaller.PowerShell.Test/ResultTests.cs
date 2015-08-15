// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
