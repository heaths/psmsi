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
