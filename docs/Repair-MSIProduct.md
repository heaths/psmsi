---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Repair-MSIProduct

## SYNOPSIS
Repairs or modifies a product package.

## SYNTAX

### Path (Default)
```
Repair-MSIProduct [-ReinstallMode <ReinstallModes>] [-PassThru] [-Path] <String[]> [-Log <String>]
 [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### Product
```
Repair-MSIProduct [-ReinstallMode <ReinstallModes>] [-PassThru] -ProductCode <String[]> [-Log <String>]
 [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### Installation
```
Repair-MSIProduct [-ReinstallMode <ReinstallModes>] [-PassThru] -Product <ProductInstallation[]>
 [-Log <String>] [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### LiteralPath
```
Repair-MSIProduct [-ReinstallMode <ReinstallModes>] [-PassThru] -LiteralPath <String[]> [-Log <String>]
 [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

## DESCRIPTION
By default, simply repairs an existing product.
This cmdlet can also add or remove features, patches, or even uninstall but there are specialized cmdlets for that.

Progress, warnings, and errors during the install are sent through the pipeline making this command fully integrated.

## EXAMPLES

### EXAMPLE 1
```
repair-msiproduct -productcode {12341234-1234-1234-1234-123412341234} -reinstall "pecmsu" -log $env:TEMP\repair.log
```

Repair the specified product using REINSTALLMODE="pecmsu" and log to the TEMP directory.

### EXAMPLE 2
```
get-msiproductinfo -name *TEST* | repair-msiproduct -chain
```

Repair all products with ProductName matching *TEST* and show a single progress bar for the entire operation.

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
The path to a product package to repair.
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

### -Path
The path to a product package to repair.
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

### -Product
An existing product to repair or modify.

```yaml
Type: ProductInstallation[]
Parameter Sets: Installation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ProductCode
The ProductCode of an existing product to repair or modify.

```yaml
Type: String[]
Parameter Sets: Product
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Properties
Additional property=value pairs to pass during repair.

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

### -ReinstallMode
The REINSTALLMODE to use.
You can specify the value as a string in the format used by Windows Installer.
The default is equivalent to "omus".

```yaml
Type: ReinstallModes
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: FileOlderVersion,MachineData,UserData,Shortcut
Accept pipeline input: True (ByPropertyName)
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## NOTES

## RELATED LINKS

[Install-MSIProduct](install-msiproduct)

[Uninstall-MSIProduct](uninstall-msiproduct)

