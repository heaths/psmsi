<#
The MIT License (MIT)

Copyright (c) Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
#>

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
