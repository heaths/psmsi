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


namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Helps avoid misspellings throughout the code.
    /// </summary>
    internal static class ParameterSet
    {
        /// <summary>
        /// Parameter set for component information.
        /// </summary>
        internal const string Component = "Component";

        /// <summary>
        /// Parameter set for feature information.
        /// </summary>
        internal const string Feature = "Feature";

        /// <summary>
        /// Parameter set for product information.
        /// </summary>
        internal const string Product = "Product";

        /// <summary>
        /// Parameter set for patch information.
        /// </summary>
        internal const string Patch = "Patch";

        /// <summary>
        /// Parameter set for either product or patch information.
        /// </summary>
        internal const string Installation = "Installation";

        /// <summary>
        /// Parameter set for a path supporting wildcards.
        /// </summary>
        internal const string Path = "Path";

        /// <summary>
        /// Parameter set for a literal path.
        /// </summary>
        internal const string LiteralPath = "LiteralPath";

        /// <summary>
        /// Parameter set for a Name parameter.
        /// </summary>
        internal const string Name = "Name";
    }
}
