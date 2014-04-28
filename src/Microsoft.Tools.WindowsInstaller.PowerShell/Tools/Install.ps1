# Copyright (C) Microsoft Corporation. All rights reserved.
#
# THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
# KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
# IMPLIED WARRANTIES OF MERCHANTABILITY ANDOR FITNESS FOR A
# PARTICULAR PURPOSE.

param($installPath, $toolsPath, $package, $project)

Write-Host "This is not a package with libraries to reference in it."
Write-Host "Please install using chocolatey'."
Write-Host "To get chocolatey just run 'Install-Package chocolatey' followed by 'Initialize-Chocolatey'"
Write-Host "chocolatey install psmsi"

Write-Host "Removing this package..."
Uninstall-Package psmsi -ProjectName $project.Name
