[CmdletBinding()]
param(
    [ValidateSet('All', 'SleepOnAnyBed', 'MuchBiggerBackpack', 'EventLookupReadableLocations', 'DestroyableTownTrees')]
    [string] $Mod = 'All',
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

. "$PSScriptRoot/common.ps1"

if ($Mod -eq 'EventLookupReadableLocations') {
    Write-Host 'EventLookupReadableLocations is a content pack; no compilation is needed. Use pack.ps1 to package it.'
    return
}

$target = if ($Mod -eq 'All') {
    Join-Path $RepositoryRoot 'Stardew.GameplayTweaks.sln'
} else {
    Join-Path $RepositoryRoot "mods/$Mod/$($ModCatalog[$Mod].Project)"
}

# These small projects do not need worker processes or a persistent compiler server.
& dotnet build $target --configuration $Configuration --nologo -m:1 -p:UseSharedCompilation=false -p:EnableModDeploy=false -p:EnableModZip=false
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit code $LASTEXITCODE)." }
