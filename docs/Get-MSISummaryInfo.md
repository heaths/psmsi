---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSISummaryInfo

## SYNOPSIS
Gets the summary information from a product or patch package, or from a transform.

## SYNTAX

### Path (Default)
```
Get-MSISummaryInfo [-IncludeTransforms] [[-Path] <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Get-MSISummaryInfo [-IncludeTransforms] -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
The summary information stream is used by Windows Installer to determine applicability, version requirements, and more.
Use this command to view the summary information for a product or patch package, or a transform.
The properties returned are adapted for each file type.

## EXAMPLES

### EXAMPLE 1
```
get-childitem -filter *.ms* | get-msisummaryinfo
```

Gets summary information for any file matching *.ms*, including .msi, .msp, and .mst packages.

### EXAMPLE 2
```
get-msisummaryinfo *.msp -includetransforms
```

Gets the patch and embedded transform summary information.

## PARAMETERS

### -IncludeTransforms
Whether to enumerate the transforms within a patch package.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: Transforms

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
The path to a package or transform to open.
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

### -Path
The path to a package or transform to open.
Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.Package.TransformInfo

### Microsoft.Tools.WindowsInstaller.SummaryInfo

## NOTES

## RELATED LINKS
