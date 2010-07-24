#Requires -Version 2.0

<#
.Synopsis
Test-Progress

.Description
Tests the following progress smoothing algorithms:

* Cumulative Average
* Modified Moving Average

.Parameter Minimum
The minimum number of steps. The default is 2.

.Parameter Maximum
The maximum number of steps. The default is 5.

.Parameter Sleep
The amount of time to sleep in milliseconds. The default is 100.

.Link
write-progress
#>

[CmdletBinding(ConfirmImpact="None")]
param
(
    [Parameter()]
    [ValidateRange(0, 2147483647)]
    [int] $Minimum = 2,

    [Parameter(Position = 0)]
    [ValidateRange(1, 2147483647)]
    [ValidateScript({$_ -ge $Minimum})]
    [int] $Maximum = 5,

    [Parameter()]
    [ValidateRange(0, 2147483647)]
    [int] $Sleep = 100
)

begin
{
    # Import localized strings and formats.
	$Loc = data {
	    convertfrom-stringdata @'
			ActivityCA = Cumulative Average
            ActivityMMA = Modified Moving Average
			Status = Outer {0} of {1}
			Current = Inner {0} of {1}
            Completed = Completed
'@
	}
	import-localizeddata -bindingVariable Loc -errorAction SilentlyContinue

    $Rand = new-object Random

    $IdCA = $Rand.Next()
    $IdMMA = $Rand.Next()
}

process
{
    # Initial time span for each step assuming maximum integral steps.
    $TimeAvgCA = $TimeAvgMMA = [TimeSpan]::FromMilliseconds($Maximum * $Sleep)

    # Range of outer steps are known.
	$OuterMax = $Maximum
	for ($OuterIndex = 0; $OuterIndex -lt $OuterMax; $OuterIndex++)
	{
        $TimeStart = [DateTime]::Now

        # Range of inner steps are known only in each outer step.
	    $InnerMax = $Rand.Next($Minimum, $Maximum)
	    for ($InnerIndex = 0; $InnerIndex -lt $InnerMax; $InnerIndex++)
	    {
            $TimeRemainsCA = [TimeSpan]::FromMilliseconds($TimeAvgCA.TotalMilliseconds * ($OuterMax - $OuterIndex))
            $TimeRemainsMMA = [TimeSpan]::FromMilliseconds($TimeAvgMMA.TotalMilliseconds * ($OuterMax - $OuterIndex))

            # Do not factor out 100 if using integers or precision is lost from casting.
            $Percent = (100 * $OuterIndex / $OuterMax + 100 * $InnerIndex / ($OuterMax * $InnerMax))

            # Progress for the cumulative average
	        write-progress -id $IdCA -activity $Loc.ActivityCA -status ($Loc.Status -f $OuterIndex, $OuterMax) `
	            -current ($Loc.Current -f $InnerIndex, $InnerMax) -percent $Percent `
                -seconds $TimeRemainsCA.TotalSeconds

            # Progress for the modified moving average
	        write-progress -id $IdMMA -activity $Loc.ActivityMMA -status ($Loc.Status -f $OuterIndex, $OuterMax) `
	            -current ($Loc.Current -f $InnerIndex, $InnerMax) -percent $Percent `
                -seconds $TimeRemainsMMA.TotalSeconds
	        start-sleep -m $Sleep
	    }
            
        $Span = [DateTime]::Now - $TimeStart

        # Update the cumulative average from the previous step time.
        $script:TimeAvgCA = [TimeSpan]::FromMilliseconds(
            ($Span.TotalMilliseconds + $OuterIndex * $TimeAvgCA.TotalMilliseconds) / ($OuterIndex + 1)
        )

        # Update the modified moving average from the previous step time.
        $script:TimeAvgMMA = [TimeSpan]::FromMilliseconds(
            (($OuterMax - 1) * $TimeAvgMMA.TotalMilliseconds + $Span.TotalMilliseconds) / $OuterMax
        )
	}
}

end
{
	write-progress -id $IdCA -activity $Loc.ActivityCA -status $Loc.Completed -completed
	write-progress -id $IdMMA -activity $Loc.ActivityMMA -status $Loc.Completed -completed
}
