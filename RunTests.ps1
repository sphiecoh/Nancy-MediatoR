$VerbosePreference = "Continue"

$projects = Get-ChildItem -Path $PSScriptRoot\test -Filter project.json -recurse

[int]$errors = 0
$workingDir = Get-Location;

foreach($project in $projects)
{
	Write-Output "Test $($project.DirectoryName)"
	Set-Location $project.DirectoryName

	dnx test 2>&1
    $errors = $errors + $lastexitcode

	Set-Location $workingDir
}

if($errors -gt 0)
{
    Throw "Some tests have failed" 
}

exit $errors