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
    [TestClass]
    public sealed class LoggingPoliciesConverterTests : TestBase
    {
        private const LoggingPolicies Default = LoggingPoliciesConverter.Normal;
        private static readonly string DefaultString = Enum.Format(typeof(LoggingPolicies), LoggingPoliciesConverterTests.Default, "g");

        [TestMethod]
        public void ConvertLoggingModesToShortForm()
        {
            var converter = new LoggingPoliciesConverter();
            Assert.IsTrue(converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(converter.CanConvertTo(this.GetType()));

            var mode = (string)converter.ConvertTo(LoggingPoliciesConverterTests.Default, typeof(string));
            CollectionAssert.AreEquivalent("oicewarmup".ToArray(), mode.ToArray());

            mode = (string)converter.ConvertTo(LoggingPoliciesConverterTests.Default | LoggingPolicies.FlushEachLine, typeof(string));
            CollectionAssert.AreEquivalent("oicewarmup!".ToArray(), mode.ToArray());
        }

        [TestMethod]
        public void ConvertNamesToLoggingModes()
        {
            var converter = new LoggingPoliciesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)));
            Assert.IsFalse(converter.CanConvertFrom(this.GetType()));

            var mode = (LoggingPolicies)converter.ConvertFrom(LoggingPoliciesConverterTests.DefaultString);
            Assert.AreEqual(LoggingPoliciesConverterTests.Default, mode);

            // Used mixed casing to test default enumeration parsing.
            mode = (LoggingPolicies)converter.ConvertFrom(LoggingPoliciesConverterTests.DefaultString + ", flushEACHline");
            Assert.AreEqual(LoggingPoliciesConverterTests.Default | LoggingPolicies.FlushEachLine, mode);
        }

        [TestMethod]
        public void ConvertShortFormToLoggingModes()
        {
            var converter = new LoggingPoliciesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)));
            Assert.IsFalse(converter.CanConvertFrom(this.GetType()));

            var mode = (LoggingPolicies)converter.ConvertFrom("oiceWARMUP");
            Assert.AreEqual<LoggingPolicies>(Default, mode);

            mode = (LoggingPolicies)converter.ConvertFrom("*vx");
            Assert.AreEqual<LoggingPolicies>(LoggingPolicies.All, mode);

            mode = (LoggingPolicies)converter.ConvertFrom("*vx!");
            Assert.AreEqual<LoggingPolicies>(LoggingPolicies.All | LoggingPolicies.FlushEachLine, mode);
        }

        [TestMethod]
        public void ConvertInvalidStringToLoggingModes()
        {
            var converter = new LoggingPoliciesConverter();
            Assert.IsTrue(converter.CanConvertFrom(typeof(string)));
            Assert.IsFalse(converter.CanConvertFrom(this.GetType()));

            // The append logging command line option is not supported in the registry policy.
            ExceptionAssert.Throws<ArgumentException>(() => { var mode = (LoggingPolicies)converter.ConvertFrom("*vx!+"); });

            // Bogus command line options.
            ExceptionAssert.Throws<ArgumentException>(() => { var mode = (LoggingPolicies)converter.ConvertFrom("jkl"); });
        }
    }
}
