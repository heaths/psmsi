// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Management.Automation;

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
        /// Initializes the <see cref="ViewManager"/>.
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
                var columns = ViewManager.GetColumns(view).Select(column => column.Key);
                var properties = new PSPropertySet("DefaultDisplayPropertySet", columns);

                memberSet = new PSMemberSet("PSStandardMembers");
                memberSet.Members.Add(properties);

                memberSets[view.QueryString] = new WeakReference(memberSet);
            }

            return memberSet;
        }
    }
}
