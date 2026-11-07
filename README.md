# Stardew Gameplay Tweaks

Small, independently installable mods for Stardew Valley.

| Mod | Features | Dependencies beyond SMAPI |
| --- | --- | --- |

## Development

Requires a .NET SDK supporting `net6.0` and a SMAPI-enabled game installation. Copy the settings template and set `GamePath` and `StardewHackPath`; local settings are ignored by Git.

```powershell
Copy-Item Directory.Build.local.props.example Directory.Build.local.props
.\scripts\build.ps1                              # Build all C# mods
.\scripts\pack.ps1                               # Create separate ZIPs for all mods
```

Scripts default to Release; use `-Configuration Debug` for debug builds. Content packs need no compilation. Open `Stardew.GameplayTweaks.sln` to work with the C# projects.

Build output is in each project's `bin/<Configuration>/`; ZIPs are in `artifacts/`. Scripts do not deploy to the game automatically.

To add a mod, register its directory and package files in `scripts/common.ps1` and update the scripts' `ValidateSet` options. Add C# projects to the solution. Each mod keeps its own version in `manifest.json`.

## Licenses

Each mod includes its own license.
