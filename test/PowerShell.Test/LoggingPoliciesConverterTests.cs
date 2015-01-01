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
