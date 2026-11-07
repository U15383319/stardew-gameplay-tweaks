Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$RepositoryRoot = Split-Path -Parent $PSScriptRoot

# Runtime files are explicit so personal files, source code and game DLLs
# cannot accidentally enter release archives.
$ModCatalog = [ordered]@{
    SleepOnAnyBed = @{ Project = 'SleepOnAnyBed.csproj'; Files = @('manifest.json', 'README.md', 'LICENSE') }
    MuchBiggerBackpack = @{ Project = 'BiggerBackpack.csproj'; Files = @('manifest.json', 'README.md', 'LICENSE', 'backpack.png', 'JunimoNote.png', 'i18n/*.json') }
    EventLookupReadableLocations = @{ Project = $null; Files = @('manifest.json', 'README.md', 'LICENSE', 'content.json', 'i18n/*.json') }
}

function Get-SelectedMods([string] $Name) {
    if ($Name -eq 'All') { return @($ModCatalog.Keys) }
    return @($Name)
}
