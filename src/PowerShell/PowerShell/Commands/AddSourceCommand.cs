// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

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
