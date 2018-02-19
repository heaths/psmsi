---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSIComponentInfo

## SYNOPSIS
Gets information about components registered to the current user and the machine.

## SYNTAX

### Component (Default)
```
Get-MSIComponentInfo [[-ComponentCode] <String[]>] [<CommonParameters>]
```

### Product
```
Get-MSIComponentInfo [-ComponentCode] <String[]> [-ProductCode] <String> [<CommonParameters>]
```

## DESCRIPTION
Gets information about all the components registered to the current user and to the machine.
You can also limit the components to only those installed by a particular product.

The information includes the state of the component and the path all based on the product that installed it, since multiple products can install the same component even to different locations.

## EXAMPLES

### EXAMPLE 1
```
get-msicomponentinfo
```

This command gets all components installed or registered to the current user or to the machine.

### EXAMPLE 2
```
get-msiproductinfo "{90120000-00BA-0409-0000-0000000FF1CE}" | get-msicomponentinfo -componentcode "{90120000-00BA-0409-0000-0E32E9F6E558}"
```

This command gets information for the component "{90120000-00BA-0409-0000-0E32E9F6E558}" installed by the product "{90120000-00BA-0409-0000-0000000FF1CE}".

## PARAMETERS

### -ComponentCode
The component GUIDs to retrieve information.

```yaml
Type: String[]
Parameter Sets: Component
Aliases: ComponentId

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

```yaml
Type: String[]
Parameter Sets: Product
Aliases: ComponentId

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ProductCode
The ProductCode of the product that installed the components to retrieve information.

```yaml
Type: String
Parameter Sets: Product
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
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

[Get-MSIComponentState](get-msicomponentstate)

