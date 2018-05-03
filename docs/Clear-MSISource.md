---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Clear-MSISource

## SYNOPSIS
Clears all registered network sources and URLs from a product or patch.

## SYNTAX

```
Clear-MSISource [-ProductCode] <String> [-PatchCode <String>] [-UserSid <String>] [-UserContext <UserContexts>]
 [<CommonParameters>]
```

## DESCRIPTION
Windows Installer products and patches can have zero or more registered locations that direct Windows Installer where to look for package source.
This cmdlet will clear all network sources and URLs from a product or patch.

## EXAMPLES

### EXAMPLE 1
```
get-msiproductinfo '{707ABAE4-4DC5-478C-9D36-7CC5C1A85A3C}' | clear-msisource
```

Clears all registered source from the specified product.

## PARAMETERS

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

[Add-MSISource](add-msisource)

[Get-MSISource](get-msisource)

[Remove-MSISource](remove-msisource)

