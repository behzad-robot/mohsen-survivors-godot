using Godot;

public enum PlayerStat { MaxHealth, MovementSpeed, Knockback, BonusDamagePercent, RangedDamagePercent, MeleeDamagePercent, HealthRegenPerSecond, Spread, BulletPierce, CriticalChancePercent }

[GlobalClass]
public partial class StatModifier : Resource
{
    [Export] public PlayerStat Stat { get; set; } = PlayerStat.BonusDamagePercent;
    // Missing resource values should be neutral; item resources save their amounts explicitly.
    [Export] public float Amount { get; set; } = 0f;
}
