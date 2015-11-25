# The MIT License (MIT)
#
# Copyright (c) Microsoft Corporation
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

Framework '4.0'

Properties {
    $Configuration = 'Debug'
    $Script:MSBuild = 'MSBuild'
    $Script:MSTest = 'MSTest'
    $Script:NuGet = 'NuGet'
    $SolutionDir = Resolve-Path .
    $SolutionFile = Join-Path $SolutionDir 'Psmsi.sln' -Resolve
    $SourceDir = Join-Path $SolutionDir 'src' -Resolve
    $TestDir = Join-Path $SolutionDir 'test' -Resolve
    $Script:Version = '3.0.0.0'
}

Task Default -Depends Compile

TaskSetup {

    if (-not (Get-Command $MSBuild -ea SilentlyContinue))
    {
        Write-Verbose "Looking for location of MSBuild..."
        $Keys = 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\MSBuild', 'HKLM:\SOFTWARE\Microsoft\MSBuild'
        $MSBuild = $Keys | Get-ChildItem -ea SilentlyContinue | Where-Object { $_.PSChildName -match '\d+\.\d+' } `
                         | Get-ItemProperty -Name MSBuildOverrideTasksPath -ea SilentlyContinue `
                         | Where-Object { Test-Path (Join-Path $_.MSBuildOverrideTasksPath 'MSBuild.exe') } `
                         | Select-Object MSBuildOverrideTasksPath -First 1 `
                         | ForEach-Object { Join-Path $_.MSBuildOverrideTasksPath 'MSBuild.exe' }

        if ($MSBuild.Length)
        {
            $Script:MSBuild = $MSBuild
            Write-Verbose "Found MSBuild at '$MSBuild'."
        }
        else
        {
            $MSBuild = 'MSBuild'
            Write-Warning "MSBuild could not be found. Will simply invoke 'MSBuild'."
       }
    }

    if (-not (Get-Command $MSTest -ea SilentlyContinue))
    {
        Write-Verbose "Looking for location of MSTest..."
        $Keys = 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\14.0\Setup\VS', 'HKLM:\SOFTWARE\Microsoft\VisualStudio\14.0\Setup\VS',
                'HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\Setup\VS', 'HKLM:\SOFTWARE\Microsoft\VisualStudio\12.0\Setup\VS',
                'HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\11.0\Setup\VS', 'HKLM:\SOFTWARE\Microsoft\VisualStudio\11.0\Setup\VS'
        $MSTest = $Keys | Get-Item -ea SilentlyContinue | Get-ItemProperty -Name ProductDir -ea SilentlyContinue `
                        | Where-Object { Test-Path (Join-Path $_.ProductDir 'Common7\IDE\MSTest.exe') } `
                        | Select-Object ProductDir -First 1 `
                        | ForEach-Object { Join-Path $_.ProductDir 'Common7\IDE\MSTest.exe' }

        if ($MSTest.Length)
        {
            $Script:MSTest = $MSTest
            Write-Verbose "Found MSTest at '$MSTest'."
        }
        else
        {
            Write-Warning "MSTest could not be found. Will simply invoke 'MSTest'."
        }
    }

    if (-not (Get-Command $NuGet -ea SilentlyContinue))
    {
        $Script:NuGet = Join-Path $SolutionDir '.nuget\NuGet.exe'
    }
}

Task Compile -Alias Build {
    assert ($Configuration.Length) "Must specify `$Configuration"
    assert (Get-Command $MSBuild -ea SilentlyContinue).Length "Must specify location of `$MSBuild"

    exec { & "$MSBuild" /nologo "$SolutionFile" /m /t:Build /p:Configuration="$Configuration" }
}

Task AddCommands -Alias Update -Depends Compile {
    $HelpToolsDir = Join-Path $SolutionDir 'tools\help'
    assert (Test-Path $HelpToolsDir) 'Help tools not found. Did you run "git submodule update --init"?'

    $Project = 'PowerShell'
    $ProjectDir = Join-Path $SourceDir $Project
    $OutputDir = Join-Path $SourceDir "$Project\bin\$Configuration"
    $ModulePath = Join-Path $OutputDir 'MSI.psd1'

    Import-Module (Join-Path $HelpToolsDir 'Help.psd1')

    $ProjectHelp = Join-Path $ProjectDir 'Help.xml'
    Write-Host "Adding commands from $ModulePath into $ProjectHelp"
    Export-HelpTemplate -Module $ModulePath -Path $ProjectHelp
}

Task Document -Alias Doc -Depends Compile {
    $HelpToolsDir = Join-Path $SolutionDir 'tools\help'
    assert (Test-Path $HelpToolsDir) 'Help tools not found. Did you run "git submodule update --init"?'

    $Project = 'PowerShell'
    $AssemblyName = 'Microsoft.Tools.WindowsInstaller.PowerShell'
    $ProjectDir = Join-Path $SourceDir $Project
    $OutputDir = Join-Path $SourceDir "$Project\bin\$Configuration"
    $ModulePath = Join-Path $OutputDir 'MSI.psd1'
    $IntermediateDir = Join-Path $SourceDir "$Project\obj\$Configuration"

    Import-Module (Join-Path $HelpToolsDir 'Help.psd1')

    $ProjectHelp = Join-Path $ProjectDir 'Help.xml'
    $IntermediateHelp = Join-Path $IntermediateDir 'Help.xml'
    Write-Host "Converting $ProjectHelp into $IntermediateHelp"
    ConvertTo-Help -Module $ModulePath -Path $IntermediateHelp -TemplatePath $ProjectHelp

    $OutputHelp = Join-Path $ProjectDir "$AssemblyName.dll-Help.xml"
    Write-Host "Formatting $IntermediateHelp into MAML help file $OutputHelp"
    Format-Help -Path $OutputHelp -ContentPath $IntermediateHelp -Transform (Join-Path $HelpToolsDir 'maml.xslt')

    $OutputHelp = Join-Path $OutputDir 'codeplex\codeplex.txt'
    Write-Host "Formatting $IntermediateHelp into CodePlex help file $OutputHelp"
    Format-Help4CodePlex -Path $OutputHelp -ContentPath $IntermediateHelp -Version $Version
}

Task Clean {
    assert ($Configuration.Length) "Must specify `$Configuration"
    assert (Get-Command $MSBuild -ea SilentlyContinue).Length "Must specify location of `$MSBuild"

    exec { & "$MSBuild" /nologo "$SolutionFile" /m /t:Clean /p:Configuration="$Configuration" }
}

Task Test -Depends Compile {
    assert ($Configuration.Length) "Must specify `$Configuration"
    assert (Get-Command $MSTest -ea SilentlyContinue).Length "Must specify location of `$MSTest"

    $Projects = @{"PowerShell.Test" = 'Microsoft.Tools.WindowsInstaller.PowerShell.Test'}
    $CommandLine = $Projects.GetEnumerator() | ForEach-Object {
        "/testcontainer:$TestDir\$($_.Name)\bin\$Configuration\$($_.Value).dll "
    }

    $Global:LASTEXITCODE = 0
    $Results = & "$MSTest" /nologo $CommandLine /category:"!Impactful"

    $Results | ForEach-Object {
        $Result, $Name = $_ -split ' ', 2

        # Color test results for easy identification.
        $Color = [Console]::ForegroundColor
        if ($Result -eq 'Passed') {
            $Color = [ConsoleColor]::Green
        } elseif ($Result -eq 'Failed') {
            $Color = [ConsoleColor]::Red
        }

        # Strip root prefix from name for compact view.
        $Name = $Name -replace 'Microsoft\.Tools\.WindowsInstaller\.', ''

        if (@('Passed', 'Failed', 'Skipped') -contains $Result) {
            Write-Host "$Result $Name" -ForegroundColor $Color
        } else {
            Write-Host $_
        }
    }

    if ($LASTEXITCODE -ne 0) {
        throw ("Exec: `"$MSTest`" /nologo $CommandLine /category:`"!Impactful`"")
    }
}

function Join-Path
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string] $Path,

        [Parameter(Mandatory=$true, Position=1)]
        [string] $ChildPath,

        [Parameter()]
        [switch] $Resolve
    )

    # Join-Path doesn't support non-existent paths.
    $Path = [System.IO.Path]::Combine($Path, $ChildPath)
    if ($Resolve)
    {
        $Path = Get-Item $Path | Select-Object -Expand FullName
    }

    $Path
}
