// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Returns the first item of an enumeration; otherwise, if not found, the default value for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type being enumerated.</typeparam>
        /// <param name="source">An enumeration of type <typeparamref name="T"/>.</param>
        /// <returns>the first item of an enumeration; otherwise, if not found, the default value for type <typeparamref name="T"/>.</returns>
        internal static T FirstOrDefault<T>(this IEnumerable<T> source)
        {
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            var list = source as IList<T>;
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

            return default(T);
        }
    }
}
