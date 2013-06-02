[CmdletBinding()]
[OutputType([string])]
param
(
    [Parameter(Position = 0)]
    [ValidateRange(1, [int]::MaxValue)]
    [int] $Count = 1
)

begin
{
    1..$Count | foreach-object {

        [Guid]::NewGuid().ToString("B").ToUpper([Globalization.CultureInfo]::InvariantCulture)
    }
}

<#
.Synopsis
Creates GUIDs in a format fit for Windows Installer.

.Description
Windows Installer requires that GUIDs consist of numbers and upper-case
characters, and that the GUIDs are enclosed within curly brackets. This command
creates such GUIDs for use in diagnostics and testing.

.Parameter Count
The number of GUIDs to create.
#>
