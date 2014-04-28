Properties {
    $Configuration = 'Debug'
    $MSBuild = 'MSBuild'
    $MSTest = 'MSTest'
    $NuGet = 'NuGet'
    $SolutionDir = Resolve-Path .
    $SolutionFile = Join-Path $SolutionDir 'Psmsi.sln' -Resolve
    $SourceDir = Join-Path $SolutionDir 'src' -Resolve
}

Task Default -Depends Compile

TaskSetup {

    if (-not (Get-Command "$MSBuild" -ea SilentlyContinue))
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
            Write-Verbose "Found MSBuild at '$MSBuild'."
        }
        else
        {
            $MSBuild = 'MSBuild'
            Write-Warning "MSBuild could not be found. Will simply invoke 'MSBuild'."
        }
    }

    if (-not (Get-Command "$MSTest" -ea SilentlyContinue))
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
            Write-Verbose "Found MSTest at '$MSTest'."
        }
        else
        {
            $MSTest = 'MSTest'
            Write-Warning "MSTest could not be found. Will simply invoke 'MSTest'."
        }
    }
}

Task Compile {
    assert ($Configuration.Length) "Must specify `$Configuration"
    assert (Get-Command "$MSBuild" -ea SilentlyContinue) "Must specify location of `$MSBuild"

    exec { & "$MSBuild" "$SolutionFile" /m /t:Build /p:Configuration="$Configuration" }
}

Task Clean {
    assert ($Configuration.Length) "Must specify `$Configuration"
    assert (Get-Command "$MSBuild" -ea SilentlyContinue) "Must specify location of `$MSBuild"

    exec { & "$MSBuild" "$SolutionFile" /m /t:Clean /p:Configuration="$Configuration" }
}

Task Test -Depends Compile {
    assert (Get-Command "$MSTest" -ea SilentlyContinue) "Must specify location of `$MSTest"

    $Projects = 'Microsoft.Tools.WindowsInstaller.PowerShell.Test'
    $CommandLine = $Projects | ForEach-Object { "/testcontainer:$SourceDir\$_\bin\$Configuration\$_.dll " }

    exec { & "$MSTest" $CommandLine }
}

Task Package -Depends Compile {
    assert (Get-Command 'NuGet' -ea SilentlyContinue) "Must specify location of `$NuGet"
}

Task Publish -Depends Package {
    assert (Get-Command 'NuGet' -ea SilentlyContinue) "Must specify location of `$NuGet"
}
