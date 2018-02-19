---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIComponentState

## SYNOPSIS
Gets the install state for all authored components for one or more products installed on the machine.

## SYNTAX

### Product (Default)
```
Get-MSIComponentState [-Product] <ProductInstallation[]> [<CommonParameters>]
```

### ProductCode
```
Get-MSIComponentState [-ProductCode] <String[]> [-UserContext <UserContexts>] [-UserSid <String>]
 [<CommonParameters>]
```

## DESCRIPTION
Gets the install state for all components authored into one or more products.
This includes all patches applied to the product.
In addition to the information returned from Get-MSIComponentInfo, the authored component identifier from the Component table is attached along with a simple boolean property that determines if the component is installed locally or not.

## EXAMPLES

### EXAMPLE 1
```
get-msicomponentstate "{877EF582-78AF-4D84-888B-167FDC3BCC11}"
```

Gets state information for all components authored into the product "{877EF582-78AF-4D84-888B-167FDC3BCC11}" and all applied patches.

### EXAMPLE 2
```
get-msiproductinfo -name *TEST* | get-msicomponentstate
```

Gets state information for all components authored into any product where the ProductName matches *TEST*.

## PARAMETERS

### -Product
The products for which authored component state is retrieved.

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
The installed ProductCodes that define the components for which state information is retrieved.

```yaml
Type: String[]
Parameter Sets: ProductCode
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -UserContext
The context for registered products.
This can be a combination of "Machine", "UserManaged", or "UserUnmanaged".

```yaml
Type: UserContexts
Parameter Sets: ProductCode
Aliases: Context, InstallContext

Required: False
Position: Named
Default value: All
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UserSid
The security identifier for a user for user-managed and user-unmanaged products.

```yaml
Type: String
Parameter Sets: ProductCode
Aliases:

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

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.ComponentInstallation#State

## NOTES

## RELATED LINKS

[Get-MSIComponentInfo](get-msicomponentinfo)

[Get-MSIProductInfo](get-msiproductinfo)

