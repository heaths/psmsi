---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Measure-MSIProduct

## SYNOPSIS
Gets drive costs for components that would be installed to any drive.

## SYNTAX

### Path (Default)
```
Measure-MSIProduct [-Destination <String>] [-Properties <String[]>] [-Path] <String[]> [-Patch <String[]>]
 [-Transform <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Measure-MSIProduct [-Destination <String>] [-Properties <String[]>] [-Patch <String[]>] [-Transform <String[]>]
 -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
You can use this command to get the costs for components that will be installed to any drive mounted to the system.
If multiple product packages are specified the total costs for all components in all specified products are returned.

## EXAMPLES

### EXAMPLE 1
```
measure-msiproduct .\example.msi -patch .\example.msp -target X:\Example
```

Gets the drive costs for example.msi with example.msp applied if installed to the X:\Example directory.

## PARAMETERS

### -Destination
The target directory where the product should be installed.

Note that the product package must be authored to support installing to TARGETDIR.

```yaml
Type: String
Parameter Sets: (All)
Aliases: TargetDirectory

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
The path to a product package to measure.
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

### -Patch
The path to a patch package or packages to apply to the product package before measuring.
Patches are applied in sequence order.

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

### -Path
The path to a product package to measure.
Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Properties
Additional properties to pass to the session.

Note that you can mark all features for installation using ADDLOCAL=ALL or set public directories using this parameter.

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

### -Transform
The path to a transform or transforms to apply to the product package before measuring.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### System.Management.Automation.PSDriveInfo

## NOTES

## RELATED LINKS
