---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIFileHash

## SYNOPSIS
Gets a hash of a file in a Windows Installer-compatible format.

## SYNTAX

### Path (Default)
```
Get-MSIFileHash [-PassThru] [[-Path] <String[]>] [<CommonParameters>]
```

### LiteralPath
```
Get-MSIFileHash [-PassThru] -LiteralPath <String[]> [<CommonParameters>]
```

## DESCRIPTION
Get-MSIFileHash returns a 128-bit file hash in 4 separate parts, compatible with columns in the MsiFileHash table in Windows Installer packages.
All non-versioned files should contain this hash.

You can optionally add these HashPart1, HashPart2, HashPart3, and HashPart4 properties to FileSystem items.

## EXAMPLES

### EXAMPLE 1
```
get-msifilehash -path * | format-table -auto
```

This command outputs the file hash of every file in the current directory as a table.

### EXAMPLE 2
```
get-childitem | where-object {$_.PSIsContainer -eq $False} | get-msifilehash -passthru | format-table Name, MSI* -auto
```

This command outputs the name and hash parts of each file in the current directory.

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
Passes the item or items passed into this cmdlet through the pipeline with additional properties for the file hash.

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

### Microsoft.Tools.WindowsInstaller.FileHash

### System.IO.DirectoryInfo

### System.IO.FileInfo

## NOTES

## RELATED LINKS

[Get-MSIFileType](get-msifilehash)

