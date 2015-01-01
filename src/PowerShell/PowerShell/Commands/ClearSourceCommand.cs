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
    /// Cmdlets to clear all registered source paths from a product or patch.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "MSISource")]
    public sealed class ClearSourceCommand : SourceCommandBase
    {
        /// <summary>
        /// Clears all registered source paths from a product or patch.
        /// </summary>
        protected override void EndProcessing()
        {
            foreach (var param in base.PreviousParameters)
            {
                var installation = base.GetInstallation(param);
                if (null != installation)
                {
                    installation.SourceList.ClearNetworkSources();
                    installation.SourceList.ClearUrlSources();
                }
            }
        }
    }
}
