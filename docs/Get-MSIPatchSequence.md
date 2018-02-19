---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIPatchSequence

## SYNOPSIS
Given a list of patches or patch XML, outputs the sequence of applicable patches for a product or products.

## SYNTAX

### Path,PackagePath (Default)
```
Get-MSIPatchSequence [-Path] <String[]> [-PackagePath] <String[]> [<CommonParameters>]
```

### Path,ProductCode
```
Get-MSIPatchSequence [-Path] <String[]> [-ProductCode] <String[]> [-UserContext <UserContexts>]
 [-UserSid <String>] [<CommonParameters>]
```

### LiteralPath,PackagePath
```
Get-MSIPatchSequence -LiteralPath <String[]> [-PackagePath] <String[]> [<CommonParameters>]
```

### LiteralPath,ProductCode
```
Get-MSIPatchSequence -LiteralPath <String[]> [-ProductCode] <String[]> [-UserContext <UserContexts>]
 [-UserSid <String>] [<CommonParameters>]
```

## DESCRIPTION
Patch packages or patch XML files can be specified along with a list of products.
Each patch is added to a list and after all patches specified are processed, the sequence for all applicable patches is output for each product specified.

By default, the table format is used with a grouping for each product specified.

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -LiteralPath
The path to a package to open.
The value of -LiteralPath is used exactly as typed.
No characters are interpreted as wildcards.

```yaml
Type: String[]
Parameter Sets: LiteralPath,PackagePath, LiteralPath,ProductCode
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PackagePath
The path to a product package or packages for which the patch sequence is returned.

```yaml
Type: String[]
Parameter Sets: Path,PackagePath, LiteralPath,PackagePath
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
The path to a package to open.
Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path,PackagePath, Path,ProductCode
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -ProductCode
The ProductCode or ProductCodes for products for which the patch sequence is returned.

```yaml
Type: String[]
Parameter Sets: Path,ProductCode, LiteralPath,ProductCode
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserContext
The user context for the product listed in the ProductCode parameter.

```yaml
Type: UserContexts
Parameter Sets: Path,ProductCode, LiteralPath,ProductCode
Aliases: Context, InstallContext

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserSid
The user security identifier for product listed in the ProductCode parameter.

```yaml
Type: String
Parameter Sets: Path,ProductCode, LiteralPath,ProductCode
Aliases: User

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

### Microsoft.Tools.WindowsInstaller.PatchSequence

## NOTES

## RELATED LINKS
