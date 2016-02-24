param (
	[Parameter(Mandatory=$true)]
	[ValidatePattern("^\d+\.\d+\.(?:\d+\.\d+$|\d+$)")]
	[string]
	$ReleaseVersionNumber
	
)

$PSScriptFilePath = (Get-Item $MyInvocation.MyCommand.Path).FullName

" PSScriptFilePath = $PSScriptFilePath"

$SolutionRoot = Split-Path -Path $PSScriptFilePath -Parent

# load up the global.json so we can find the DNX version
$globalJson = Get-Content -Path $PSScriptRoot\global.json -Raw -ErrorAction Ignore | ConvertFrom-Json -ErrorAction Ignore

if($globalJson)
{
    $dnxVersion = $globalJson.sdk.version
}
else
{
    Write-Warning "Unable to locate global.json to determine using 'latest'"
    $dnxVersion = "latest"
}

$DNU = "dnu"
$DNVM = "dnvm"

# ensure the correct version
& $DNVM install $dnxVersion

# use the correct version
& $DNVM use $dnxVersion

# Make sure we don't have a release folder for this version already
$BuildFolder = Join-Path -Path $SolutionRoot -ChildPath "build";
$ReleaseFolder = Join-Path -Path $BuildFolder -ChildPath "Releases\v$ReleaseVersionNumber";
if ((Get-Item $ReleaseFolder -ErrorAction SilentlyContinue) -ne $null)
{
	Write-Warning "$ReleaseFolder already exists on your local machine. It will now be deleted."
	Remove-Item $ReleaseFolder -Recurse
}

$projects = Get-ChildItem -Path $PSScriptRoot -Filter project.json -recurse
[int]$errors = 0
$workingDir = Get-Location;
foreach($project in $projects)
{
   Set-Location $project.DirectoryName
# Set the version number in package.json
(gc -Path $project) `
	-replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$ReleaseVersionNumber$PreReleaseName" |
	sc -Path $project -Encoding UTF8
# Set the copyright
$DateYear = (Get-Date).year
(gc -Path $project) `
	-replace "(?<=`"copyright`":\s`")[\w\s©]*(?=`",)", "Copyright © Sifiso Shezi $DateYear" |
	sc -Path $project -Encoding UTF8

# Build the proj in release mode

& $DNU restore 2>&1
if (-not $?)
{
	throw "The DNU restore process returned an error code."
}

& $DNU build 2>&1
if (-not $?)
{
	throw "The DNU build process returned an error code."
}

& $DNU pack 2>&1 --configuration Release --out "$ReleaseFolder"
if (-not $?)
{
	throw "The DNU pack process returned an error code."
}
}
#restore location to root folder
Set-Location $workingDir