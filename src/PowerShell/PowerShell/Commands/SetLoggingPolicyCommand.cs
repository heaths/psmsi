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
    [Cmdlet(VerbsCommon.Set, "MSILoggingPolicy")]
    [OutputType(typeof(string), typeof(string[]))]
    public sealed class SetLoggingPolicyCommand : LoggingPolicyCommandBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SetLoggingPolicyCommand"/> class.
        /// </summary>
        public SetLoggingPolicyCommand()
            : base(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SetLoggingPolicyCommand"/> class with the given <see cref="ILoggingPolicyService"/>.
        /// </summary>
        /// <param name="loggingService">The <see cref="ILoggingPolicyService"/> to use. The default is the current instance.</param>
        internal SetLoggingPolicyCommand(ILoggingPolicyService loggingService = null)
            : base(loggingService)
        {
        }

        /// <summary>
        /// Gets or sets the logging policy.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        [LoggingPolicies]
        public LoggingPolicies LoggingPolicy { get; set; }

        /// <summary>
        /// Gets or sets whether to pass the new registry value back through the pipeline.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Gets or sets whether to retrieve the raw regisry value.
        /// </summary>
        [Parameter]
        public SwitchParameter Raw { get; set; }

        /// <summary>
        /// Sets the logging policy.
        /// </summary>
        protected override void BeginProcessing()
        {
            var converter = LoggingPolicyCommandBase.LoggingConverter;
            if (converter.CanConvertTo(typeof(string)))
            {
                var policy = converter.ConvertToInvariantString(this.LoggingPolicy);
                base.SetPolicy(policy);

                if (this.PassThru)
                {
                    if (this.Raw)
                    {
                        this.WriteObject(policy);
                    }
                    else
                    {
                        base.WriteEnumValues(this.LoggingPolicy);
                    }
                }
            }
        }
    }
}
