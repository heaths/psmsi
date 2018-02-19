---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Edit-MSIPackage

## SYNOPSIS
Opens an install package or patch in Orca or another registered editor.

## SYNTAX

### Path (Default)
```
Edit-MSIPackage [-Wait] [[-Path] <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Edit-MSIPackage [-Wait] -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
Orca can be installed from the Windows SDK.
If installed, MSI and MSP packages can be opened in Orca.
If Orca is not installed, any application registered with the "Edit" verb for .msi or .msp file extensions is used.

## EXAMPLES

### EXAMPLE 1
```
get-childitem -filter *.msi -recurse | edit-msipackage
```

Opens all install packages in the current directory or subdirectories in separate instances of Orca.

## PARAMETERS

### -LiteralPath
The path to a package to open.
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
The path to a package to open.
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

### -Wait
Wait until the process is closed before opening another package.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
