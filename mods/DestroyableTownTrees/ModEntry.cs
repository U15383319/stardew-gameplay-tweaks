using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace DestroyableTownTrees;

internal sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Harmony harmony = new(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(
                typeof(Tree),
                nameof(Tree.performToolAction),
                new[] { typeof(Tool), typeof(int), typeof(Vector2) }
            ),
            prefix: new HarmonyMethod(typeof(ModEntry), nameof(BeforeTreePerformToolAction)),
            postfix: new HarmonyMethod(typeof(ModEntry), nameof(AfterTreePerformToolAction))
        );

        Monitor.Log("Town tree chopping is enabled.", LogLevel.Info);
    }

    private static void BeforeTreePerformToolAction(Tree __instance, Vector2 tileLocation, out bool __state)
    {
        __state = false;

        if (__instance.Location is not Town town)
        {
            return;
        }

        if (tileLocation.X >= 100 || __instance.isTemporaryGreenRainTree.Value)
        {
            return;
        }

        int pathTileIndex = town.getTileIndexAt((int)tileLocation.X, (int)tileLocation.Y, "Paths", null);
        if (pathTileIndex is not (9 or 10 or 11))
        {
            return;
        }

        __state = true;
        __instance.isTemporaryGreenRainTree.Value = true;
    }

    private static void AfterTreePerformToolAction(Tree __instance, bool __state)
    {
        if (__state)
        {
            __instance.isTemporaryGreenRainTree.Value = false;
        }
    }
}
