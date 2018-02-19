---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Get-MSITable

## SYNOPSIS
Selects records from a table or custom query from a product or patch package.

## SYNTAX

### Path,Table (Default)
```
Get-MSITable [-Path] <String[]> [[-Table] <String>] [-Patch <String[]>] [-Transform <String[]>]
 [<CommonParameters>]
```

### Path,Query
```
Get-MSITable [-Path] <String[]> -Query <String> [-Patch <String[]>] [-Transform <String[]>]
 [<CommonParameters>]
```

### LiteralPath,Table
```
Get-MSITable -LiteralPath <String[]> [[-Table] <String>] [-Patch <String[]>] [-Transform <String[]>]
 [<CommonParameters>]
```

### LiteralPath,Query
```
Get-MSITable -LiteralPath <String[]> -Query <String> [-Patch <String[]>] [-Transform <String[]>]
 [<CommonParameters>]
```

### Installation,Table
```
Get-MSITable [-Product] <ProductInstallation[]> [[-Table] <String>] [-IgnoreMachineState] [<CommonParameters>]
```

### Installation,Query
```
Get-MSITable [-Product] <ProductInstallation[]> -Query <String> [-IgnoreMachineState] [<CommonParameters>]
```

## DESCRIPTION
You can query all records from a table or records matching a custom query from a product or patch package.
The Windows Installer SDK has more information about custom queries, since the SQL-like syntax is rather constrained.

When no table or query is provided all tables from the package are displayed.
Specifying a patch or transform will cause tables added by the patch or transform to be displayed, along with the operation performed on that table by the patch or transform in the MSIOperation property.

Records are returned with properties matching column names.
If records are selected from a single table, the table name is also part of the type name queryable from the PSTypeNames object property.
If the column name is prefixed with the table name - required to disambiguate names, or optional otherwise - a property is returned as typed in the original query string.
Note that Windows PowerShell allows periods in property names but may require in some scenarios that a property name with periods is contained in quotes, like 'File.Attributes'.

For attribute columns in standard Windows Installer tables, you may also query for specific attribute values by specifying a special property name on the end of the attribute column name, like 'File.Attributes'.HasVital, to query for any columns that have the attribute value set.
You can define an $MsiAttributeColumnFormat variable to control the display format of attribute columns, though the underlying value will not be changed.
Run 'help about_MSI' for more information.

Note that patch packages do not typically have more than a couple of tables.
The patch has to be applied to a product package to view any changes it has made.
When applying a patch or transforms, records will contain an operation performed on the record by the patch or transform in the MSIOperation property.

## EXAMPLES

### EXAMPLE 1
```
get-msitable .\example.msi -table Property
```

Gets all records from the Property table.

### EXAMPLE 2
```
$productCode = get-msitable .\example.msi -table Property | where-object { $_.Property -eq "ProductCode" } | select-object -expand Value
```

Selects just the ProductCode property from the example.msi package and assigns the value to a variable.

### EXAMPLE 3
```
get-childitem -filter *.msi | get-msitable -query "SELECT ComponentId, FileName, FileSize FROM Component, File WHERE Component_ = Component"
```

Selects the component GUID, file name, and file size for all files in all packages in the current directory.

### EXAMPLE 4
```
get-msitable .\example.msi -query "SELECT ComponentId, FileName, File.Attributes FROM Component, File WHERE Component_ = Component" | where-object { $_.'File.Attributes'.HasVital }
```

Selects all vital files and displays the component GUID, file name, and all file attribtes from the example.msi package.

Note that in the query filter the 'File.Attributes' column is contained in quotes; otherwise, Windows PowerShell will attempt to filter based on an Attributes property of a File property of the current pipeline object.
The 'File.Attributes' column is contained in quotes in the original query because the Component table also contains a column named Attributes.
Windows Installer requires that you disambiguate column names.

### EXAMPLE 5
```
get-msitable .\example.msi -patch .\example.msp | get-msitable | where-object { $_.MSIOperation -ne 'None' }
```

Gets all records in the example.msi package added or modified by the example.msp patch package.

### EXAMPLE 6
```
get-msiproductinfo '{877EF582-78AF-4D84-888B-167FDC3BCC11}' | get-msitable -table Property
```

Selects records from the installed product along with any patches currently installed.

## PARAMETERS

### -IgnoreMachineState
Whether to apply any patches current installed to the product.

```yaml
Type: SwitchParameter
Parameter Sets: Installation,Table, Installation,Query
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
The path to a product package to open.
The value of -LiteralPath is used exactly as typed.
No characters are interpreted as wildcards.

```yaml
Type: String[]
Parameter Sets: LiteralPath,Table, LiteralPath,Query
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Patch
The path to a patch package to apply to the product package.
Multiple patches are applied in authored sequence order.

Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path,Table, Path,Query, LiteralPath,Table, LiteralPath,Query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### -Path
The path to a product package to open.
Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path,Table, Path,Query
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -Product
An installed product to query.

```yaml
Type: ProductInstallation[]
Parameter Sets: Installation,Table, Installation,Query
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Query
A custom query for which records are selected.
Ambiguous column names must be prefixed with the table name to which they belong.

```yaml
Type: String
Parameter Sets: Path,Query, LiteralPath,Query, Installation,Query
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Table
The table from which all records are selected.
If no table name is provided all tables in the database are displayed.

```yaml
Type: String
Parameter Sets: Path,Table, LiteralPath,Table, Installation,Table
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Transform
The path to a transform to apply to the product package.

Wildcards are permitted.
You can specify * in any part of the path to select all matching files.

```yaml
Type: String[]
Parameter Sets: Path,Table, Path,Query, LiteralPath,Table, LiteralPath,Query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.Deployment.WindowsInstaller.ProductInstallation

## OUTPUTS

### Microsoft.Tools.WindowsInstaller.Record

### Microsoft.Tools.WindowsInstaller.TableInfo

## NOTES

## RELATED LINKS

[Get-MSIProductInfo](get-msiproductinfo)

[Get-MSIProperty](get-msiproperty)

