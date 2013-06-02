function Set-UICulture
{
	param ( [string] $locale )
	[System.Threading.Thread]::CurrentThread.CurrentUICulture = new-object System.Globalization.CultureInfo $locale
}

# Make sure the working directory contains all our test data.
push-location $TestDeploymentDirectory
