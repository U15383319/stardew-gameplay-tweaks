# Stardew Gameplay Tweaks

Small, independently installable mods for Stardew Valley.

| Mod | Features | Dependencies beyond SMAPI |
| --- | --- | --- |
| [SleepOnAnyBed](mods/SleepOnAnyBed) | Sleep in beds outside the farmhouse | None |
| [MuchBiggerBackpack](mods/MuchBiggerBackpack) | Configurable backpack capacity and upgrade price | StardewHack |
| [EventLookupReadableLocations](mods/EventLookupReadableLocations) | Readable English and Chinese location names for vanilla and expansion maps | Content Patcher, Event Lookup |
| [DestroyableTownTrees](mods/DestroyableTownTrees) | Chop normally protected trees in Pelican Town | None |
| [Fluid Combat](mods/FluidCombat) | Move while attacking, cancel attacks and specials, hold to repeat attacks, and face the mouse cursor | Optional: Generic Mod Config Menu |

## Development

Requires a .NET SDK supporting `net6.0` and a SMAPI-enabled game installation. Copy the settings template and set `GamePath` and `StardewHackPath`; local settings are ignored by Git.

```powershell
Copy-Item Directory.Build.local.props.example Directory.Build.local.props
.\scripts\build.ps1                              # Build all C# mods
.\scripts\build.ps1 -Mod FluidCombat           # Build one mod
.\scripts\pack.ps1                               # Create separate ZIPs for all mods
.\scripts\pack.ps1 -Mod EventLookupReadableLocations
```

Scripts default to Release; use `-Configuration Debug` for debug builds. Content packs need no compilation. Open `Stardew.GameplayTweaks.sln` to work with the C# projects.

Build output is in each project's `bin/<Configuration>/`; ZIPs are in `artifacts/`. Scripts do not deploy to the game automatically.

To add a mod, register its directory and package files in `scripts/common.ps1` and update the scripts' `ValidateSet` options. Add C# projects to the solution. Each mod keeps its own version in `manifest.json`.

## Licenses

EventLookupReadableLocations uses [CC BY-NC-SA 4.0](mods/EventLookupReadableLocations/LICENSE). The other four mods use MIT. MuchBiggerBackpack retains its original author credits and copyright notice. See each mod's `LICENSE` for details.
