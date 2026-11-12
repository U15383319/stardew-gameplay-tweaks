using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace FluidCombat;

/// <summary>Shared combat checks used by the Harmony patches and input handling.</summary>
internal static class CombatState
{
    private static readonly Func<MeleeWeapon, int> GetSpecialCooldown =
        AccessTools.MethodDelegate<Func<MeleeWeapon, int>>(AccessTools.Method(typeof(MeleeWeapon), "specialCooldown"));

    public static ModConfig Config { get; set; } = new();

    /// <summary>While true, <see cref="Farmer.canStrafeForToolUse"/> returns its vanilla result.</summary>
    public static bool BypassStrafeOverride { get; set; }

    /// <summary>Whether the player is in a state where the game can be told to act on input at all.</summary>
    public static bool CanAct(Farmer player)
    {
        return player.IsLocalPlayer
            && player.currentLocation != null
            && Game1.activeClickableMenu == null
            && Game1.currentMinigame == null
            && Game1.farmEvent == null
            && !Game1.eventUp
            && !Game1.dialogueUp
            && !Game1.fadeToBlack
            && player.health > 0
            && !player.isEating
            && !player.canOnlyWalk
            && !player.onBridge.Value
            && !player.swimming.Value
            && !player.bathingClothes.Value
            && !player.isRidingHorse()
            && !player.IsSitting();
    }

    /// <summary>Whether the local player is attacking in a way this mod lets them move during.</summary>
    public static bool IsCombatMoving(Farmer player)
    {
        if (!CanAct(player) || player.freezePause > 0 || player.noMovementPause > 0)
            return false;

        switch (player.CurrentTool)
        {
            case MeleeWeapon weapon when !weapon.isScythe():
                if (!player.UsingTool)
                    return false;
                return weapon.isOnSpecial ? Config.MoveWhileUsingSpecial : Config.MoveWhileAttacking;

            case Slingshot:
                return player.usingSlingshot && Config.MoveWhileUsingSlingshot;

            default:
                return false;
        }
    }

    /// <summary>Whether <see cref="MeleeWeapon.animateSpecialMove"/> would actually start a special move.</summary>
    public static bool CanStartSpecial(MeleeWeapon weapon)
    {
        // stabbing swords have no special move since 1.6
        if (weapon.isScythe() || weapon.type.Value is not (MeleeWeapon.dagger or MeleeWeapon.club or MeleeWeapon.defenseSword))
            return false;

        return !Game1.fadeToBlack && GetSpecialCooldown(weapon) <= 0;
    }

    /// <summary>
    /// Turn the player toward the mouse cursor, however far away it is. Vanilla only turns toward a
    /// click within one tile of the player, so this is calculated directly.
    /// </summary>
    /// <param name="player">The local player.</param>
    /// <param name="cursor">The cursor position in world pixels, if the mouse is in use.</param>
    /// <returns>Whether the mouse is in use (it's hidden when playing with a gamepad).</returns>
    public static bool TryFaceCursor(Farmer player, out Vector2 cursor)
    {
        cursor = Vector2.Zero;
        if (!Game1.wasMouseVisibleThisFrame)
            return false;

        cursor = new Vector2(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y);

        // measure from the middle of the body instead of the feet
        Vector2 offset = cursor - (player.getStandingPosition() - new Vector2(0, 32));
        player.FacingDirection = Math.Abs(offset.X) > Math.Abs(offset.Y)
            ? (offset.X > 0 ? Game1.right : Game1.left)
            : (offset.Y > 0 ? Game1.down : Game1.up);
        return true;
    }
}
