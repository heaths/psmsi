<#
.Synopsis
	Exports wiki content from cmdlet help to text files.

.Description
	Help content is extracted for each cmdlet to a text file with text formatted for the CodePlex wiki.

.Parameter Path
	A path or paths to the help files.

.Parameter Version
	The version to use in filenames for the wiki.

.Parameter OutDirectory
	The directory where the output files are created.

.Example
	PS> export-help.ps1 -path Microsoft.WindowsInstaller.PowerShell.dll-help.xml

.Link
	get-content
	set-content
#>

#Requires -Version 2.0

param(
	[Parameter(Position=0, Mandatory=$true)]
	[string[]] $Path,

	[Parameter(Position=1)]
	[int] $Version = 2,

	[Parameter(Position=2)]
	[string] $OutDirectory = $([System.IO.Path]::GetTempPath())
)

process {

	[xml] $doc = get-content $Path
	$doc.helpItems.command | foreach-object -begin {

		$xslt = new-object -type System.Xml.Xsl.XslCompiledTransform
		$xargs = new-object -type System.Xml.Xsl.XsltArgumentList
		
		$invocation = $(get-variable MyInvocation -scope 0).Value
		$stylesheet = join-path $(split-path $invocation.MyCommand.Path) "Help.xslt"

		$xslt.Load($stylesheet);

		$xargs.AddParam("version", "", $Version);

	} -process {

		$reader = new-object -type System.Xml.XmlNodeReader -argumentlist $_
		$writer = new-object -type System.IO.StringWriter

		$xslt.Transform($reader, $xargs, $writer);

		$page = join-path $OutDirectory $("v{0}_{1}.txt" -f $Version, $_.details.name)
		$writer.ToString() | set-content -path $page -encoding "UTF8"
		
		$page

	}

	$doc.helpItems.command | foreach-object -begin {

		$line = "! Help`n!! Cmdlets"

	} -process {

		$line += $("`n* [{1}|v{0}_{1}]" -f $Version, $_.details.name)

	} -end {

		$page = join-path $OutDirectory $("v{0}_Help.txt" -f $Version)
		$line | set-content -path $page -encoding "UTF8"

		$page

	}
}
