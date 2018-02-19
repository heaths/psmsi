---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Uninstall-MSIProduct

## SYNOPSIS
Uninstalls a product package or packages, or an existing product on the machine.

## SYNTAX

### Path (Default)
```
Uninstall-MSIProduct [-Path] <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force]
 [-ResultVariable <String>] [<CommonParameters>]
```

### Product
```
Uninstall-MSIProduct -ProductCode <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force]
 [-ResultVariable <String>] [<CommonParameters>]
```

### Installation
```
Uninstall-MSIProduct -Product <ProductInstallation[]> [-Log <String>] [-Properties <String[]>] [-Chain]
 [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### LiteralPath
```
Uninstall-MSIProduct -LiteralPath <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force]
 [-ResultVariable <String>] [<CommonParameters>]
```

## DESCRIPTION
This cmdlet, unlike related cmdlets, is designed to uninstalled one or more products.
While there are ways to override this behavior, it is not recommend and you should instead use specialized cmdlets for this purpose.
See the related links for suggestions.

Progress, warnings, and errors during the install are sent through the pipeline making this command fully integrated.

## EXAMPLES

### EXAMPLE 1
```
uninstall-msiproduct .\example.msi -log $env:TEMP\uninstall.log
```

Uninstall the example.msi product package and log to the TEMP directory.

### EXAMPLE 2
```
get-msiproductinfo -name *TEST* | uninstall-msiproduct -chain
```

Uninstall all products with ProductName matching *TEST* and show a single progress bar for the entire operation.

## PARAMETERS

### -Chain
Whether to install all packages together.
If elevated, a single restore point is created for all packages in the chain and reboots are suppressed when possible.

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

### -Force
Whether to suppress all prompts.

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

### -LiteralPath
The path to a product package to uninstall.
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

### -Log
The path to the log file.
This use the file name as the base name and will append timestamp and product-specific information.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
The path to a product package to uninstall.
Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -Product
The product to uninstall.

```yaml
Type: ProductInstallation[]
Parameter Sets: Installation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ProductCode
The ProductCode of a product to uninstall.

```yaml
Type: String[]
Parameter Sets: Product
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Properties
Additional property=value pairs to pass during uninstall.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ResultVariable
The name of a variable to store operation results.
Optionally prefix with "+" to combine results with existing results variable.

```yaml
Type: String
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

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## OUTPUTS

## NOTES

## RELATED LINKS

[Install-MSIProduct](install-msiproduct)

[Repair-MSIProduct](repair-msiproduct)

