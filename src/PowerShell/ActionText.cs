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
