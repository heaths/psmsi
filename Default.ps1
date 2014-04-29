# Copyright (C) Microsoft Corporation. All rights reserved.
#
# THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
# KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
# IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
# PARTICULAR PURPOSE.

Properties {
    $Configuration = 'Debug'
    $Script:MSBuild = 'MSBuild'
    $Script:MSTest = 'MSTest'
    $Script:NuGet = 'NuGet'
    $SolutionDir = Resolve-Path .
    $SolutionFile = Join-Path $SolutionDir 'Psmsi.sln' -Resolve
    $SourceDir = Join-Path $SolutionDir 'src' -Resolve
    $Script:Version = '2.3.0.0'
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
        $Keys = 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\Setup\VS', 'HKLM:\SOFTWARE\Microsoft\VisualStudio\12.0\Setup\VS',
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

    exec { & "$MSBuild" "$SolutionFile" /m /t:Build /p:Configuration="$Configuration" }
}

Task Document -Alias Doc -Depends Compile {
    $HelpToolsDir = Join-Path $SolutionDir 'tools\help'
    assert (Test-Path $HelpToolsDir) 'Help tools not found. Did you run "git submodule update --init"?'

    $Project = 'Microsoft.Tools.WindowsInstaller.PowerShell'
    $ProjectDir = Join-Path $SourceDir $Project
    $OutputDir = Join-Path $SourceDir "$Project\bin\$Configuration"
    $ModulePath = Join-Path $OutputDir 'MSI.psd1'
    $IntermediateDir = Join-Path $SourceDir "$Project\obj\$Configuration"

    Import-Module (Join-Path $HelpToolsDir 'Help.psd1')

    $ProjectHelp = Join-Path $ProjectDir 'Help.xml'
    $IntermediateHelp = Join-Path $IntermediateDir 'Help.xml'
    Write-Host "Converting $ProjectHelp into $IntermediateHelp"
    ConvertTo-Help -Module $ModulePath -Path $IntermediateHelp -TemplatePath $ProjectHelp

    $OutputHelp = Join-Path $ProjectDir "$Project.dll-Help.xml"
    Write-Host "Formatting $IntermediateHelp into MAML help file $OutputHelp"
    Format-Help -Path $OutputHelp -ContentPath $IntermediateHelp -Transform (Join-Path $HelpToolsDir 'maml.xslt')

    $OutputHelp = Join-Path $OutputDir 'codeplex\codeplex.txt'
    Write-Host "Formatting $IntermediateHelp into CodePlex help file $OutputHelp"
    Format-Help4CodePlex -Path $OutputHelp -ContentPath $IntermediateHelp -Version $Version
}

Task Clean {
    assert ($Configuration.Length) "Must specify `$Configuration"
    assert (Get-Command $MSBuild -ea SilentlyContinue).Length "Must specify location of `$MSBuild"

    exec { & "$MSBuild" "$SolutionFile" /m /t:Clean /p:Configuration="$Configuration" }
}

Task Test -Depends Compile {
    $MSTest
    assert (Get-Command $MSTest -ea SilentlyContinue).Length "Must specify location of `$MSTest"

    $Projects = 'Microsoft.Tools.WindowsInstaller.PowerShell.Test'
    $CommandLine = $Projects | ForEach-Object { "/testcontainer:$SourceDir\$_\bin\$Configuration\$_.dll " }

    exec { & "$MSTest" $CommandLine }
}

Task Package -Alias Pack -Depends Compile {
    assert (Get-Command 'NuGet' -ea SilentlyContinue).Length "Must specify location of `$NuGet"

    $Projects = 'Microsoft.Tools.WindowsInstaller.PowerShell'

    $Projects | ForEach-Object {
        $Project = Join-Path $SourceDir "$_\$_.csproj" -Resolve
        $OutputDir = Join-Path $SourceDir "$_\bin\$Configuration"
        exec { & "$NuGet" pack "$Project" -OutputDirectory $OutputDir -Version $Version -Properties "Configuration=$Configuration" -Symbols }
    }
}

Task Publish -Depends Package {
    assert (Get-Command 'NuGet' -ea SilentlyContinue).Length "Must specify location of `$NuGet"
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
