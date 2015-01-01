# Copyright (C) Microsoft Corporation. All rights reserved.
#
# THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
# KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
# IMPLIED WARRANTIES OF MERCHANTABILITY ANDOR FITNESS FOR A
# PARTICULAR PURPOSE.

try
{
    $InstallPath = Split-Path (Split-Path $MyInvocation.MyCommand.Definition)
    $PSModulePath = ([Environment]::GetEnvironmentVariable('PSModulePath', 'User') -split ';' | where-Object { $_ -ne $InstallPath }) -join ';'

    Install-ChocolateyEnvironmentVariable 'PSModulePath' $PSModulePath -VariableType 'User'
    Write-ChocolateySuccess 'psmsi'
}
catch
{
    Write-ChocolateyFailure 'psmsi' "$($_.Exception.Message)"
    throw
}
