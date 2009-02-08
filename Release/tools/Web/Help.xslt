<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:maml="http://schemas.microsoft.com/maml/2004/10"
    xmlns:command="http://schemas.microsoft.com/maml/dev/command/2004/10"
    xmlns:dev="http://schemas.microsoft.com/maml/dev/2004/10"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl">
  
  <xsl:output method="text" encoding="Windows-1252"/>
  <xsl:preserve-space elements="maml:code maml:para"/>
  <xsl:param name="version">2</xsl:param>

  <!-- for debugging purposes only -->
  <xsl:template match="msh:helpItems" xmlns:msh="http://msh">
    <xsl:apply-templates select="command:command[command:details/command:name='Get-MSIPatchInfo']"/>
  </xsl:template>

  <xsl:template match="command:command">
    <xsl:apply-templates select="command:details"/>
    <xsl:apply-templates select="command:syntax"/>
    <xsl:apply-templates select="maml:description" mode="details"/>
    <xsl:apply-templates select="command:parameters"/>
    <xsl:apply-templates select="command:inputTypes"/>
    <xsl:apply-templates select="command:returnValues"/>
    <xsl:apply-templates select="command:examples"/>
    <xsl:apply-templates select="maml:relatedLinks"/>
  </xsl:template>

  <xsl:template match="command:details">
    <!-- NAME -->
    <xsl:text xml:space="preserve">! </xsl:text>
    <xsl:value-of select="command:name"/>
    <xsl:text xml:space="preserve">
</xsl:text>
    <!-- SYNOPSIS -->
    <xsl:call-template name="heading">
      <xsl:with-param name="title">Synopsis</xsl:with-param>
    </xsl:call-template>
    <xsl:apply-templates select="maml:description/maml:para"/>
  </xsl:template>

  <xsl:template match="command:syntax">
    <!-- SYNTAX -->
    <xsl:call-template name="heading">
      <xsl:with-param name="title">Syntax</xsl:with-param>
    </xsl:call-template>
    <xsl:apply-templates select="command:syntaxItem"/>
  </xsl:template>

  <xsl:template match="maml:description" mode="details">
    <!-- DETAILED DESCRIPTION -->
    <xsl:call-template name="heading">
      <xsl:with-param name="title">Detailed Description</xsl:with-param>
    </xsl:call-template>
    <xsl:apply-templates select="maml:para"/>
  </xsl:template>

  <xsl:template match="command:parameters">
    <!-- PARAMETERS -->
    <xsl:call-template name="heading">
      <xsl:with-param name="title">Parameters</xsl:with-param>
    </xsl:call-template>
    <xsl:apply-templates select="command:parameter" mode="parameters"/>
  </xsl:template>

  <xsl:template match="command:inputTypes">
    <!-- INPUT TYPE -->
    <xsl:if test="string-length(command:inputType/dev:type/maml:name)">
      <xsl:call-template name="heading">
        <xsl:with-param name="title">Input Type</xsl:with-param>
      </xsl:call-template>
      <xsl:apply-templates select="command:inputType"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="command:returnValues">
    <!-- RETURN TYPE -->
    <xsl:if test="string-length(command:returnValue/dev:type/maml:name)">
      <xsl:call-template name="heading">
        <xsl:with-param name="title">Return Type</xsl:with-param>
      </xsl:call-template>
      <xsl:apply-templates select="command:returnValue"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="command:examples">
    <!-- NOTES -->
    <xsl:call-template name="heading">
      <xsl:with-param name="title">Notes</xsl:with-param>
    </xsl:call-template>
    <xsl:apply-templates select="command:example"/>
  </xsl:template>

  <xsl:template match="maml:relatedLinks">
    <!-- RELATED LINKS -->
    <xsl:call-template name="heading">
      <xsl:with-param name="title">Related Links</xsl:with-param>
    </xsl:call-template>
    <xsl:apply-templates select="maml:navigationLink/maml:linkText"/>
  </xsl:template>

  <xsl:template name="heading">
    <xsl:param name="title"/>
    <xsl:text xml:space="preserve">!! </xsl:text>
    <xsl:value-of select="$title"/>
    <xsl:text>{anchor:</xsl:text>
    <xsl:value-of select="translate($title,'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
    <xsl:text xml:space="preserve">}
</xsl:text>
  </xsl:template>

  <xsl:template match="maml:para">
    <xsl:if test="string-length(node()) > 0">
      <!-- add extra space for separate paragraphs -->
      <xsl:if test="preceding-sibling::maml:para">
        <xsl:text xml:space="preserve">
</xsl:text>
      </xsl:if>
      <xsl:value-of select="node()" disable-output-escaping="yes"/>
      <xsl:text xml:space="preserve">
