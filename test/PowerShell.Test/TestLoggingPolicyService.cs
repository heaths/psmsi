// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;

namespace Microsoft.Tools.WindowsInstaller
{
    internal class TestLoggingPolicyService : ILoggingPolicyService
    {
        internal Func<string> GetLoggingPolicyAction { get; set; }
        internal Action<string> SetLoggingPolicyAction { get; set; }
        internal Action RemoveLoggingPolicyAction { get; set; }

        public string GetLoggingPolicy()
        {
            var handler = this.GetLoggingPolicyAction;
            if (null != handler)
            {
                return handler();
            }

            throw new NotImplementedException();
        }

        public void SetLoggingPolicy(string value)
        {
            var handler = this.SetLoggingPolicyAction;
            if (null != handler)
            {
                handler(value);
                return;
            }

            throw new NotImplementedException();
        }

        public void RemoveLoggingPolicy()
        {
            var handler = this.RemoveLoggingPolicyAction;
            if (null != handler)
            {
                handler();
                return;
            }

            throw new NotImplementedException();
        }
    }
}
