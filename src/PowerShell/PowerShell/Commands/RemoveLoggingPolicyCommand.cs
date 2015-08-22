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
