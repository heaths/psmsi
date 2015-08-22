# Windows Installer PowerShell Module

Exposes Windows Installer functionality to PowerShell, providing means to query installed product and patch information and to query views on packages.

## Description

[PowerShell](http://www.microsoft.com/powershell) is a powerful command shell that pipes objects - not just text. Because of this ability, you can string practically unrelated commands together in many different ways to work on different types of objects, all built on .NET. You can use all the properties and methods of those objects passed through the pipeline instead of being limited by the text sent to you as with traditional command shells.

 This Windows Installer module for PowerShell provides cmdlets ("command-lets") - similar to functions - to query package states, patches registered and applied to products, and more. You can use it to query Windows Installer products and patches installed on your system.

``` powershell
get-msiproductinfo | where { $_.Name -like '*Visual Studio*' }
```

You can even use it to determine which products installed a particular file on your system.

``` powershell
get-msicomponentinfo `
    | where { $_.Path -like 'C:\Program Files\*\Common7\IDE\devenv.exe'} `
    | get-msiproductinfo
```

You can also install, repair, and uninstall products and patches complete with progress information, and warnings and errors direct to the pipeline.

``` powershell
install-msiproduct .\example.msi -destination (join-path $env:ProgramFiles Example)
```

## License

The Windows Installer PowerShell module is licensed under the [MIT License](LICENSE.txt).

## Status

[![Build status](https://ci.appveyor.com/api/projects/status/251twb4wvywq0vei?svg=true)](https://ci.appveyor.com/project/heaths/psmsi)
