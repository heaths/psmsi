---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSISource

## SYNOPSIS
Gets the registered network source or URLs for a product or patch.

## SYNTAX

```
Get-MSISource [-ProductCode] <String> [-PatchCode <String>] [-UserSid <String>] [-UserContext <UserContexts>]
 [<CommonParameters>]
```

## DESCRIPTION
Windows Installer products and patches can have zero or more registered locations that direct Windows Installer where to look for package source.
This cmdlet will enumerate those locations registered to a product or patch.

## EXAMPLES

### EXAMPLE 1
```
get-msiproductinfo | get-msisource
```

Gets the registered source for all installed products on the machine.

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

[Clear-MSISource](clear-msisource)

[Remove-MSISource](remove-msisource)

