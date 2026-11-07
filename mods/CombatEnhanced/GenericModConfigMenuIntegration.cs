using StardewModdingAPI;

namespace CombatEnhanced;

/// <summary>The subset of the Generic Mod Config Menu API used by this mod.</summary>
public interface IGenericModConfigMenuApi
{
    void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

    void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);

    void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

    void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string>? tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null, string? fieldId = null);
}

internal static class GenericModConfigMenuIntegration
{
    public static void Register(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        var api = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (api is null)
            return;

        ITranslationHelper i18n = helper.Translation;
        api.Register(manifest, reset, save);

        api.AddSectionTitle(manifest, () => i18n.Get("config.section.movement"));
        api.AddBoolOption(manifest,
            () => getConfig().MoveWhileAttacking, value => getConfig().MoveWhileAttacking = value,
            () => i18n.Get("config.move-while-attacking.name"), () => i18n.Get("config.move-while-attacking.tooltip"));
        api.AddBoolOption(manifest,
            () => getConfig().MoveWhileUsingSpecial, value => getConfig().MoveWhileUsingSpecial = value,
            () => i18n.Get("config.move-while-special.name"), () => i18n.Get("config.move-while-special.tooltip"));
        api.AddBoolOption(manifest,
            () => getConfig().MoveWhileUsingSlingshot, value => getConfig().MoveWhileUsingSlingshot = value,
            () => i18n.Get("config.move-while-slingshot.name"), () => i18n.Get("config.move-while-slingshot.tooltip"));
        api.AddNumberOption(manifest,
            () => getConfig().AttackMoveSpeedMultiplier, value => getConfig().AttackMoveSpeedMultiplier = value,
            () => i18n.Get("config.move-speed.name"), () => i18n.Get("config.move-speed.tooltip"),
            min: 0.1f, max: 1.5f, interval: 0.05f, formatValue: value => $"{value:P0}");
        api.AddBoolOption(manifest,
            () => getConfig().ShowMovingHop, value => getConfig().ShowMovingHop = value,
            () => i18n.Get("config.moving-hop.name"), () => i18n.Get("config.moving-hop.tooltip"));

        api.AddSectionTitle(manifest, () => i18n.Get("config.section.cancel"));
        api.AddBoolOption(manifest,
            () => getConfig().SpecialCancelsAttack, value => getConfig().SpecialCancelsAttack = value,
            () => i18n.Get("config.special-cancels-attack.name"), () => i18n.Get("config.special-cancels-attack.tooltip"));
        api.AddBoolOption(manifest,
            () => getConfig().AttackCancelsSpecial, value => getConfig().AttackCancelsSpecial = value,
            () => i18n.Get("config.attack-cancels-special.name"), () => i18n.Get("config.attack-cancels-special.tooltip"));

        api.AddSectionTitle(manifest, () => i18n.Get("config.section.hold"));
        api.AddBoolOption(manifest,
            () => getConfig().HoldToRepeatAttack, value => getConfig().HoldToRepeatAttack = value,
            () => i18n.Get("config.hold-attack.name"), () => i18n.Get("config.hold-attack.tooltip"));
        api.AddBoolOption(manifest,
            () => getConfig().HoldToRepeatSpecial, value => getConfig().HoldToRepeatSpecial = value,
            () => i18n.Get("config.hold-special.name"), () => i18n.Get("config.hold-special.tooltip"));
    }
}
