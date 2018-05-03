---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Install-MSIPatch

## SYNOPSIS
Installs a patch package or packages for all or only specified products.

## SYNTAX

### Path (Default)
```
Install-MSIPatch [-PassThru] [-ProductCode <String[]>] [-UserContext <UserContexts>] [-UserSid <String>]
 [-Path] <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>]
 [<CommonParameters>]
```

### Installation
```
Install-MSIPatch [-PassThru] -Patch <PatchInstallation[]> [-ProductCode <String[]>]
 [-UserContext <UserContexts>] [-UserSid <String>] [-Log <String>] [-Properties <String[]>] [-Chain] [-Force]
 [-ResultVariable <String>] [<CommonParameters>]
```

### LiteralPath
```
Install-MSIPatch [-PassThru] [-ProductCode <String[]>] [-UserContext <UserContexts>] [-UserSid <String>]
 -LiteralPath <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>]
 [<CommonParameters>]
```

## DESCRIPTION
You can install one or more patch packages to all installed targets products or to just a subset of products.

Progress, warnings, and errors during the install are sent through the pipeline making this command fully integrated.

## EXAMPLES

### EXAMPLE 1
```
install-msipatch .\example.msp MSIFASTINSTALL=2
```

Install the example.msp patch package for all installed target products with MSIFASTINSTALL set to only do file costing before installation.

### EXAMPLE 2
```
get-msiproductinfo -name TEST | install-msipatch .\example.msp -log $env:TEMP\patch.log -passthru
```

Install the example.msp patch package only for the product with ProductName "TEST" and log to the TEMP directory.

Return information about the patch after logging.

## PARAMETERS

### -Chain
Whether to install all packages together.
If elevated, a single restore point is created for all packages in the chain and reboots are suppressed when possible.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Whether to suppress all prompts.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
The path to a patch package to install.
The value of -LiteralPath is used exactly as typed.
No characters are interpreted as wildcards.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Log
The path to the log file.
This use the file name as the base name and will append timestamp and product-specific information.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Whether to pass the newly installed patch information after installation to the pipeline.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Patch
Information about a patch or patches to install to other products.

```yaml
Type: PatchInstallation[]
Parameter Sets: Installation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Path
The path to a patch package to install.
Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -ProductCode
The ProductCode or ProductCodes to which the patch or patches should be applied.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Properties
Additional property=value pairs to pass during install.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ResultVariable
The name of a variable to store operation results.
Optionally prefix with "+" to combine results with existing results variable.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserContext
The user context for the product listed in the ProductCode parameter.

```yaml
Type: UserContexts
Parameter Sets: (All)
Aliases: Context, InstallContext

Required: False
Position: Named
Default value: Machine
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UserSid
The user security identifier for product listed in the ProductCode parameter.

```yaml
Type: String
Parameter Sets: (All)
Aliases: User

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.PatchInstallation

## NOTES

## RELATED LINKS

[Uninstall-MSIPatch](uninstall-msipatch)

