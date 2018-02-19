---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Uninstall-MSIPatch

## SYNOPSIS
Installs a patch package or packages for all or only specified products.

## SYNTAX

### Path (Default)
```
Uninstall-MSIPatch [-ProductCode <String[]>] [-UserContext <UserContexts>] [-UserSid <String>]
 [-Path] <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>]
 [<CommonParameters>]
```

### Installation
```
Uninstall-MSIPatch -Patch <PatchInstallation[]> [-ProductCode <String[]>] [-UserContext <UserContexts>]
 [-UserSid <String>] [-Log <String>] [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>]
 [<CommonParameters>]
```

### LiteralPath
```
Uninstall-MSIPatch [-ProductCode <String[]>] [-UserContext <UserContexts>] [-UserSid <String>]
 -LiteralPath <String[]> [-Log <String>] [-Properties <String[]>] [-Chain] [-Force] [-ResultVariable <String>]
 [<CommonParameters>]
```

## DESCRIPTION
Uninstalls one or more packages from all products which they're applied or only from the specified set of products based on their ProductCode.

Progress, warnings, and errors during the install are sent through the pipeline making this command fully integrated.

## EXAMPLES

### EXAMPLE 1
```
get-msiproductifo -name TEST | get-msipatchinfo | uninstall-msipatch -log $env:TEMP\unpatch.log
```

Uninstalls all patches applied to the product with ProductName TEST and logs to the TEMP directory.

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
The path to a patch package to uninstall.
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

### -Patch
Information about a patch or patches to uninstall.

```yaml
Type: PatchInstallation[]
Parameter Sets: Installation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Path
The path to a patch package to uninstall.
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

### -ProductCode
The ProductCode or ProductCodes from which patches are removed.

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

### -UserContext
The user context for the product listed in the ProductCode parameter.

```yaml
Type: UserContexts
Parameter Sets: (All)
Aliases: Context, InstallContext

Required: False
Position: Named
Default value: Machine
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UserSid
The user security identifier for product listed in the ProductCode parameter.

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

## NOTES

## RELATED LINKS

[Install-MSIPatch](install-msipatch)

