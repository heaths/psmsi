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
using System.Globalization;
using System.Resources;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains the localized ActionText descriptions and templates.
    /// </summary>
    internal static class ActionText
    {
        private static volatile object sync = new object();
        private static ResourceManager actionText;
        private static ResourceManager actionData;

        /// <summary>
        /// Gets the ActionText description for the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The name of the action.</param>
        /// <param name="culture">The optional culture for which the ActionText description should be retrieved.</param>
        /// <returns>The ActionText description; otherwise, null if not found.</returns>
        internal static string GetActionText(string action, CultureInfo culture = null)
        {
            var resourceMan = ActionText.GetResourceManager(ref actionText, ResourceSet.ActionText);
            return resourceMan.GetString(action, culture);
        }

        /// <summary>
        /// Gets the ActionText template for the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The name of the action.</param>
        /// <param name="culture">The optional culture for which the ActionText template should be retrieved.</param>
        /// <returns>The ActionText template; otherwise, null if not found.</returns>
        internal static string GetActionData(string action, CultureInfo culture = null)
        {
            var resourceMan = ActionText.GetResourceManager(ref actionData, ResourceSet.ActionData);
            return resourceMan.GetString(action, culture);
        }

        private static ResourceManager GetResourceManager(ref ResourceManager field, ResourceSet name)
        {
            if (null == field)
            {
                lock (sync)
                {
                    if (null == field)
                    {
                        var t = typeof(ActionText);
                        var qualifiedName = t.Namespace + "." + Enum.GetName(typeof(ResourceSet), name);

                        field = new ResourceManager(qualifiedName, t.Assembly);
                    }
                }
            }

            return field;
        }

        private enum ResourceSet
        {
            ActionText,
            ActionData,
        }
    }
}
