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

using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The data for actions to install patches.
    /// </summary>
    public class InstallPatchActionData : InstallCommandActionData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstallPatchActionData"/> class.
        /// </summary>
        public InstallPatchActionData()
        {
            this.Patches = new List<string>();
        }

        /// <summary>
        /// Gets the list of patch paths to apply.
        /// </summary>
        public List<string> Patches { get; private set; }

        /// <summary>
        /// Gets or sets the user security identifier for the product to which patches apply.
        /// </summary>
        public string UserSid { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserContexts"/> for the product to which patches apply.
        /// </summary>
        public UserContexts UserContext { get; set; }
    }
}
