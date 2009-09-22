#Requires -Version 2.0

# PowerShell module for common TF server commands.
#

DATA loc {
	# en-US
	convertfrom-stringdata @'
		FindingWorkspaceInfo = Finding workspace information for path "{0}".
		UpdatingWorkspaceCache = Updating the workstation information cache.
'@
}

# Make sure required assemlies are loaded.
$Version = "Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
[Reflection.Assembly]::Load("Microsoft.TeamFoundation.Client, $Version")
[Reflection.Assembly]::Load("Microsoft.TeamFoundation.VersionControl.Client, $Version")

function Get-TFServer
{
<#

.Synopsis
Gets or lists TF servers registered to the machine.

.Description
Gets Team Foundation servers registered to the machine. You can specify a URI
to identify the server by its source location, or pass $Null (default)
to list all Team Foundation servers registered to the machine.

.Parameter Uri
If specified, gets the server(s) for each URI.

#>
	[CmdletBinding(ConfirmImpact="None")]
	param
	(
		[Parameter(Position=0, ValueFromPipelineByPropertyName=$true)]
		[string[]] $Uri
	)

	process
	{
		if ( $Uri )
		{
			foreach ( $i in $Uri )
			{
				$server = [Microsoft.TeamFoundation.Client.RegisteredServers]::GetServerForUri( $i )
				[Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer( $server )
			}
		}
		else
		{
			[Microsoft.TeamFoundation.Client.RegisteredServers]::GetServers()
		}
	}
}

function Get-TFWorkspace
{
<#

.Synopsis
Gets the workspace(s) for the specified path(s).

.Description
Gets each workspace for the specified paths. The default location
is the current directory, but you can specify multiple paths belonging
to multiple workspaces. This makes it possible to execute commands
against all workspaces on a machine.

.Parameter Path
The mapped path for which workspace information is retrieved.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The mapped path for which workspace information is retrieved.
This parameter does not expand wildcards.

#>
	[CmdletBinding(ConfirmImpact="None", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, ValueFromPipeline=$true)]
		[ValidateNotNullOrEmpty()]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath
	)

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			$LiteralPath = resolve-path $Path
		}

		get-tfworkspaceinfo -literalPath $LiteralPath | select-object -unique | foreach-object {

			$info = $_
			get-tfserver $_.ServerUri | foreach-object { $info.GetWorkspace( $_ ) }
		}
	}
}

function Get-TFWorkspaceInfo
{
<#

.Synopsis
Gets information about the workspace for the specified path(s).

.Description
Gets each workspace information for the specified paths. The default location
is the current directory, but you can specify multiple paths belonging
to multiple workspaces. This makes it possible to list all information
for all workspaces on a machine.

.Parameter Path
The mapped path for which workspace information is retrieved.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The mapped path for which workspace information is retrieved.
This parameter does not expand wildcards.

.Paramet.Parameter All
Get all local workspaces.

.Parameter ServerUri
The Team Foundation Server URI to which a workspace belongs.
The default will query all registered servers on the machine.

#>
	[CmdletBinding(ConfirmImpact="None", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, ValueFromPipeline=$true)]
		[ValidateNotNullOrEmpty()]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath, 

		[Parameter(ParameterSetName="All")]
		[switch] $All,

		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[Alias("Uri")]
		[string] $ServerUri
	)

	begin
	{
		$workstation = [Microsoft.TeamFoundation.VersionControl.Client.Workstation]

		# Update the workspace cache for each (specified) server.
		write-verbose $loc.UpdatingWorkspaceCache
		get-tfserver $ServerUri | foreach-object {

			$vcs = $_.GetService([Microsoft.TeamFoundation.VersionControl.Client.VersionControlServer])
			if ($_.HasAuthenticated)
			{
				$workstation::Current.EnsureUpdateWorkspaceInfoCache($vcs, $vcs.AuthenticatedUser)
			}
		}
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "All" )
		{
			$workstation::Current.GetAllLocalWorkspaceInfo()
		}
		else
		{
			if ( $PSCmdlet.ParameterSetName -eq "Path" )
			{
				$LiteralPath = resolve-path $Path
			}
			
			convert-path -literalPath $LiteralPath | select-object -unique | foreach-object {
			
				write-verbose ( $loc.FindingWorkspaceInfo -f $_ )
				$workstation::Current.GetLocalWorkspaceInfo( $_ )
			}
		}
	}
}

