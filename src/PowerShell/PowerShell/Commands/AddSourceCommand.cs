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

using Microsoft.Tools.WindowsInstaller.Properties;
using System.IO;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Cmdlet to register a source path to a product or patch.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "MSISource")]
    public sealed class AddSourceCommand : SourcePathCommandBase
    {
        private static readonly string ErrorId = "DirectoryNotFound";

        /// <summary>
        /// Registers a source path to a product or patch.
        /// </summary>
        protected override void EndProcessing()
        {
            foreach (var param in base.PreviousParameters)
            {
                var installation = base.GetInstallation(param);
                if (null != installation)
                {
                    foreach (var path in param.Paths)
                    {
                        if (this.Validate(path))
                        {
                            installation.SourceList.Add(path);
                        }
                    }

                    if (this.PassThru)
                    {
                        base.WriteSourceList(installation);
                    }
                }
            }
        }

        private bool Validate(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                var message = string.Format(Resources.Error_InvalidDirectory, path);
                var ex = new DirectoryNotFoundException(message);
                var error = new ErrorRecord(ex, AddSourceCommand.ErrorId, ErrorCategory.ResourceUnavailable, path);

                base.WriteError(error);

                return false;
            }
        }
    }
}
