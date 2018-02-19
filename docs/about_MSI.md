﻿# MSI
## about_MSI

# SHORT DESCRIPTION
Windows Installer PowerShell Module

# LONG DESCRIPTION
Exposes Windows Installer functionality to PowerShell, providing means to
query installed product and patch information and to query views on
packages.

# VARIABLES

```
PS> $MsiAttributeColumnFormat
```

The format specification string to alter how attributes are displayed.

Valid values are:

* G:   Displays the integer value. This is the default.
* X:   Displays the hexadecimal integer value.
* F:   Displays one or more names for well-known attribute types.

# ONLINE
https://github.com/heaths/psmsi/wiki

# LICENSE
https://github.com/heaths/psmsi/blob/develop/LICENSE.txt

# HISTORY
See https://github.com/heaths/psmsi/releases for up-to-date information.

## 2.3.0
This release adds support for per-machine installs of the module.
Administrators may choose this option for enterprise deployment
or to make the module available to remote sessions.

Several performance enhancements were made in addition to bug fixes:
* PSPath property for components only resolves full path when used.
* Export-MSIPatchXml no longer requires full path to patch MSP.
* Get-MSITable can apply patch MSPs to not-installed product MSIs.

Piping components to Get-ChildItem now also works correctly for
file system and registry key paths. To support this behavior, a
KeyPath property was added that contains the non-transformed path
previously contained within the PSPath property.

## 2.2.1
This release is a servicing issue and fixes various issues, notably:
* Returns the correct 32- or 64-bit registry path.
* Module can correctly be loaded into PowerShell v2.
* Older versions of Orca are now supported.
* The Record property adapter is now case-insensitive.

The Get-MSITable cmdlet now also supports applying patches in
sequence order and transforms.

## 2.2.0
This release adds many new cmdlets from the past release, including
cmdlets to select records from a table or custom query, run ICEs on a
package, and install, repair, or uninstall packages. The install,
repair, and uninstall cmdlets also display progress, warning, and
error information. Some issues were fixed as well.

The snap-in installer has been removed in favor of the module
installer - a per-user install that will upgrade any previous version.
You may also install the package to a per-machine location, though
elevation is required. This will make the module accessible to any
host for any user on the machine.
