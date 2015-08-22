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

using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIFileType cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIFileType", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(string), typeof(System.IO.FileInfo), typeof(DirectoryInfo))]
    public sealed class GetFileTypeCommand : ItemCommandBase
    {
        /// <summary>
        /// Gets or sets whether the file objects are returned.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Processes the item enumerated by the base class.
        /// </summary>
        /// <param name="item">The <see cref="PSObject"/> to process.</param>
        protected override void ProcessItem(PSObject item)
        {
            if (this.PassThru)
            {
                this.WriteObject(item);
            }
            else if (null != item.Properties["MSIFileType"])
            {
                try
                {
                    // Return the file type from ETS.
                    this.WriteObject(item.Properties["MSIFileType"].Value);
                }
                catch (GetValueInvocationException ex)
                {
                    this.WriteError(ex.ErrorRecord);
                }
                catch (PSNotSupportedException ex)
                {
                    this.WriteError(ex.ErrorRecord);
                }
            }
        }
    }
}
