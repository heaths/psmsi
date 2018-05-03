---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Install-MSIAdvertisedFeature

## SYNOPSIS
Installs advertised features.

## SYNTAX

### ProductCode (Default)
```
Install-MSIAdvertisedFeature [-ProductCode <String[]>] [[-FeatureName] <String[]>] [-Force]
 [-Properties <String[]>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Product
```
Install-MSIAdvertisedFeature -Product <ProductInstallation[]> [[-FeatureName] <String[]>] [-Force]
 [-Properties <String[]>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Some or all features can be advertised for a product.
This may be by design or incidental if certain patching problems occur.
You can use this cmdlet to to install some or all advertise features for some or all products.

You can also scan for all advertised features using the -WhatIf parameter to see what would be done to your system without performing those operations.
Use -Confirm if you want to approve the operation to each product.

## EXAMPLES

### EXAMPLE 1
```
install-msiadvertisedfeature -whatif
```

Scans all features in all products for advertised features and reports what operation would be performed.

### EXAMPLE 2
```
get-msiproductinfo '{12341234-1234-1234-1234-123412341234}' | install-msiadvertisedfeature Complete
```

Installs the 'Complete' feature for the specified product.

## PARAMETERS

### -Confirm
Confirm installing advertised features for each product.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FeatureName
One or more specific features to install.
Feature names are case-sensitive.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Force
Perform each operation without confirmation.

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

### -Product
One or more products passed through the pipeline to limit which products are scanned for advertised features.

```yaml
Type: ProductInstallation[]
Parameter Sets: Product
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ProductCode
One or more ProductCodes to limit which products are scanned for advertised features.

```yaml
Type: String[]
Parameter Sets: ProductCode
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Properties
Additional properties to pass to the installation operation.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -WhatIf
Show what operations would be performed without actually performing them.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

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

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## OUTPUTS

## NOTES

## RELATED LINKS

[Get-MSIProductInfo](get-msiproductinfo)

[Install-MSIProduct](install-msiproduct)

