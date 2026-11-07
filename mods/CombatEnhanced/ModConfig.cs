namespace CombatEnhanced;

public sealed class ModConfig
{
    /// <summary>Allow moving during a normal (left-click) melee attack.</summary>
    public bool MoveWhileAttacking { get; set; } = true;

    /// <summary>Allow moving during a melee weapon special move (right-click).</summary>
    public bool MoveWhileUsingSpecial { get; set; } = true;

    /// <summary>Allow moving while pulling back a slingshot.</summary>
    public bool MoveWhileUsingSlingshot { get; set; } = true;

    /// <summary>Movement speed while attacking, relative to normal walking/running speed.</summary>
    public float AttackMoveSpeedMultiplier { get; set; } = 1f;

    /// <summary>Show the vanilla little hop animation when moving while attacking.</summary>
    public bool ShowMovingHop { get; set; } = false;

    /// <summary>Right-click during a normal attack cancels it and starts the weapon special immediately.</summary>
    public bool SpecialCancelsAttack { get; set; } = true;

    /// <summary>Left-click during a weapon special cancels it and starts a normal attack immediately.</summary>
    public bool AttackCancelsSpecial { get; set; } = true;

    /// <summary>Keep attacking while the attack button is held.</summary>
    public bool HoldToRepeatAttack { get; set; } = true;

    /// <summary>Keep using the weapon special whenever it's off cooldown while the special button is held.</summary>
    public bool HoldToRepeatSpecial { get; set; } = true;
}
