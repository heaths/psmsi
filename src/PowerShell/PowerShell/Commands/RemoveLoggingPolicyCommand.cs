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
    /// The Set-MSILoggingPolicy cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "MSILoggingPolicy")]
    public sealed class RemoveLoggingPolicyCommand : LoggingPolicyCommandBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RemoveLoggingPolicyCommand"/> class.
        /// </summary>
        public RemoveLoggingPolicyCommand()
            : base(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RemoveLoggingPolicyCommand"/> class with the given <see cref="ILoggingPolicyService"/>.
        /// </summary>
        /// <param name="loggingService">The <see cref="ILoggingPolicyService"/> to use. The default is the current instance.</param>
        internal RemoveLoggingPolicyCommand(ILoggingPolicyService loggingService = null)
            : base(loggingService)
        {
        }

        /// <summary>
        /// Deletes the current logging policy.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.RemovePolicy();
        }
    }
}
