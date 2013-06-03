// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Creates and caches an instance of a <see cref="ColumnCollection"/> for each <see cref="View"/>.
    /// </summary>
    internal static class ViewManager
    {
        // Weak reference allows a column collection to be GC'd it no records are alive that need it.
        private static Dictionary<string, WeakReference> views;

        /// <summary>
        /// Initializes the <see cref="ViewManager"/>.
        /// </summary>
        static ViewManager()
        {
            views = new Dictionary<string, WeakReference>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Creates and caches an instance of a <see cref="ColumnCollection"/> for a <see cref="View"/>.
        /// </summary>
        /// <param name="view">The <see cref="View"/> from which column information is retrieved.</param>
        /// <returns>A new or cached copy of columns for the <see cref="View.QueryString"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> parameter value is null.</exception>
        internal static ColumnCollection GetColumns(View view)
        {
            if (null == view)
            {
                throw new ArgumentNullException("view");
            }

            ColumnCollection columns;
            if (views.ContainsKey(view.QueryString) && views[view.QueryString].IsAlive)
            {
                // Get an existing collection.
                columns = (ColumnCollection)views[view.QueryString].Target;
            }
            else
            {
                // Add or set a new column collection;
                columns = new ColumnCollection(view);
                views[view.QueryString] = new WeakReference(columns);
            }

            return columns;
        }
    }
}
