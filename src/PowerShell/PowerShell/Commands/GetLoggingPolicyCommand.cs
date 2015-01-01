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
    /// The Get-MSILoggingPolicy cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSILoggingPolicy")]
    [OutputType(typeof(string), typeof(string[]))]
    public sealed class GetLoggingPolicyCommand : LoggingPolicyCommandBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GetLoggingPolicyCommand"/> class.
        /// </summary>
        public GetLoggingPolicyCommand()
            : base(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GetLoggingPolicyCommand"/> class with the given <see cref="ILoggingPolicyService"/>.
        /// </summary>
        /// <param name="loggingService">The <see cref="ILoggingPolicyService"/> to use. The default is the current instance.</param>
        internal GetLoggingPolicyCommand(ILoggingPolicyService loggingService = null)
            : base(loggingService)
        {
        }

        /// <summary>
        /// Gets or sets whether to retrieve the raw regisry value.
        /// </summary>
        [Parameter]
        public SwitchParameter Raw { get; set; }

        /// <summary>
        /// Gets the current logging policy if set.
        /// </summary>
        protected override void BeginProcessing()
        {
            var policy = base.GetPolicy();
            if (!string.IsNullOrEmpty(policy))
            {
                var converter = LoggingPolicyCommandBase.LoggingConverter;

                if (this.Raw)
                {
                    base.WriteObject(policy);
                }
                else if (null != policy && converter.CanConvertFrom(policy.GetType()))
                {
                    var values = (LoggingPolicies)converter.ConvertFromString(policy);
                    base.WriteEnumValues(values);
                }
            }
        }
    }
}
