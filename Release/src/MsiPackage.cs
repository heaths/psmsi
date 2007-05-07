// Represents an MSI package, which may be an MSI, MSM, PCP, MST, or MSP file.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Tue, 20 Mar 2007 06:32:33 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Management.Automation;

namespace Microsoft.Windows.Installer
{
	public class PackageSource
	{
		internal PackageSource(string source)
		{
			string[] fields = source.Split(';');
			try
			{
				// Get the type.
				switch (fields[0].ToLower())
				{
					case "u": type = SourceType.URL; break;
					case "n": type = SourceType.Network; break;
					case "m": type = SourceType.Media; break;
					default: throw new PSNotSupportedException();
				}

				// Get the index.
				index = int.Parse(fields[1]);

				// Get the path.
				path = fields[2];
			}
			catch (Exception ex)
			{
				throw new PSArgumentException(Properties.Resources.Argument_InvalidSource, ex);
			}
		}

		internal PackageSource(SourceType type, int index, string path)
		{
			if (type != SourceType.Network && type != SourceType.URL && type != SourceType.Media)
			{
				throw new PSNotSupportedException(Properties.Resources.Argument_InvalidSourceType);
			}
			if (index < 0) throw new PSArgumentOutOfRangeException("index");
			if (string.IsNullOrEmpty(path)) throw new PSArgumentNullException("path");

			this.type = type;
			this.index = index;
			this.path = path;
		}
		
		public override string ToString()
		{
			// Faster than string.Format.
			return string.Join(";", new string[] { TypeChar.ToString(), Index.ToString(), Path });
		}

		public SourceType Type
		{
			get { return type; }
		}
		SourceType type;

		public int Index
		{
			get { return index; }
		}
		int index;

		public string Path
		{
			get { return path; }
		}
		string path;

		internal char TypeChar
		{
			get
			{
				switch (type)
				{
					case SourceType.Network: return 'n';
					case SourceType.URL: return 'u';
					case SourceType.Media: return 'm';
					default: throw new PSNotSupportedException(Properties.Resources.Argument_InvalidSourceType);
				}
			}
		}
	}
}
