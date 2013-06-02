[CmdletBinding()]
[OutputType([string])]
param
(
    [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
    [ValidatePattern('^(?<b>\{)?[A-F0-9]{8}-([A-F0-9]{4}-){3}[A-F0-9]{12}(?(b)\})$')]
    [string[]] $Guid
)

begin
{
    $Translation = 7,6,5,4,3,2,1,0,11,10,9,8,15,14,13,12,17,16,19,18,21,20,23,22,25,24,27,26,29,28,31,30
}

process
{
    $Guid | foreach-object {

        $SquishedGuid = ""

        $x = $_ -replace "[{}-]", ""
        for ($i = 0; $i -lt $x.Length; ++$i)
        {
            $SquishedGuid += $x[$Translation[$i]]
        }

        $SquishedGuid
    }
}

<#
.Synopsis
Converts GUIDs to Windows Installer squished GUIDs.

.Description
Windows Installer does not store GUIDs as you might normally see them.
Instead, they are stored in a string representation of their native structure
format. This command converts GUIDs to these squished GUIDs for use in
diagnostics and testing.

.Parameter Guid
The GUID to convert to a squished GUID.
#>
