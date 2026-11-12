using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace FluidCombat;

public sealed class ModEntry : Mod
{
    /// <summary>How long a button must be held before it starts repeating, so a normal click doesn't trigger twice.</summary>
    private const int HoldRepeatDelayTicks = 12;

    private ModConfig config = new();

    private readonly HashSet<SButton> heldAttackButtons = new();
    private readonly HashSet<SButton> heldSpecialButtons = new();
    private int attackHeldTicks;
    private int specialHeldTicks;

    public override void Entry(IModHelper helper)
    {
        this.config = helper.ReadConfig<ModConfig>();
        CombatState.Config = this.config;

        var harmony = new Harmony(this.ModManifest.UniqueID);
        FarmerPatches.Apply(harmony);
        WeaponPatches.Apply(harmony);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        GenericModConfigMenuIntegration.Register(this.Helper, this.ModManifest, () => this.config, this.ResetConfig, this.SaveConfig);
    }

    private void ResetConfig()
    {
        this.config = new ModConfig();
        CombatState.Config = this.config;
    }

    private void SaveConfig()
    {
        this.Helper.WriteConfig(this.config);
        CombatState.Config = this.config;
    }

    /// <summary>
    /// SMAPI raises input events before the game processes input for the frame, so the current
    /// action can be cancelled here and the new one started before vanilla rejects the click.
    /// </summary>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        if (e.Button.IsUseToolButton())
            this.heldAttackButtons.Add(e.Button);
        if (e.Button.IsActionButton())
            this.heldSpecialButtons.Add(e.Button);

        Farmer player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe() || !CombatState.CanAct(player))
            return;

        if (!player.UsingTool)
            return;

        if (weapon.isOnSpecial)
        {
            if (this.config.AttackCancelsSpecial && e.Button.IsUseToolButton())
            {
                this.CancelCurrentAction(player, weapon);
                this.StartNormalAttack(player);
                this.Helper.Input.Suppress(e.Button);
            }
        }
        else if (this.config.SpecialCancelsAttack && e.Button.IsActionButton() && CombatState.CanStartSpecial(weapon))
        {
            this.CancelCurrentAction(player, weapon);
            weapon.animateSpecialMove(player);
            this.Helper.Input.Suppress(e.Button);
        }
    }

    /// <summary>Keep attacking or using the special while the button is held.</summary>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        this.attackHeldTicks = this.UpdateHeldButtons(this.heldAttackButtons, this.attackHeldTicks);
        this.specialHeldTicks = this.UpdateHeldButtons(this.heldSpecialButtons, this.specialHeldTicks);

        bool repeatAttack = this.config.HoldToRepeatAttack && this.attackHeldTicks > HoldRepeatDelayTicks;
        bool repeatSpecial = this.config.HoldToRepeatSpecial && this.specialHeldTicks > HoldRepeatDelayTicks;
        if (!repeatAttack && !repeatSpecial)
            return;

        Farmer player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe() || !CombatState.CanAct(player))
            return;

        // chain into the next action when idle, or at the point where vanilla accepts a follow-up click
        bool idle = !player.UsingTool && player.CanMove;
        bool attackEnding = player.UsingTool && !weapon.isOnSpecial && player.FarmerSprite.currentAnimationIndex > GetComboFrame(weapon);
        if (!idle && !attackEnding)
            return;

        // the special has a cooldown, so use it whenever it's ready and attack in between
        if (repeatSpecial && CombatState.CanStartSpecial(weapon))
        {
            if (!idle)
                this.CancelCurrentAction(player, weapon);
            weapon.animateSpecialMove(player);
        }
        else if (repeatAttack)
        {
            if (!idle)
                this.CancelCurrentAction(player, weapon);
            this.StartNormalAttack(player);
        }
    }

    /// <summary>Drop released buttons and return the updated number of ticks any of them has been held.</summary>
    private int UpdateHeldButtons(HashSet<SButton> buttons, int heldTicks)
    {
        buttons.RemoveWhere(button => !this.IsButtonHeld(button));
        return buttons.Count > 0 ? heldTicks + 1 : 0;
    }

    /// <summary>Whether the player is physically holding a button.</summary>
    /// <remarks>
    /// SMAPI reports a suppressed button as released for as long as it's held, but keeps it
    /// suppressed until it's actually let go, so a suppressed button is still being held.
    /// </remarks>
    private bool IsButtonHeld(SButton button)
    {
        return this.Helper.Input.GetState(button) is SButtonState.Pressed or SButtonState.Held
            || this.Helper.Input.IsSuppressed(button);
    }

    /// <summary>The animation frame after which <see cref="MeleeWeapon.leftClick"/> restarts the swing.</summary>
    private static int GetComboFrame(MeleeWeapon weapon)
    {
        return weapon.type.Value == MeleeWeapon.dagger ? 0 : 5;
    }

    /// <summary>Stop the current swing or special move and return the player to a neutral state.</summary>
    private void CancelCurrentAction(Farmer player, MeleeWeapon weapon)
    {
        if (weapon.type.Value == MeleeWeapon.dagger)
            MeleeWeapon.daggerHitsLeft = 0;

        // drop any queued follow-up swing so it doesn't interrupt the new action
        this.Helper.Reflection.GetField<bool>(weapon, "anotherClick").SetValue(false);

        player.completelyStopAnimatingOrDoingAction();
    }

    /// <summary>Start a normal attack the same way the game does when clicking while idle (the swing patch turns it toward the cursor).</summary>
    private void StartNormalAttack(Farmer player)
    {
        Vector2 position = Game1.wasMouseVisibleThisFrame
            ? new Vector2(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y)
            : player.GetToolLocation();

        player.lastClick = new Vector2((int)position.X, (int)position.Y);
        player.BeginUsingTool();
    }
}
