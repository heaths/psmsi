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
    /// The Get-MSILoggingPolicy cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSILoggingPolicy")]
    [OutputType(typeof(string), typeof(string[]))]
    public sealed class GetLoggingPolicyCommand : LoggingPolicyCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetLoggingPolicyCommand"/> class.
        /// </summary>
        public GetLoggingPolicyCommand()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLoggingPolicyCommand"/> class.
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
