// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// A function that takes a single argument of type <typeparamref name="T"/> and returns an object of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input argument.</typeparam>
    /// <typeparam name="TResult">The type of the return object.</typeparam>
    /// <param name="arg">The input argument.</param>
    /// <returns>An object of type <typeparamref name="TResult"/>.</returns>
    internal delegate TResult Func<T, TResult>(T arg);

    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The type of the elements of source.</param>
        /// <param name="predicate">The condition to apply to each element.</param>
        /// <returns>True if any of the elements satisfy the <paramref name="predicate"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> or <paramref name="predicate"/> argument is null.</exception>
        internal static bool Any<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (null == predicate)
            {
                throw new ArgumentNullException("selector");
            }

            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the input sequence or 0 if <paramref name="source"/> is null.</returns>
        internal static long Count<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source)
            {
                return 0;
            }

            var coll = source as ICollection<TSource>;
            if (null != coll)
            {
                return coll.Count;
            }

            var count = 0L;
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns the first item of an enumeration; otherwise, if not found, the default value for type <typeparamref name="TSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type being enumerated.</typeparam>
        /// <param name="source">An enumeration of type <typeparamref name="TSource"/>.</param>
        /// <returns>the first item of an enumeration; otherwise, if not found, the default value for type <typeparamref name="TSource"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> argument is null.</exception>
        internal static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            var list = source as IList<TSource>;
            if (null != list && 0 < list.Count)
            {
                return list[0];
            }
            else
            {
                using (var e = source.GetEnumerator())
                {
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                }
            }

            return default(TSource);
        }

        /// <summary>
        /// Joins an enumerable of <see cref="String"/> elements with the given <paramref name="separator"/>.
        /// </summary>
        /// <param name="source">An enumerable of type <see cref="String"/>.</param>
        /// <param name="separator">The string separator to separate <see cref="String"/> elements. The default is an empty string.</param>
        /// <returns>A string of all elements separated by the given <paramref name="separator"/>, or null if <paramref name="source"/> is null.</returns>
        internal static string Join(this IEnumerable<string> source, string separator)
        {
            if (null == source)
            {
                return null;
            }

            var array = source as string[];
            if (null != array)
            {
                return string.Join(separator, array);
            }

            var sb = new StringBuilder();
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    sb.Append(e.Current);
                }

                while (e.MoveNext())
                {
                    sb.Append(separator ?? string.Empty);
                    sb.Append(e.Current);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> whose elements are the result of invoking the transform function on each element of source..</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> or <paramref name="selector"/> argument is null.</exception>
        internal static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (null == selector)
            {
                throw new ArgumentNullException("selector");
            }

            // Project each item in a separate method or null checks above are compiled away.
            return ExtensionMethods.SelectIterator(source, selector);
        }

        /// <summary>
        /// Reeturns the sum of the field selected from each item in the source enumerable.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values from which fields are selected to sum.</param>
        /// <param name="selector">A transform function to apply to each element to get the field to sum.</param>
        /// <returns>The sum of the field selected from each item in the source enumerable.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> or <paramref name="selector"/> argument is null.</exception>
        internal static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (null == selector)
            {
                throw new ArgumentNullException("selector");
            }

            var sum = 0L;
            foreach (long i in ExtensionMethods.SelectIterator(source, selector))
            {
                sum += i;
            }

            return sum;
        }

        /// <summary>
        /// Creates an array from the enumerable <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An enumerable from which the array is created.</param>
        /// <returns>An array that contains the elements from the input <paramref name="source"/>.</returns>
        internal static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            var list = new List<TSource>(source);
            return list.ToArray();
        }

        /// <summary>
        /// Createsa new list from the enumerable <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An enumerable from which the array is created.</param>
        /// <returns>A list that contains the elements from the input <paramref name="source"/>.</returns>
        internal static IList<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            return new List<TSource>(source);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The type of the elements of source.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An enumerable that contains elements from the input sequence that satisfy the condition.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> or <paramref name="predicate"/> argument is null.</exception>
        internal static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }
            else if (null == predicate)
            {
                throw new ArgumentNullException("selector");
            }

            // Project each item in a separate method or null checks above are compiled away.
            return ExtensionMethods.WhereIterator(source, predicate);
        }

        private static IEnumerable<TResult> SelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            foreach (var element in source)
            {
                yield return selector(element);
            }
        }

        private static IEnumerable<TSource> WhereIterator<TSource>(IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    yield return element;
                }
            }
        }
    }
}
