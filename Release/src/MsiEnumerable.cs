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
			if (callback == null) throw new ArgumentNullException("callback");
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
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            MsiEnumerable<K> outer;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            K data;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            int i = 0;
	
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
				if (ret == Msi.ERROR_SUCCESS) return true;
				else if (ret == Msi.ERROR_NO_MORE_ITEMS) return false;
				else
					// Something catastrophic happened, so throw.
					throw new System.ComponentModel.Win32Exception(ret);
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
