---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Install-MSIProduct

## SYNOPSIS
Installs or modifies a product package.

## SYNTAX

### Path (Default)
```
Install-MSIProduct [-Destination <String>] [-PassThru] [-Path] <String[]> [-Log <String>]
 [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### LiteralPath
```
Install-MSIProduct [-Destination <String>] [-PassThru] -LiteralPath <String[]> [-Log <String>]
 [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### Product
```
Install-MSIProduct [-PassThru] -ProductCode <String[]> [-Log <String>] [-Properties <String[]>] [-Chain]
 [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

### Installation
```
Install-MSIProduct [-PassThru] -Product <ProductInstallation[]> [-Log <String>] [-Properties <String[]>]
 [-Chain] [-Force] [-ResultVariable <String>] [<CommonParameters>]
```

## DESCRIPTION
Installs a product package or adds features to existing products.

Ultimately, this cmdlet can install, modify, repair, and even uninstall a product package or install patches but specialized cmdlets have been added for those tasks.

Progress, warnings, and errors during the install are sent through the pipeline making this command fully integrated.

## EXAMPLES

### EXAMPLE 1
```
install-msiproduct .\example.msi NOBLOCK=1
```

Installs the example.msi product package passing the ficticious NOBLOCK=1 property.

### EXAMPLE 2
```
get-msiproductinfo -name TEST | install-msiproduct ADDLOCAL=Addin -log $env:TEMP\install.log
```

Modifies the existing product with ProductName TEST to add the "Addin" feature locally and log to the TEMP directory.

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

### -Destination
The target directory where the product should be installed.

Note that the product package must be authored to support installing to TARGETDIR.

```yaml
Type: String
Parameter Sets: Path, LiteralPath
Aliases: TargetDirectory

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
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
The path to a product package to install.
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

### -PassThru
Whether to pass the newly installed product information after installation to the pipeline.

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
The path to a product package to install.
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
An existing product to modify.

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
The ProductCode of an existing product to modify.

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
Additional property=value pairs to pass during install.

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

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## NOTES

## RELATED LINKS

[Install-MSIPatch](install-msipatch)

[Repair-MSIProduct](repair-msiproduct)

[Uninstall-MSIPatch](uninstall-msipatch)

[Uninstall-MSIProduct](uninstall-msiproduct)

