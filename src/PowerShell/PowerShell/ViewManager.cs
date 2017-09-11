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
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Deployment.WindowsInstaller;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Creates and caches an instance of a <see cref="ColumnCollection"/> for each <see cref="View"/>.
    /// </summary>
    internal static class ViewManager
    {
        // Weak reference allows a column collection or property sets to be GC'd it no records are alive that need it.
        private static Dictionary<string, WeakReference> views;
        private static Dictionary<string, WeakReference> memberSets;

        /// <summary>
        /// Initializes static members of the <see cref="ViewManager"/> class.
        /// </summary>
        static ViewManager()
        {
            views = new Dictionary<string, WeakReference>(StringComparer.OrdinalIgnoreCase);
            memberSets = new Dictionary<string, WeakReference>(StringComparer.OrdinalIgnoreCase);
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

        /// <summary>
        /// Creates and caches an instance of the "PSStandardMembers" <see cref="PSMemberSet"/> to attach to a <see cref="Record"/>.
        /// </summary>
        /// <param name="view">The <see cref="View"/> from which column information is retrieved.</param>
        /// <returns>A new or cached copy of the "PSStandardMembers" <see cref="PSMemberSet"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> parameter value is null.</exception>
        internal static PSMemberSet GetMemberSet(View view)
        {
            if (null == view)
            {
                throw new ArgumentNullException("view");
            }

            PSMemberSet memberSet;
            if (memberSets.ContainsKey(view.QueryString) && memberSets[view.QueryString].IsAlive)
            {
                // Get an existing PSMemberSet.
                memberSet = (PSMemberSet)memberSets[view.QueryString].Target;
            }
            else
            {
                // Add or set a new PSMemberSet.
                memberSet = new PSMemberSet("PSStandardMembers");

                var columns = ViewManager.GetColumns(view).Select(column => column.Key);
                var properties = new PSPropertySet("DefaultDisplayPropertySet", columns);
                memberSet.Members.Add(properties);

                columns = ViewManager.GetColumns(view).PrimaryKeys;
                if (0 < columns.Count())
                {
                    properties = new PSPropertySet("DefaultKeyPropertySet", columns);
                    memberSet.Members.Add(properties);
                }

                memberSets[view.QueryString] = new WeakReference(memberSet);
            }

            return memberSet;
        }
    }
}
