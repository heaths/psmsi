---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIFileType

## SYNOPSIS
Gets the Windows Installer file type.

## SYNTAX

### Path (Default)
```
Get-MSIFileType [-PassThru] [[-Path] <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Get-MSIFileType [-PassThru] -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
Gets the Windows Installer file type for a given file or files.

You can optionally add this MSIFileType property to FileSystem items.

## EXAMPLES

### EXAMPLE 1
```
get-msifilehash -path $env:WINDIR\Installer
```

This command outputs the file type of files in the Windows Installer cache directory.

### EXAMPLE 2
```
get-childitem -path $env:WINDIR\Installer\* | where-object {$_.PSIsContainer -eq $False} | get-msifiletype -passthru | format-table Name, MSIFileType -auto
```

This command outputs the Windows Installer file type for files in the Windows Installer cache directory.

## PARAMETERS

### -LiteralPath
The path to the item or items which must resolve to a file system path.
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
Passes the item or items passed into this cmdlet through the pipeline with the additional property for the file type.

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
The path to the item or items which must resolve to a file system path.
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

### string

### System.IO.DirectoryInfo

### System.IO.FileInfo

## NOTES

## RELATED LINKS

[Get-MSIFileHash](get-msifilehash)

