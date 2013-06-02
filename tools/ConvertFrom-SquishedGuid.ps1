[CmdletBinding()]
[OutputType([string])]
param
(
    [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
    [ValidatePattern('^[A-F0-9]{32}$')]
    [string[]] $SquishedGuid
)

begin
{
    $Translation = 7,6,5,4,3,2,1,0,11,10,9,8,15,14,13,12,17,16,19,18,21,20,23,22,25,24,27,26,29,28,31,30
}

process
{
    $SquishedGuid | foreach-object {

        $Guid = ""

        for ($i = 0; $i -lt $_.Length; ++$i)
        {
            if (8,12,16,20 -contains $i)
            {
                $Guid += '-'
            }

            $Guid += $_[$Translation[$i]]
        }

        '{' + $Guid + '}'
    }
}

<#
.Synopsis
Converts Windows Installer squished GUIDs to GUIDs.

.Description
Windows Installer does not store GUIDs as you might normally see them.
Instead, they are stored in a string representation of their native structure
format. This command converts these squished GUIDs to GUIDs for use in
diagnostics and testing.

.Parameter SquishedGuid
The squished GUID to convert to a GUID.
#>
