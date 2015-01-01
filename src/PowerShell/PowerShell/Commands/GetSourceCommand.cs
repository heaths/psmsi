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
    /// Cmdlet to get the source list for a product or patch.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSISource")]
    public sealed class GetSourceCommand : SourceCommandBase
    {
        /// <summary>
        /// Writes the list of source paths or URLs to the pipeline.
        /// </summary>
        protected override void EndProcessing()
        {
            foreach (var param in base.PreviousParameters)
            {
                var installation = base.GetInstallation(param);
                if (null != installation)
                {
                    base.WriteSourceList(installation);
                }
            }
        }
    }
}
