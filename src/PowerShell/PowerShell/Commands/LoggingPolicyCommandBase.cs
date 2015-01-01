// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Win32;
using System;
using System.Management.Automation;

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
        /// Creates a new instance of the <see cref="LoggingPolicyCommandBase"/> class using the given <see cref="ILoggingPolicyService"/>.
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
