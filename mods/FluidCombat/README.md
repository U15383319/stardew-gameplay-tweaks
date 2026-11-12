# Fluid Combat

Move while attacking, chain normal attacks and specials, and hold buttons to keep fighting.

## Features

- Move during melee attacks, weapon specials, and slingshot charging.
- Cancel a normal melee attack into a special, or a special into a normal attack. Specials still require their cooldown to be ready.
- Hold the attack button to repeat normal melee attacks. Hold the special button to repeat specials whenever their cooldown is ready.
- Face the mouse cursor at the start of each melee attack or special when using a mouse.
- Adjust movement speed during combat and optionally enable the vanilla movement hop.

Scythes are excluded. Attack cancellation and held-button repetition apply to melee weapons, not slingshots.

## Installation and removal

Requires SMAPI 4.5.2+. Generic Mod Config Menu is optional. English and Chinese translations are included.

1. Install SMAPI.
2. Extract the release ZIP into the game's `Mods` folder, so `Mods/FluidCombat/manifest.json` exists.
3. Launch the game through SMAPI.

If you have an installation under the previous name, remove its old folder before installing Fluid Combat. Copy its `config.json` into the new folder to keep your settings. Do not install both copies.

To uninstall, close the game and remove the `FluidCombat` folder from `Mods`.

## Configuration

Use Generic Mod Config Menu, or launch the game once and edit `config.json` while the game is closed.

| Setting | Default | Effect |
| --- | --- | --- |
| `MoveWhileAttacking` | `true` | Move during normal melee attacks |
| `MoveWhileUsingSpecial` | `true` | Move during weapon specials |
| `MoveWhileUsingSlingshot` | `true` | Move while charging a slingshot |
| `AttackMoveSpeedMultiplier` | `1.0` | Combat movement speed relative to normal movement |
| `ShowMovingHop` | `false` | Show the vanilla movement hop |
| `SpecialCancelsAttack` | `true` | Cancel a normal attack into a ready special |
| `AttackCancelsSpecial` | `true` | Cancel a special into a normal attack |
| `HoldToRepeatAttack` | `true` | Hold to repeat normal attacks |
| `HoldToRepeatSpecial` | `true` | Hold to repeat specials after cooldown |

Mouse-facing behavior is always enabled when using a mouse; it has no configuration toggle.

## Compatibility and support

Other mods that change weapon animations, attack input, movement, or specials may conflict. Compatibility with those mods, controllers, multiplayer, and split-screen still needs in-game verification before being advertised as supported.

For bug reports, include the game version, SMAPI version, mod configuration, steps to reproduce, and a SMAPI log link from https://smapi.io/log.

## Build

Build a local ZIP from the repository root:

```powershell
.\scripts\pack.ps1 -Mod FluidCombat
```

## License

MIT. See [LICENSE](LICENSE).
