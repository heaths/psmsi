// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Cmdlet to remove a registered source path from a product or patch.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "MSISource")]
    public sealed class RemoveSourceCommand : SourcePathCommandBase
    {
        /// <summary>
        /// Removes a registered source path from a product or patch.
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
                        installation.SourceList.Remove(path);
                    }

                    if (this.PassThru)
                    {
                        base.WriteSourceList(installation);
                    }
                }
            }
        }
    }
}
