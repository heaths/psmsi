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
