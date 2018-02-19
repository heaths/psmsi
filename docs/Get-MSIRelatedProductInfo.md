---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIRelatedProductInfo

## SYNOPSIS
Gets product information for related products.

## SYNTAX

```
Get-MSIRelatedProductInfo [-UpgradeCode] <String[]> [<CommonParameters>]
```

## DESCRIPTION
Gets product information for related products based on an UpgradeCode.

## EXAMPLES

### EXAMPLE 1
```
get-msirelatedproductinfo "{B4160C68-1EA5-458F-B1EA-E69B41E44007}"
```

This command gets all related products based on their UpgradeCode.

## PARAMETERS

### -UpgradeCode
The UpgradeCode for related products.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## NOTES

## RELATED LINKS

[Get-MSIProductInfo](get-msiproductinfo)

