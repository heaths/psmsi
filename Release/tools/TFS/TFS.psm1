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

function Get-TFSWorkspaceInfo
{
<#

.Synopsis
Gets information about the workspace for the specified path(s).

.Parameter Path
The mapped path for which workspace information is retrieved.
The default path is the current resolved path on the file system.

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
		$workspaceType = [Microsoft.TeamFoundation.VersionControl.Client.Workstation]

		# Update the workspace cache for each (specified) server.
		get-tfsserver $Server | foreach-object {
			$vcs = $_.GetService([Microsoft.TeamFoundation.VersionControl.Client.VersionControlServer])
			if ($_.HasAuthenticated)
			{
				$workspaceType::Current.EnsureUpdateWorkspaceInfoCache($vcs, $vcs.AuthenticatedUser)
			}
		}
	}

	process
	{
		if ($PSCmdlet.ParameterSetName -eq "Path")
		{
			get-item $Path | convert-path | foreach-object {

				write-verbose "Getting workspace information for path, $_."
				$workspaceType::Current.GetLocalWorkspaceInfo($_)
			}
		}
		else
		{
			$workspaceType::Current.GetAllLocalWorkspaceInfo()
		}
	}
}

# Command: tf status
function Get-TFSStatus
{
<#

.Synopsis
Gets the status of pending changes.

#>

	[CmdletBinding(ConfirmImpact = "None")]
	param
	(
		[Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
		[Alias("PSPath")]
		[string[]] $Path,

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
			get-tfsworkspaceinfo $localPath | foreach-object {

				if ( -not $_ )
				{
					$message = $loc.Error_NoWorkspace -f $localPath
					$action = $loc.Action_MapWorkspace -f $localPath

					write-error -message $message -category ResourceUnavailable -recommendedaction $action
				}
				else
				{
					$_.GetPendingChanges($_, $recurseType, $true)
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

#>

	[CmdletBinding(ConfirmImpact = "Low", SupportsShouldProcess = $true)]
	param
	(
		[Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
		[Alias("PSPath")]
		[string[]] $Path,

		[Parameter()]
		[string] $Version,

		[Parameter()]
		[switch] $All,

		[Parameter()]
		[switch] $Overwrite,

		[Parameter()]
		[switch] $Recurse
	)

	process
	{
		$versionSpec = [Microsoft.TeamFoundation.VersionControl.Client.VersionSpec]::Parse($Version, $env:username)
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

		# Get the workstation for the current file system path.

		foreach ($p in $Path)
		{
			$itemSpec = new-object Microsoft.TeamFoundation.VersionControl.Client.ItemSpec $p, $recurseType
		}
	}
}

export-modulemember -function *
