using HarmonyLib;
using StardewValley;

namespace FluidCombat;

/// <summary>
/// The game already lets the player "strafe" while charging a tool: movement input is accepted,
/// the facing direction is locked and the tool animation keeps playing. These patches reuse that
/// mechanism for weapon attacks, special moves and the slingshot.
/// </summary>
internal static class FarmerPatches
{
    public static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.canStrafeForToolUse)),
            postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(CanStrafeForToolUse_Postfix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
            prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(GetMovementSpeed_Prefix)),
            postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(GetMovementSpeed_Postfix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.updateMovementAnimation)),
            prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(UpdateMovementAnimation_Prefix)),
            postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(UpdateMovementAnimation_Postfix))
        );
    }

    /// <summary>Allow movement input and position updates while attacking.</summary>
    private static void CanStrafeForToolUse_Postfix(Farmer __instance, ref bool __result)
    {
        if (!__result && !CombatState.BypassStrafeOverride && CombatState.IsCombatMoving(__instance))
            __result = true;
    }

    /// <summary>Use normal walking speed (times the configured multiplier) instead of the slow vanilla strafe speed.</summary>
    private static void GetMovementSpeed_Prefix(Farmer __instance, out bool __state)
    {
        __state = CombatState.IsCombatMoving(__instance);
        if (__state)
            CombatState.BypassStrafeOverride = true;
    }

    private static void GetMovementSpeed_Postfix(ref float __result, bool __state)
    {
        if (!__state)
            return;

        CombatState.BypassStrafeOverride = false;
        __result *= Math.Max(0f, CombatState.Config.AttackMoveSpeedMultiplier);
    }

    /// <summary>Skip the vanilla strafe hop unless enabled in the config.</summary>
    private static void UpdateMovementAnimation_Prefix(Farmer __instance, out bool __state)
    {
        __state = !CombatState.Config.ShowMovingHop && CombatState.IsCombatMoving(__instance);
        if (__state)
            CombatState.BypassStrafeOverride = true;
    }

    private static void UpdateMovementAnimation_Postfix(bool __state)
    {
        if (__state)
            CombatState.BypassStrafeOverride = false;
    }
}
