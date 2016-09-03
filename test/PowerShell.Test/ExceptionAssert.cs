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
using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Methods for asserting exceptions in unit tests.
    /// </summary>
    /// <remarks>
    /// This class comes in handy for asserting several exception cases without a test method for each case.
    /// </remarks>
    public static class ExceptionAssert
    {
        /// <summary>
        /// Asserts that an exception of type <typeparamref name="T"/> was thrown.
        /// </summary>
        /// <typeparam name="T">The exception type that should be thrown.</typeparam>
        /// <param name="action">The <see cref="Action"/> that should thrown an exception of <typeparamref name="T"/>.</param>
        public static void Throws<T>(Action action)
            where T : Exception
        {
            Contract.Requires(null != action);
            ExceptionAssert.Throws(typeof(T), null, action);
        }

        /// <summary>
        /// Asserts that an exception of type <typeparamref name="T"/> was thrown containing inner exception of type <typeparamref name="TInner"/>.
        /// </summary>
        /// <typeparam name="T">The exception type that should be thrown.</typeparam>
        /// <typeparam name="TInner">The inner exception type that should be contained by the exception of type <typeparamref name="T"/>.</typeparam>
        /// <param name="action">The <see cref="Action"/> that should thrown an exception of <typeparamref name="T"/>.</param>
        public static void Throws<T, TInner>(Action action)
            where T : Exception
            where TInner : Exception
        {
            Contract.Requires(null != action);
            ExceptionAssert.Throws(typeof(T), typeof(TInner), action);
        }

        private static void Throws(Type outerException, Type innerException, Action action)
        {
            Contract.Requires(null != outerException);
            Contract.Requires(null != action);

            try
            {
                action.Invoke();

                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Exception of type {0} was not thrown.",
                    outerException.FullName
                );

                Assert.Fail(message);
            }
            catch (Exception ex)
            {
                if (!outerException.IsAssignableFrom(ex.GetType()))
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception of type {0} was expected, but an exception of type {1} was thrown instead.",
                        outerException.FullName,
                        ex.GetType().FullName
                    );

                    Assert.Fail(message);
                }
                else if (null != innerException)
                {
                    if (null == ex.InnerException)
                    {
                        var message = string.Format(
                           CultureInfo.InvariantCulture,
                           "Inner exception of type {0} was not thrown.",
                           innerException.FullName
                       );

                        Assert.Fail(message);
                    }
                    else if (!innerException.IsAssignableFrom(ex.InnerException.GetType()))
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Inner exception of type {0} was expected, but an inner exception of type {1} was thrown instead.",
                            innerException.FullName,
                            ex.InnerException.GetType().FullName
                        );

                        Assert.Fail(message);
                    }
                }
            }
        }
    }
}