# Command: tf status
function Get-TFStatus
{
<#

.Synopsis
Gets the status of pending changes.

.Description
Gets the status of all items in all workspaces specified by the given
path(s). You can specify multiple paths in multiple workspaces to get
status of every mapped item on the machine.

.Parameter Path
The mapped path for which status is retrieved.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The mapped path for which status is retrieved.
This parameter does not expand wildcards.

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

#>
	[CmdletBinding(ConfirmImpact="None", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, ValueFromPipeline=$true)]
		[ValidateNotNullOrEmpty()]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath,

		[Parameter()]
		[switch] $Recurse
	)

	begin
	{
		$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			# Status defaults to all items in the current file system directory.
			$LiteralPath = get-childitem $Path
		}

		[string[]] $localPath = convert-path -literalPath $LiteralPath | select-object -unique
		get-tfworkspace -literalPath $localPath | foreach-object {

			$_.GetPendingChanges( $localPath, $recursive, $true )
		}
	}
}

# Command: tf add
function Add-TFItem
{
<#

.Synopsis
Adds item(s) to version control.

.Description
Adds an item or items to version control. You must publish the changes
to actually add the files to the version control server.

.Parameter Path
The item within a mapped path to add to version control.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The item within a mapped path to add to version control.
This parameter does not expand wildcards.

.Parameter Encoding
The encoding for the file. The default value of $null will detect
the encoding of the file(s).

.Parameter LockLevel
The lock type to put on the file(s). Values include locking for
"Checkin", "Checkout", "Unchanged", or the default value of "None".

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

.Parameter PassThru
Whether to return status for added items. The default returns nothing.

#>
	[CmdletBinding(ConfirmImpact="Low", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, Mandatory=$true, ValueFromPipeline=$true)]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath,

		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[string] $Encoding,

		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[Microsoft.TeamFoundation.VersionControl.Client.LockLevel] $LockLevel = "None",

		[Parameter()]
		[switch] $Recurse,

		[Parameter()]
		[switch] $PassThru
	)

	begin
	{
		$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			$LiteralPath = resolve-path $Path
		}

		[string[]] $localPath = convert-path -literalPath $LiteralPath | select-object -unique
		get-tfworkspace -literalPath $localPath | foreach-object {

			$params = @( $localPath, [bool] $Recurse, $Encoding, $LockLevel)
			[type[]] $types = $params | foreach-object { $_.GetType() }

			# Work around that passing $null results in an empty string.
			if ( -not $Encoding ) { $params[2] = $null }
			[void] $_.GetType().GetMethod("PendAdd", $types).Invoke($_, $params)

			if ( $PassThru )
			{
				$_.GetPendingChanges( $localPath, $recursive, $true )
			}
		}
	}
}

# Command: tf checkout
function Edit-TFItem
{
<#

.Synopsis
Checks out an item or items for editing.

.Description
Checks out one or more items for editing. You can lock the files
and change the encoding.

.Parameter Path
The mapped item(s) to edit.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The mapped item(s) to edit.
This parameter does not expand wildcards.

.Parameter Encoding
The new encoding for the file. The default value of $null leaves
the file encoding as is.

.Parameter LockLevel
The lock type to put on the file(s). Values include locking for
"Checkin", "Checkout", "Unchanged", or the default value of "None".

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

.Parameter PassThru
Whether to return status for edited items. The default returns nothing.

#>
	[CmdletBinding(ConfirmImpact="Low", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, Mandatory=$true, ValueFromPipeline=$true)]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath,

		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[string] $Encoding,

		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[Microsoft.TeamFoundation.VersionControl.Client.LockLevel] $LockLevel = "None",

		[Parameter()]
		[switch] $Recurse,

		[Parameter()]
		[switch] $PassThru
	)

	begin
	{
		$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			$LiteralPath = resolve-path $Path
		}

		[string[]] $localPath = convert-path -literalPath $LiteralPath | select-object -unique
		get-tfworkspace -literalPath $localPath | foreach-object {

			$params = @( $localPath, $recursive, $Encoding, $LockLevel)
			[type[]] $types = $params | foreach-object { $_.GetType() }

			# Work around that passing $null results in an empty string.
			if ( -not $Encoding ) { $params[2] = $null }
			[void] $_.GetType().GetMethod("PendEdit", $types).Invoke($_, $params)

			if ( $PassThru )
			{
				$_.GetPendingChanges( $localPath, $recursive, $true )
			}
		}
	}
}

