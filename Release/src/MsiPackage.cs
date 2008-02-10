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
using System.Globalization;

namespace Microsoft.Windows.Installer
{
	public class PackageSource
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PackageSource(string source)
		{
			string[] fields = source.Split(';');
			try
			{
				// Get the type.
				switch (fields[0].ToLowerInvariant())
				{
					case "u": sourceType = SourceTypes.Url; break;
					case "n": sourceType = SourceTypes.Network; break;
					case "m": sourceType = SourceTypes.Media; break;
					default: throw new PSNotSupportedException();
				}

				// Get the index.
				index = int.Parse(fields[1], CultureInfo.InvariantCulture);

				// Get the path.
				path = fields[2];
			}
			catch (Exception ex)
			{
				throw new PSArgumentException(Properties.Resources.Argument_InvalidSource, ex);
			}
		}

		internal PackageSource(SourceTypes type, int index, string path)
		{
			if (type != SourceTypes.Network && type != SourceTypes.Url && type != SourceTypes.Media)
			{
				throw new PSNotSupportedException(Properties.Resources.Argument_InvalidSourceType);
			}
			if (index < 0) throw new PSArgumentOutOfRangeException("index");
			if (string.IsNullOrEmpty(path)) throw new PSArgumentNullException("path");

			this.sourceType = type;
			this.index = index;
			this.path = path;
		}
		
		public override string ToString()
		{
			// Faster than string.Format.
			return string.Join(";", new string[] { TypeChar.ToString(), Index.ToString(CultureInfo.InvariantCulture), Path });
		}

		public SourceTypes SourceType
		{
			get { return sourceType; }
		}
		SourceTypes sourceType;

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
				switch (sourceType)
				{
					case SourceTypes.Network: return 'n';
					case SourceTypes.Url: return 'u';
					case SourceTypes.Media: return 'm';
					default: throw new PSNotSupportedException(Properties.Resources.Argument_InvalidSourceType);
				}
			}
		}
	}
}
