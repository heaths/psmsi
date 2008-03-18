// Various enumerators for Windows Installer APIs.
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Sat, 13 Jan 2007 06:11:21 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Windows.Installer
{
    delegate int MsiEnumCallback<T>(int index, out T data);

    class MsiEnumerable<T> : IEnumerable<T>
    {
        MsiEnumCallback<T> callback;

        public MsiEnumerable(MsiEnumCallback<T> callback)
        {
            Debug.Assert(null != callback);
            this.callback = callback;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MsiEnumerator<T>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // Inner generic enumerator class that works with various Windows
        // Installer APIs, such as those beginning with "MsiEnum*" that all
        // work the same way but require different data.
        class MsiEnumerator<K> : IEnumerator<K>
        {
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")] MsiEnumerable<K> outer;
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")] K data;
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")] int i = 0;

            internal MsiEnumerator(MsiEnumerable<K> outer)
            {
                this.outer = outer;
            }

            public K Current
            {
                get { return this.data; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return (object)this.data; }
            }

            public bool MoveNext()
            {
                int ret = this.outer.callback(this.i++, out this.data);
                switch (ret)
                {
                    // Good cases.
                    case NativeMethods.ERROR_SUCCESS:
                        return true;

                    case NativeMethods.ERROR_NO_MORE_ITEMS:
                        return false;

                    // Throw and terminate the pipeline.
                    default:
                        throw new Win32Exception(ret);
                }
            }

            public void Reset()
            {
                this.i = 0;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
