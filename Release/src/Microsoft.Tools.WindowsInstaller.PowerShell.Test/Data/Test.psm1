function Set-UICulture
{
	param ( [string] $locale )
	[System.Threading.Thread]::CurrentThread.CurrentUICulture = new-object System.Globalization.CultureInfo $locale
}