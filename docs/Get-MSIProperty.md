---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIProperty

## SYNOPSIS
Gets properties from a product or patch package.

## SYNTAX

### Path (Default)
```
Get-MSIProperty [[-Property] <String[]>] [-PassThru] [-Path] <String[]> [-Patch <String[]>]
 [-Transform <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Get-MSIProperty [[-Property] <String[]>] [-PassThru] [-Patch <String[]>] [-Transform <String[]>]
 -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
Selects all or matching properties from a product or patch package and either returns them to the pipeline or attaches them to a file object for a product or patch package if -PassThru is specified.
When propertie are attached to a file object you can select them all using the "MSIProperties" property set.

## EXAMPLES

### EXAMPLE 1
```
get-msiproperty Product*, UpgradeCode -path example.msi
```

Gets the identifying properties from the example.msi product package.

### EXAMPLE 2
```
get-childitem -filter *.msi | get-msiproperty -passthru | select Name, MSIProperties
```

Attaches all properties from each product package and shows them all along with the file name.

## PARAMETERS

### -LiteralPath
The path to a product or patch package to open.
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

### -PassThru
Whether to pass the file object back to the pipeline with selected properties attached.

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
The path to a patch package to apply to the product package.
Multiple patches are applied in authored sequence order.

Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

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
The path to a product or patch package to open.
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

### -Property
Optional list of property names to select.
Wildcard are permitted.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Transform
The path to a transform to apply to the product package.

Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

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

### Microsoft.Tools.WindowsInstaller.Record

### System.IO.FileInfo

## NOTES

## RELATED LINKS

[Get-MSITable](get-msitable)

