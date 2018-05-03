---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSISharedComponentInfo

## SYNOPSIS
Gets information about shared components installed or registered for the current user or the machine.

## SYNTAX

```
Get-MSISharedComponentInfo [[-ComponentCode] <String[]>] [[-Count] <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Shared components are component which are installed to the same directory by one or more products.
This cmdlet gets information about all or specified shared components installed for the current user or the machine.

The output is already sorted by ComponentCode then ProductCode.

## EXAMPLES

### EXAMPLE 1
```
get-msisharedcomponentinfo -count 4 | format-table -view Clients
```

Gets shared components installed by at least 4 products (or features) and displays them in a table grouped by ComponentCode.

## PARAMETERS

### -ComponentCode
The component GUIDs to retrieve information.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Count
The minimum number count for shared components returned.
The absolute minimum is 2.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: 2
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### Microsoft.Deployment.WindowsInstaller.ComponentInstallation

## NOTES

## RELATED LINKS

[Get-MSIComponentInfo](get-msicomponentinfo)

