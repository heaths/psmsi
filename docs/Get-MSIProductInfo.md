---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIProductInfo

## SYNOPSIS
Gets product information for registered products.

## SYNTAX

### Product (Default)
```
Get-MSIProductInfo [[-ProductCode] <String[]>] [-UserContext <UserContexts>] [-UserSid <String>] [-Everyone]
 [<CommonParameters>]
```

### Name
```
Get-MSIProductInfo -Name <String[]> [-UserContext <UserContexts>] [-UserSid <String>] [-Everyone]
 [<CommonParameters>]
```

## DESCRIPTION
Gets product information for all per-machine, user-managed, and user-unmanaged products on the machine.

## EXAMPLES

### EXAMPLE 1
```
get-msiproductinfo
```

This command outputs product information for all registered products assigned to this machine.

### EXAMPLE 2
```
get-msiproductinfo | where-object {$_.Name -match "Visual Studio"}
```

This command outputs all product information for products with "Visual Studio" in the name assigned to this machine.

### EXAMPLE 3
```
get-msiproductinfo -installcontext userunmanaged | where-object {$_.ProductState -eq "Installed"} | get-childitem
```

This command gets file information for all installed user-unmanaged products.

### EXAMPLE 4
```
get-msiproductinfo "{1862162E-3BBC-448F-AA63-49F33152D54A}"
```

This command gets product information for the given ProductCode.

## PARAMETERS

### -Everyone
Whether to retrieve user-managed or user-unmanaged products for everyone.

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

### -Name
The name of a product or products to retrieve.
Wildcards are supported.

```yaml
Type: String[]
Parameter Sets: Name
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -ProductCode
The ProductCode or ProductCodes to retrieve product information.

```yaml
Type: String[]
Parameter Sets: Product
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UserContext
The context for registered products.
This can be a combination of "Machine", "UserManaged", or "UserUnmanaged".

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
The security identifier for a user for user-managed and user-unmanaged products.

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

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## NOTES

## RELATED LINKS

[Get-MSIPatchInfo](get-msipatchinfo)

[Get-MSIRelatedProductInfo](get-msirelatedproductinfo)

