#Requires -Version 2.0

# PowerShell module for common TFS commands.
#

DATA loc {
	# en-US
	convertfrom-stringdata @'
		Action_MapWorkspace = Map the path, {0}, to a new or existing workspace.
		Error_NoWorkspace = No workspace is mapped to the local path, {0}.
'@
}

# Make sure required assemlies are loaded.
$Version = "Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
[Reflection.Assembly]::Load("Microsoft.TeamFoundation.Client, $Version")
[Reflection.Assembly]::Load("Microsoft.TeamFoundation.VersionControl.Client, $Version")

function Get-TFSServer
{
<#

.Synopsis
Gets or lists TFS servers registered to the machine.

.Description
Gets Team Foundation servers registered to the machine. You can specify a URI
to identify the server by its source location, or pass $Null (default)
to list all Team Foundation servers registered to the machine.

.Parameter Uri
If specified, gets the server(s) for each URI.

#>
	[CmdletBinding(ConfirmImpact = "None")]
	param
	(
		[Parameter(Position = 0)]
		[string[]] $Uri = $null
	)

	process
	{
		if ($Uri)
		{
			foreach ($i in $Uri)
			{
				$server = [Microsoft.TeamFoundation.Client.RegisteredServers]::GetServerForUri($i)
				[Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer($server)
			}
		}
		else
		{
			[Microsoft.TeamFoundation.Client.RegisteredServers]::GetServers()
		}
	}
}

function Get-TFSWorkspace
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

#>
	[CmdletBinding(ConfirmImpact = "None")]
	param
	(
		[Parameter(Position = 0)]
		[ValidateNotNullOrEmpty()]
		[string[]] $Path = $( get-location -psprovider FileSystem )
	)

	process
	{
		get-tfsworkspaceinfo $Path | foreach-object {
			$tfs = get-tfsserver $_.ServerUri
			$_.GetWorkspace($tfs)
		}
	}
}

function Get-TFSWorkspaceInfo
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

.Parameter All
Get all local workspaces.

.Parameter Server
The Team Foundation Server to which a workspace belongs.
The default will query all registered servers on the machine.

#>

	[CmdletBinding(ConfirmImpact = "None", DefaultParameterSetName = "Path")]
	param
	(
		[Parameter(ParameterSetName = "Path", Position = 0)]
		[ValidateNotNullOrEmpty()]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(ParameterSetName = "All")]
		[switch] $All,

		[Parameter(ValueFromPipeline = $true)]
		[Microsoft.TeamFoundation.Client.TeamFoundationServer] $Server = $null
	)

	begin
	{
		$workstation = [Microsoft.TeamFoundation.VersionControl.Client.Workstation]

		# Update the workspace cache for each (specified) server.
		get-tfsserver $Server | foreach-object {
			$vcs = $_.GetService([Microsoft.TeamFoundation.VersionControl.Client.VersionControlServer])
			if ($_.HasAuthenticated)
			{
				$workstation::Current.EnsureUpdateWorkspaceInfoCache($vcs, $vcs.AuthenticatedUser)
			}
		}
	}

	process
	{
		if ($PSCmdlet.ParameterSetName -eq "Path")
		{
			get-item $Path | convert-path | foreach-object {

				write-verbose "Getting workspace information for path, $_."
				$workstation::Current.GetLocalWorkspaceInfo($_)
			}
		}
		else
		{
			$workstation::Current.GetAllLocalWorkspaceInfo()
		}
	}
}

# Command: tf status
function Get-TFSStatus
{
<#

.Synopsis
Gets the status of pending changes.

.Description
Gets the status of all items in all workspaces specified by the given
path(s). You can specify multiple paths in multiple workspaces to get
status of every mapped item on the machine.

.Parameter Path
The mapped path for which workspace information is retrieved.
The default path is the current resolved path on the file system.

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

#>

	[CmdletBinding(ConfirmImpact = "None")]
	param
	(
		[Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
		[Alias("PSPath")]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter()]
		[switch] $Recurse
	)

	begin
	{
		$recurseType = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recurseType = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
		}
	}

	process
	{
		get-childitem $Path | convert-path | foreach-object {

			$localPath = $_

			# Get the workspace from the mapped info.
			get-tfsworkspace $localPath | foreach-object {

				if ( -not $_ )
				{
					$message = $loc.Error_NoWorkspace -f $localPath
					$action = $loc.Action_MapWorkspace -f $localPath

					write-error -message $message -category ResourceUnavailable -recommendedaction $action
				}
				else
				{
					$_.GetPendingChanges($localPath, $recurseType, $true)
				}
			}
		}
	}
}

# Command: tf get
function Get-TFSItem
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
The mapped path for which workspace information is retrieved.
The default path is the current resolved path on the file system.

.Parameter Version
The version of the item you want to get.

.Parameter All
Gets all items in the workspace.

.Parameter Overwrite
Replace writable files.

.Parameter Recurse
Whether to recurse into sub-directories of the specified path(s).

#>

	[CmdletBinding(ConfirmImpact = "Low", SupportsShouldProcess = $true)]
	param
	(
		[Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
		[Alias("PSPath")]
		[string[]] $Path = $( get-location -psprovider FileSystem ),

		[Parameter(Position = 1, ValueFromPipelineByPropertyName = $true)]
		[string] $Version = [Microsoft.TeamFoundation.VersionControl.Client.LatestVersionSpec]::Identifier,

		[Parameter()]
		[switch] $All,

		[Parameter()]
		[switch] $Overwrite,

		[Parameter()]
		[switch] $Recurse
	)

	begin
	{
		$recurseType = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::None
		if ($Recurse)
		{
			$recurseType = [Microsoft.TeamFoundation.VersionControl.Client.RecursionType]::Full
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
		$versionSpec = [Microsoft.TeamFoundation.VersionControl.Client.VersionSpec]::ParseSingleSpec(
			$Version,
			$env:username)

		[string[]] $localPath = convert-path $Path
		$requests = [Microsoft.TeamFoundation.VersionControl.Client.GetRequest]::FromStrings(
			$localPath,
			$recurseType,
			$versionSpec)

		get-tfsworkspace $localPath | foreach-object { $_.Get( $requests, $getOptions ) }
	}
}

export-modulemember -function *
