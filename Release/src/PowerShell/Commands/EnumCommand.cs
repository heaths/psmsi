// Cmdlet to get or enumerator Windows Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 06:55:55 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management;
using System.Management.Automation;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
	public abstract class EnumCommand<T> : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			foreach (T obj in new MsiEnumerable<T>(Enumerate))
			{
				WritePSObject(obj);
			}
		}

		protected abstract int Enumerate(int index, out T data);

		protected virtual void WritePSObject(T obj)
		{
			PSObject psobj = PSObject.AsPSObject(obj);
			WriteObject(psobj);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		internal void Debug(string format, params object[] args)
		{
			WriteDebug(string.Format(format, args));
		}
	}
}
