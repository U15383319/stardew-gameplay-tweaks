[CmdletBinding()]
param(
    [ValidateSet('All', 'SleepOnAnyBed', 'MuchBiggerBackpack')]
    [string] $Mod = 'All',
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

. "$PSScriptRoot/common.ps1"
# Always build first; never silently package a stale DLL.
& "$PSScriptRoot/build.ps1" -Mod $Mod -Configuration $Configuration

$artifactRoot = Join-Path $RepositoryRoot 'artifacts'
New-Item -ItemType Directory -Path $artifactRoot -Force | Out-Null
Add-Type -AssemblyName System.IO.Compression.FileSystem

foreach ($name in (Get-SelectedMods $Mod)) {
    $definition = $ModCatalog[$name]
    $source = Join-Path $RepositoryRoot "mods/$name"
    $manifest = Get-Content -LiteralPath (Join-Path $source 'manifest.json') -Raw | ConvertFrom-Json
    if ($manifest.Version -notmatch '^[0-9A-Za-z.+-]+$') { throw "Invalid version for ${name}." }
    $archivePath = Join-Path $artifactRoot "$name-$($manifest.Version).zip"
    $temporaryArchive = Join-Path $artifactRoot "$([guid]::NewGuid()).tmp"
    $files = [ordered]@{}
    foreach ($pattern in $definition.Files) {
        $matches = @(Get-ChildItem -Path (Join-Path $source $pattern) -File)
        if ($matches.Count -eq 0) { throw "Missing package file: $name/$pattern" }
        foreach ($file in $matches) {
            $relative = $file.FullName.Substring($source.Length + 1).Replace('\', '/')
            $files["$name/$relative"] = $file.FullName
        }
    }
    if ($definition.Project) {
        $output = Join-Path $source "bin/$Configuration"
        $entryDll = Join-Path $output $manifest.EntryDll
        if (!(Test-Path -LiteralPath $entryDll)) { throw "Missing compiled DLL: $entryDll" }
        $files["$name/$($manifest.EntryDll)"] = $entryDll
        $stem = [IO.Path]::GetFileNameWithoutExtension($manifest.EntryDll)
        foreach ($suffix in @('.pdb', '.deps.json')) {
            $extra = Join-Path $output "$stem$suffix"
            if (Test-Path -LiteralPath $extra) { $files["$name/$stem$suffix"] = $extra }
        }
    }
    try {
        $zip = [IO.Compression.ZipFile]::Open($temporaryArchive, [IO.Compression.ZipArchiveMode]::Create)
        try {
            foreach ($entry in $files.GetEnumerator()) {
                [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $entry.Value, $entry.Key, [IO.Compression.CompressionLevel]::Optimal) | Out-Null
            }
        } finally { $zip.Dispose() }
        Move-Item -LiteralPath $temporaryArchive -Destination $archivePath -Force
    } finally {
        if (Test-Path -LiteralPath $temporaryArchive) { Remove-Item -LiteralPath $temporaryArchive }
    }
    Write-Host "Created $archivePath"
}
