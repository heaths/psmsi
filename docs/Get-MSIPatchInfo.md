---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIPatchInfo

## SYNOPSIS
Gets patch information for registered patches.

## SYNTAX

```
Get-MSIPatchInfo [[-ProductCode] <String[]>] [[-PatchCode] <String[]>] [-Filter <PatchStates>]
 [-UserContext <UserContexts>] [-UserSid <String>] [-Everyone] [<CommonParameters>]
```

## DESCRIPTION
Gets patch information for a given patch or for all patches registered to a given product or products.
You can get patch information for machine-registered patches, and patch information for both user-managed- and user-unmanaged-registered patches for the current or another user.

## EXAMPLES

### EXAMPLE 1
```
get-msipatchinfo
```

This command outputs a table of patch information for all applied patches on the machine.

### EXAMPLE 2
```
get-msipatchinfo -filter superseded | get-childitem
```

This command gets file information for superseded patches on the machine.

### EXAMPLE 3
```
get-msiproductinfo | where-object {$_.Name -match "Office"} | get-msipatchinfo -filter all
```

This command gets patch information for all patches applied to products with "Office" in the name.

## PARAMETERS

### -Everyone
Whether to retrieve user-managed or user-unmanaged patches for everyone.

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

### -Filter
The state or states of patches to be retrieved.
This can be a combination of "Applied", "Superseded", "Obsoleted", "Registered", or "All".

```yaml
Type: PatchStates
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### -PatchCode
The patch code or patch codes to retrieve patch information.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ProductCode
Specifies the ProductCode or ProductCodes to get patch information.

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

### -UserContext
The context for registered patches.
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
The security identifier for a user for user-managed and user-unmanaged patches.

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

### Microsoft.Deployment.WindowsInstaller.PatchInstallation

## NOTES

## RELATED LINKS

[Get-MSIProductInfo](get-msiproductinfo)

