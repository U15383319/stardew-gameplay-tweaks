using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace FluidCombat;

internal static class WeaponPatches
{
    public static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.animateSpecialMove)),
            prefix: new HarmonyMethod(typeof(WeaponPatches), nameof(AnimateSpecialMove_Prefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.setFarmerAnimating)),
            prefix: new HarmonyMethod(typeof(WeaponPatches), nameof(SetFarmerAnimating_Prefix))
        );
    }

    /// <summary>
    /// Turn toward the cursor at the start of every swing: the first click, rapid follow-up clicks
    /// and the swings this mod starts all go through this method.
    /// </summary>
    private static void SetFarmerAnimating_Prefix(MeleeWeapon __instance, Farmer who)
    {
        if (who.IsLocalPlayer && who.CurrentTool == __instance && !__instance.isScythe())
            CombatState.TryFaceCursor(who, out _);
    }

    /// <summary>
    /// Turn toward the cursor before every special move. Patched here rather than in the input
    /// handling so it also covers the vanilla right-click, without taking over the other uses of
    /// that button.
    /// </summary>
    private static void AnimateSpecialMove_Prefix(MeleeWeapon __instance, Farmer who)
    {
        if (who.IsLocalPlayer && who.CurrentTool == __instance && CombatState.CanStartSpecial(__instance))
            CombatState.TryFaceCursor(who, out _);
    }
}
