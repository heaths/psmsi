// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Gets information about Windows Installer packages.
    /// </summary>
    internal static class PackageInfo
    {
        private static readonly string FileSizeQuery = "SELECT `FileSize` FROM `File`";
        private static readonly string ReserveCostSizeQuery = "SELECT `ReserveLocal` FROM `ReserveCost`";

        /// <summary>
        /// Gets the default weight based on a small sampling of machine states.
        /// </summary>
        /// <remarks>
        /// The average weight did actually turn out to be 42 MB which is further proof of its significance.
        /// </remarks>
        public const long DefaultWeight = 42 * 1024 * 1024;

        /// <summary>
        /// Gets the weight of a package given its path.
        /// </summary>
        /// <param name="path">The path to the package.</param>
        /// <returns>The weight of a package given its path or 0 if the package is missing.</returns>
        internal static long GetWeightFromPath(string path)
        {
            if (File.Exists(path))
            {
                var weight = 0L;

                using (var db = new Database(path, DatabaseOpenMode.ReadOnly))
                {
                    // Get the total size of all files in the package.
                    if (null != db.Tables["File"])
                    {
                        weight += db.ExecuteIntegerQuery(PackageInfo.FileSizeQuery).Sum(i => i);
                    }

                    // TODO: Should the weight of the registry be taken into account?
                    // Storage isn't documented but it's probably a safe assumption value names and string data are double byte.

                    // Sum up reserve costs for local installs (source installs are uncommon).
                    if (null != db.Tables["ReserveCost"])
                    {
                        weight += db.ExecuteIntegerQuery(PackageInfo.ReserveCostSizeQuery).Sum(i => i);
                    }
                }

                // Use weight of package. May include just custom actions.
                if (0 >= weight)
                {
                    var file = new FileInfo(path);
                    weight = file.Length;
                }

                return weight;
            }

            return 0;
        }

        /// <summary>
        /// Gets the weight of a package given its ProductCode.
        /// </summary>
        /// <param name="productCode">The ProductCode of the product to query.</param>
        /// <param name="userSid">The optional user SID of the product to query. The default is null.</param>
        /// <param name="context">The optional context of the product ot query. The default is <see cref="UserContexts.All"/>.</param>
        /// <returns>The weight of a package given its ProductCode or 0 if the product is not installed or the package missing.</returns>
        internal static long GetWeightFromProductCode(string productCode, string userSid = null, UserContexts context = UserContexts.All)
        {
            var product = new ProductInstallation(productCode, userSid, context);
            if (null != product && product.IsInstalled)
            {
                var path = product.LocalPackage;
                if (!string.IsNullOrEmpty(path))
                {
                    return PackageInfo.GetWeightFromPath(path);
                }
            }

            return 0;
        }
    }
}
