---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Test-MSIProduct

## SYNOPSIS
Runs internal consistency evaluators (ICEs) against the product package or packages.

## SYNTAX

### Path (Default)
```
Test-MSIProduct [-AdditionalCube <String[]>] [-NoDefault] [-Include <String[]>] [-Exclude <String[]>]
 [-Path] <String[]> [-Patch <String[]>] [-Transform <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Test-MSIProduct [-AdditionalCube <String[]>] [-NoDefault] [-Include <String[]>] [-Exclude <String[]>]
 [-Patch <String[]>] [-Transform <String[]>] -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
Internal consistency evaluators (ICEs) are custom actions that validate whether a product is authored as specified.
If Orca or MsiVal2 is installed, the default ICE .cub ("ICE cube") file is used by default, and you can specify custom ICE cube.

You can also apply any number of patches or transforms before running ICEs.
This allows you to evaluate a product that is transformed by, for example, a patch to also make sure the changes made by a patch or valid.

## EXAMPLES

### EXAMPLE 1
```
test-msiproduct .\example.msi -include ICE0* -exclude ICE03 -v
```

Output all messages from ICEs 01 through 09 except for ICE03.

### EXAMPLE 2
```
test-msiproduct .\example.msi -patch .\example.msp -add .\tests\custom.cub
```

Apply example.msp to example.msi, then run all the default and custom ICEs.

## PARAMETERS

### -AdditionalCube
One or more ICE .cub files to use for evaluation.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: Cube

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Exclude
The names of ICEs to exclude from evaluation (all other ICEs are included).

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Include
The names of ICEs to include from evaluation (all other ICEs are excluded).

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -LiteralPath
The path to a package to evaluate.
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

### -NoDefault
Do not import darice.cub if installed with Orca or MsiVal2.

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
The path to a patch package or packages to apply to the product package before evaluation.
Patches are applied in sequence order.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Path
The path to a package to evaluate.
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
Accept wildcard characters: True
```

### -Transform
The path to a transform or transforms to apply to the product package before evaluation.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### Microsoft.Tools.WindowsInstaller.IceMessage

## NOTES

## RELATED LINKS
