// Windows Installer help methods and enumerations
//
// Author: Heath Stewart (heaths@microsoft.com)
// Created: Wed, 31 Jan 2007 07:11:59 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Microsoft.Windows.Installer
{
	static class Msi
	{
		internal const string DLL = "msi.dll";

        // Non-localized string constants
        internal const string MsiPackage = "Package";
        internal const string MsiPatch = "Patch";
        internal const string MsiTransform = "Transform";

        static object syncRoot = new object();
		static int major;
		static int minor;

		internal static bool CheckVersion(int major, int minor)
		{
			return CheckVersion(major, minor, false);
		}

		internal static bool CheckVersion(int major, int minor, bool throwOtherwise)
		{
			if (0 == Msi.major)
			{
                lock (Msi.syncRoot)
                {
                    if (0 == Msi.major)
                    {
                        NativeMethods.DLLVERSIONINFO dvi = new NativeMethods.DLLVERSIONINFO();
                        if (NativeMethods.ERROR_SUCCESS == NativeMethods.MsiDllGetVersion(dvi))
                        {
                            Msi.major = dvi.dwMajorVersion;
                            Msi.minor = dvi.dwMinorVersion;
                        }
                    }
                }
			}

			bool check = Msi.major > major ||
				(Msi.major == major && Msi.minor >= minor);

			if (throwOtherwise && !check)
			{
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.MsiRequiredVersion, major, minor));
			}

			return check;
		}
	}

    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), Flags]
	public enum InstallContext : int
	{
		None = 0,
		UserManaged = 1,
		UserUnmanaged = 2,
		Machine = 4,
		All = UserManaged | UserUnmanaged | Machine
	}

	public enum AssignmentType : int
	{
		User = 0,
		Machine = 1
	}

	public enum InstanceType : int
	{
		None = 0,
		Instance = 1
	}

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum ProductState : int
	{
		Advertised = 1,
		Installed = 5
	}

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
	public enum PatchStates : int
	{
		Invalid = 0,
		Applied = 1,
		Superseded = 2,
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Obsoleted")]
        Obsoleted = 4,
		Registered = 8,
		All = Applied | Superseded | Obsoleted | Registered
	}

	public enum InstallType : int
	{
		Default = 0,
		Network = 1,
		SingleInstance = 2
	}

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags"), Flags]
	public enum InstallMessages : int
	{
		FatalExit = 0,
		Error = 0x01000000,
		Warning = 0x02000000,
		User = 0x03000000,
		Info = 0x04000000,
		FilesInUse = 0x05000000,
		ResolveSource = 0x06000000,
		OutOfDiskSpace = 0x07000000,
		ActionStart = 0x08000000,
		ActionData = 0x09000000,
		Progress = 0x0a000000,
		CommonData = 0x0b000000,
		Initialize = 0x0c000000,
		Terminate = 0x0d000000,
		ShowDialog = 0x0e000000,
		RMFilesInUse = 0x19000000
	}

	[Flags]
	public enum InstallLogModes : int
	{
		FatalExit = 1 << (InstallMessages.FatalExit >> 24),
        Error = 1 << (InstallMessages.Error >> 24),
        Warning = 1 << (InstallMessages.Warning >> 24),
        User = 1 << (InstallMessages.User >> 24),
        Info = 1 << (InstallMessages.Info >> 24),
        ResolveSource = 1 << (InstallMessages.ResolveSource >> 24),
        OutOfDiskSpace = 1 << (InstallMessages.OutOfDiskSpace >> 24),
        ActionStart = 1 << (InstallMessages.ActionStart >> 24),
        ActionData = 1 << (InstallMessages.ActionData >> 24),
        CommonData = 1 << (InstallMessages.CommonData >> 24),
        PropertyDump = 1 << (InstallMessages.Progress >> 24),
        Verbose = 1 << (InstallMessages.Initialize >> 24),
        ExtraDebug = 1 << (InstallMessages.Terminate >> 24),
        LogOnlyOnError = 1 << (InstallMessages.ShowDialog >> 24),
        Progress = 1 << (InstallMessages.Progress >> 24),
        Initialize = 1 << (InstallMessages.Initialize >> 24),
        Terminate = 1 << (InstallMessages.Terminate >> 24),
        ShowDialog = 1 << (InstallMessages.ShowDialog >> 24),
        FilesInUse = 1 << (InstallMessages.FilesInUse >> 24),
        RMFilesInUse = 1 << (InstallMessages.RMFilesInUse >> 24)
	}

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
	public enum InstallUILevels : int
	{
		NoChange = 0,
		Default = 1,
		None = 2,
		Basic = 3,
		Reduced = 4,
		Full = 5,
		EndDialog = 0x80,
		ProgressOnly = 0x40,
		HideCancel = 0x20,
		SourceResOnly = 0x100
	}

	[Flags]
	public enum SourceTypes : int
	{
		None = 0,
		Network = 1,
		Url = 2,
		Media = 4,
		All = Network | Url | Media
	}

    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum Code : int
	{
		Product = 0,
        Patch = 0x40000000
	}

    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
    public enum DBOpen : int
    {
        ReadOnly = 0,
        Transact,
        Direct,
        Create,
        CreateDirect,
        PatchFile = 32
    }
}
