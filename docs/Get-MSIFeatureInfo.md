---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIFeatureInfo

## SYNOPSIS
Gets information about features of an installed or advertised product.

## SYNTAX

### Product (Default)
```
Get-MSIFeatureInfo [-Product] <ProductInstallation[]> [<CommonParameters>]
```

### Feature
```
Get-MSIFeatureInfo [-ProductCode] <String> [-FeatureName] <String[]> [<CommonParameters>]
```

## DESCRIPTION
A product must install or advertise one or more features.
This cmdlet can query feature of a product or products to determine their state and optional usage data.

## EXAMPLES

### EXAMPLE 1
```
get-msiproductinfo "{90120000-00BA-0409-0000-0000000FF1CE}" | get-msifeatureinfo | format-table -view Usage
```

Gets the usage information for all the features installed by the product "{90120000-00BA-0409-0000-0000000FF1CE}".

### EXAMPLE 2
```
get-msifeatureinfo "{90120000-00BA-0409-0000-0000000FF1CE}" "GrooveFilesIntl_1033"
```

Gets state information for the feature "GrooveFilesIntl_1033" installed by product "{90120000-00BA-0409-0000-0000000FF1CE}".

## PARAMETERS

### -FeatureName
The names of the features for which information is retrieved.

```yaml
Type: String[]
Parameter Sets: Feature
Aliases: Name

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Product
The ProductInstallation object that installed or advertised specified features.

```yaml
Type: ProductInstallation[]
Parameter Sets: Product
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ProductCode
The ProductCode that installed or advertised specified features.

```yaml
Type: String
Parameter Sets: Feature
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

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.FeatureInstallation

## NOTES

## RELATED LINKS

[Get-MSIProductInfo](get-msiproductinfo)