</xsl:text>
    </xsl:if>
  </xsl:template>

  <xsl:template match="command:syntaxItem">
    <xsl:text xml:space="preserve">* </xsl:text>
    <xsl:value-of select="maml:name"/>
    <xsl:apply-templates select="command:parameter" mode="syntax"/>
    <xsl:text xml:space="preserve">
</xsl:text>
  </xsl:template>

  <xsl:template match="command:parameter" mode="syntax">
    <xsl:text xml:space="preserve"> </xsl:text>
    <xsl:if test="@required">&amp;#91;</xsl:if>
    <xsl:if test="@position > 0">&amp;#91;</xsl:if>
    <xsl:text>-</xsl:text>
    <xsl:value-of select="maml:name"/>
    <xsl:if test="@position > 0">&amp;#93;</xsl:if>
    <xsl:if test="command:parameterValue">
      <xsl:text xml:space="preserve"> </xsl:text>
      <xsl:if test="not(command:parameterValue/@required)">&amp;#91;</xsl:if>
      <xsl:text>&lt;</xsl:text>
      <xsl:value-of select="command:parameterValue"/>
      <xsl:text>&gt;</xsl:text>
      <xsl:if test="not(command:parameterValue/@required)">&amp;#93;</xsl:if>
    </xsl:if>
    <xsl:if test="@required">&amp;#93;</xsl:if>
  </xsl:template>

  <xsl:template match="command:parameter" mode="parameters">
    <xsl:text xml:space="preserve">!!! -</xsl:text>
    <xsl:value-of select="maml:name"/>
    <xsl:if test="command:parameterValue">
      <xsl:text xml:space="preserve"> &lt;</xsl:text>
      <xsl:value-of select="command:parameterValue"/>
      <xsl:text>&gt;</xsl:text>
    </xsl:if>
    <xsl:text xml:space="preserve">
</xsl:text>
    <xsl:apply-templates select="maml:description/maml:para"/>
    <xsl:text xml:space="preserve">
</xsl:text>
    <xsl:apply-templates select="@required" mode="parameters"/>
    <xsl:apply-templates select="@position" mode="parameters"/>
    <xsl:text xml:space="preserve">| Default value | </xsl:text>
    <xsl:value-of select="dev:defaultValue"/>
    <xsl:text xml:space="preserve"> |
</xsl:text>
    <xsl:apply-templates select="@pipelineInput" mode="parameters"/>
    <xsl:apply-templates select="@globbing" mode="parameters"/>
  </xsl:template>

  <xsl:template match="command:parameter/@*" mode="parameters">
    <xsl:text xml:space="preserve">| </xsl:text>
    <xsl:choose>
      <xsl:when test="name()='required'">Required?</xsl:when>
      <xsl:when test="name()='position'">Position?</xsl:when>
      <xsl:when test="name()='pipelineInput'">Accept pipeline input?</xsl:when>
      <xsl:when test="name()='globbing'">Accept wildcard characters?</xsl:when>
    </xsl:choose>
    <xsl:text xml:space="preserve"> | </xsl:text>
    <xsl:value-of select="."/>
    <xsl:text xml:space="preserve"> |
</xsl:text>
  </xsl:template>

  <xsl:template match="command:example">
    <xsl:text xml:space="preserve">!!! Example </xsl:text>
    <xsl:value-of select="position()"/>
    <xsl:text xml:space="preserve">

{{
</xsl:text>
    <xsl:value-of select="maml:introduction/maml:para"/>
    <xsl:text xml:space="preserve"> </xsl:text>
    <xsl:value-of select="dev:code" disable-output-escaping="yes"/>
    <xsl:text>

}}</xsl:text>
    <xsl:apply-templates select="dev:remarks/maml:para"/>
    <xsl:text xml:space="preserve">
</xsl:text>
  </xsl:template>

  <xsl:template match="command:inputType|command:returnValue">
    <!-- cmdlet help editor was inserting empty node sets, so check length -->
    <xsl:if test="string-length(dev:type/maml:name)">
      <xsl:text xml:space="preserve">* </xsl:text>
      <xsl:value-of select="dev:type/maml:name"/>
      <xsl:choose>
      <xsl:when test="dev:type/maml:description/maml:para">
        <xsl:text xml:space="preserve">: </xsl:text>
        <xsl:apply-templates select="dev:type/maml:description/maml:para"/>
      </xsl:when>
        <xsl:otherwise>
      <xsl:text xml:space="preserve">
</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>

  <xsl:template match="maml:linkText">
    <xsl:text xml:space="preserve">* [</xsl:text>
    <xsl:value-of select="node()"/>
    <xsl:text xml:space="preserve">|</xsl:text>
    <xsl:text>v</xsl:text>
    <xsl:value-of select="$version"/>
    <xsl:text>_</xsl:text>
    <xsl:value-of select="node()"/>
    <xsl:text xml:space="preserve">]
</xsl:text>
  </xsl:template>

</xsl:stylesheet>