# Command: tf get
function Sync-TFItem
{
<#

.Synopsis
Gets an item or items from the Team Foundation server.

.Description
Fetches items in all workspaces specified by the given path(s).
You can specify multiple paths in multiple workspaces to get all
items on a machine. By default this will only fetch items in the
current directory or the specified path, so pass -Recurse to
get all items in all sub-directories as well.

.Parameter Path
The mapped path to synchronize with the version control server.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The mapped path to synchronize with the version control server.
This parameter does not expand wildcards.

.Parameter Version
The version of the item you want to get.

.Parameter Owner
Name of the owner for a workspace version specification. The default
is the current workspace owner name.

.Parameter All
Gets all items in the workspace.

.Parameter Overwrite
Replace writable files.

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

#>
	[CmdletBinding(ConfirmImpact="Low", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, ValueFromPipeline=$true)]
		[ValidateNotNullOrEmpty()]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath,

		[Parameter(Position=1, ValueFromPipelineByPropertyName=$true)]
		[string] $Version = [Microsoft.TeamFoundation.VersionControl.Client.LatestVersionSpec]::Identifier,

		[Parameter()]
		[string] $Owner,

		[Parameter()]
		[switch] $All,

		[Parameter()]
		[switch] $Overwrite,

		[Parameter()]
		[switch] $Recurse
	)

	begin
	{
		$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}

		$getOptions = [Microsoft.TeamFoundation.VersionControl.Client.GetOptions]::None
		if ($All)
		{
			$getOptions = [Microsoft.TeamFoundation.VersionControl.Client.GetOptions]::GetAll
		}
		if ($Overwrite)
		{
			$getOptions = [Microsoft.TeamFoundation.VersionControl.Client.GetOptions]::Overwrite
		}

		$requestType = [Microsoft.TeamFoundation.VersionControl.Client.GetRequest]
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			# Sync defaults to all items in the current file system directory.
			$LiteralPath = get-childitem $Path
		}

		[string[]] $localPath = convert-path -literalPath $LiteralPath | select-object -unique
		get-tfworkspace -literalPath $localPath | foreach-object {

			# Default to the current workspace owner.
			if ( -not $Owner )
			{
				$Owner = $_.OwnerName
			}

			$versionSpec = [Microsoft.TeamFoundation.VersionControl.Client.VersionSpec]::ParseSingleSpec(
				$Version,
				$Owner)

			$requests = [Microsoft.TeamFoundation.VersionControl.Client.GetRequest]::FromStrings(
				$localPath,
				$recursive,
				$versionSpec)

			$_.Get( $requests, $getOptions )
		}
	}
}

# Command: tf undo
function Undo-TFItem
{
<#

.Synopsis
Undoes the adds, branches, deletes, edits, and other current operations.

.Description
For each given path, any operations such as adds, branches, deletes,
edits, and other operations are undone. Modified items are updated
on disk after changes are undone. Lost changes cannot be recovered,
so you should shelve your changes if you do not want to lose them.

.Parameter Path
The item(s) to undo current operations.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The item(s) to undo current operations.
This parameter does not expand wildcards.

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

#>
	[CmdletBinding(ConfirmImpact="High", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, Mandatory=$true, ValueFromPipeline=$true)]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath,

		[Parameter()]
		[switch] $Recurse
	)

	begin
	{
		$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			$LiteralPath = resolve-path $Path
		}

		[string[]] $localPath = convert-path -literalPath $LiteralPath | select-object -unique
		get-tfworkspace -literalPath $localPath | foreach-object { [void] $_.Undo( $localPath, $recursive, $true ) }
	}
}

# Command: tf checkin
function Publish-TFChange
{
<#
.Synopsis
Check in pending changes on this machine.

.Description
For all pending changes on the machine, changes are checked into
each associated workspace with an optional comment.

.Parameter Path
The item(s) to undo current operations.
The default path is the current resolved path on the file system.
This parameter expands wildcards.

.Parameter LiteralPath
The item(s) to undo current operations.
This parameter does not expand wildcards.

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

.Parameter Comment
Optional comment to add for the checkin.

#>
	[CmdletBinding(ConfirmImpact="None", DefaultParameterSetName="Path")]
	param
	(
		[Parameter(ParameterSetName="Path", Position=0, Mandatory=$true, ValueFromPipeline=$true)]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName="LiteralPath", Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
		[Alias("PSPath")]
		[string[]] $LiteralPath,

		[Parameter()]
		[switch] $Recurse,

		[Parameter()]
		[string] $Comment
	)
	
	begin
	{
		$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recursive = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}
	}

	process
	{
		if ( $PSCmdlet.ParameterSetName -eq "Path" )
		{
			# Sync defaults to all items in the current file system directory.
			$LiteralPath = get-childitem $Path
		}

		[string[]] $localPath = convert-path -literalPath $LiteralPath | select-object -unique
		get-tfworkspace -literalPath $localPath | foreach-object {

			$changes = $_.GetPendingChanges( $localPath, $recursive, $true )
			$_.CheckIn( $changes, $Comment )
		}
	}
}

export-modulemember -function *
