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

using System;
using System.Management.Automation;
using Microsoft.Win32;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for Windows Installer logging cmdlets.
    /// </summary>
    public abstract class LoggingPolicyCommandBase : Cmdlet, ILoggingPolicyService
    {
        /// <summary>
        /// Gets the <see cref="LoggingConverter"/> to use for conversions.
        /// </summary>
        internal static readonly LoggingPoliciesConverter LoggingConverter = new LoggingPoliciesConverter();

        private static readonly string DebugPolicy = "Debug";
        private static readonly string LoggingPolicy = "Logging";
        private static readonly string PolicyKey = @"Software\Policies\Microsoft\Windows\Installer";

        private readonly ILoggingPolicyService loggingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingPolicyCommandBase"/> class.
        /// </summary>
        /// <param name="loggingService">The <see cref="ILoggingPolicyService"/> to use. The default is the current instance.</param>
        internal LoggingPolicyCommandBase(ILoggingPolicyService loggingService = null)
        {
            this.loggingService = loggingService ?? this;
        }

        /// <summary>
        /// Gets the logging policy.
        /// </summary>
        /// <returns>The system logging policy.</returns>
        protected string GetPolicy()
        {
            return this.loggingService.GetLoggingPolicy();
        }

        /// <summary>
        /// Sets the logging policy
        /// </summary>
        /// <param name="value">The logging policy to set.</param>
        protected void SetPolicy(string value)
        {
            this.loggingService.SetLoggingPolicy(value);
        }

        /// <summary>
        /// Removes the logging policy.
        /// </summary>
        protected void RemovePolicy()
        {
            this.loggingService.RemoveLoggingPolicy();
        }

        /// <summary>
        /// Writes out the <see cref="LoggingPolicies"/> which are set individually to the pipeline.
        /// </summary>
        /// <param name="modes">The <see cref="LoggingPolicies"/> which are set.</param>
        protected void WriteEnumValues(LoggingPolicies modes)
        {
            foreach (LoggingPolicies mode in Enum.GetValues(typeof(LoggingPolicies)))
            {
                if (0 != (mode & modes) && LoggingPolicies.All != mode)
                {
                    // Return the values as strings so they can be sorted and easily used with -contains.
                    base.WriteObject(mode.ToString());
                }
            }
        }

        private RegistryKey CreatePolicyKey()
        {
            return Registry.LocalMachine.CreateSubKey(LoggingPolicyCommandBase.PolicyKey);
        }

        private RegistryKey OpenPolicyKey(bool writable = false)
        {
            return Registry.LocalMachine.OpenSubKey(LoggingPolicyCommandBase.PolicyKey, writable);
        }

        string ILoggingPolicyService.GetLoggingPolicy()
        {
            var policy = this.OpenPolicyKey();
            if (null != policy)
            {
                using (policy)
                {
                    return policy.GetValue(LoggingPolicyCommandBase.LoggingPolicy) as string;
                }
            }

            return null;
        }

        void ILoggingPolicyService.SetLoggingPolicy(string value)
        {
            var policy = this.CreatePolicyKey();
            if (null != policy)
            {
                using (policy)
                {
                    if (0 <= value.IndexOf('x'))
                    {
                        policy.SetValue(LoggingPolicyCommandBase.DebugPolicy, 7);
                    }

                    policy.SetValue(LoggingPolicyCommandBase.LoggingPolicy, value);
                }
            }
        }

        void ILoggingPolicyService.RemoveLoggingPolicy()
        {
            var policy = this.OpenPolicyKey(true);
            if (null != policy)
            {
                using (policy)
                {
                    policy.DeleteValue(LoggingPolicyCommandBase.DebugPolicy);
                    policy.DeleteValue(LoggingPolicyCommandBase.LoggingPolicy);
                }
            }
        }
    }
}
