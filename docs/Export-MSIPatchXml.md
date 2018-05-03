---
external help file: Microsoft.Tools.WindowsInstaller.PowerShell.dll-Help.xml
Module Name: MSI
online version:
schema: 2.0.0
---

# Export-MSIPatchXml

## SYNOPSIS
Exports an XML representation of applicability information from a patch package.

## SYNTAX

```
Export-MSIPatchXml [-Path] <String> [-FilePath] <String> [-Encoding <Encoding>] [-Formatted]
 [<CommonParameters>]
```

## DESCRIPTION
Windows Installer defines an XML schema that is representational of a patch package - specifically its applicability information.
This allows administrators and bundle developers to not require downloading the patch package just to find out if it's applicable or even already installed.

This XML file can be passed to Get-MSIPatchSequence along with other XML files or patch packages.

## EXAMPLES

### EXAMPLE 1
```
export-msipatchxml .\example.msp .\example.xml -formatted
```

Exports formatted XML from the example.msp patch package in the current directory.

## PARAMETERS

### -Encoding
The encoding to use for the output XML file.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: UTF8
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePath
The path to the output XML file.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Formatted
Whether to indent the XML file.

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
The path to the patch package from which XML is exported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
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

[Get-MSIPatchSequence](get-msipatchsequence)

