---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Remove-MSISource

## SYNOPSIS
Removes a registered network source or URL from a product or patch.

## SYNTAX

### Path
```
Remove-MSISource [-Path] <String[]> [-PassThru] [-ProductCode] <String> [-PatchCode <String>]
 [-UserSid <String>] [-UserContext <UserContexts>] [<CommonParameters>]
```

### LiteralPath
```
Remove-MSISource -LiteralPath <String[]> [-PassThru] [-ProductCode] <String> [-PatchCode <String>]
 [-UserSid <String>] [-UserContext <UserContexts>] [<CommonParameters>]
```

## DESCRIPTION
Windows Installer products and patches can have zero or more registered locations that direct Windows Installer where to look for package source.
This cmdlet will remove a location registered to a product or patch and optionally return the remaining locations through the pipeline.

## EXAMPLES

### EXAMPLE 1
```
remove-msisource '{707ABAE4-4DC5-478C-9D36-7CC5C1A85A3C}' 'C:\Package Cache\'
```

Removes the C:\Package Cache source location from the specified product.

## PARAMETERS

### -LiteralPath
The directory or URL to unregister.
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
Whether to return the remaining registered source through the pipeline.

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

### -PatchCode
The patch code for a patch.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Path
The directory or URL to unregister.
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

### -ProductCode
The ProductCode for a product or applied patch.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UserContext
The user context for a product or patch.

```yaml
Type: UserContexts
Parameter Sets: (All)
Aliases: Context, InstallContext

Required: False
Position: Named
Default value: All
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UserSid
The user SID for a product or patch.

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

### Microsoft.Tools.WindowsInstaller.SourceInfo

### Microsoft.Tools.WindowsInstaller.PatchSourceInfo

## NOTES

## RELATED LINKS

[Add-MSISource](remove-msisource)

[Clear-MSISource](clear-msisource)

[Get-MSISource](get-msisource)

