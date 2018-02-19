# Contributing to the Windows Installer PowerShell Module

Thank you for contributing to improve this module.

## Required software

You'll need to download and install the following software to build this project.

* [NuGet](https://nuget.org) - an older version can be found in the .nuget subdirectory.
* [Microsoft Visual Studio](https://www.visualstudio.com) 2015 or newer.
* Microsoft Modeling SDK
  * [Microsoft Modeling SDK for Visual Studio 2015](https://www.microsoft.com/download/details.aspx?id=48148)
  * Microsoft Modeling SDK for Visual Studio 2017 is an optional component under "Visual Studio extension development".
* Optional: [platyPS](https://github.com/PowerShell/platyPS) - needed only for Release builds or to update help markdown.
* Optional: [Windows Installer XML](http://wixtoolset.org) - needed only for Release builds.

## Building

You can build the _Psmsi.sln_ in the project root using MSBuild or Visual Studio. By default, the Debug configuration is built and will not build the setup project. The Release configuration will build both setup project and nuget package.

To build the setup project and nuget package you will need to first build the help documentation from markdown.

```powershell
Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force
Install-Module -Name platyPS -Repository PSGallery -SkipPublisherCheck -Force
New-ExternalHelp -Path .\docs -OutputPath .\src\PowerShell\bin\Release
```

Builds must produce no errors or warnings. PR and CI builds will ensure this.

## Testing

You should run the tests in Test Explorer from within Visual Studio or using _vstest.console.exe_ before submitting a PR. Some tests will install a basic product named "Test" but will attempt to uninstall it when completed.

Tests must all pass. PR and CI buils will ensure this.
