using Godot;

[GlobalClass]
public partial class ShotgunTierDefinition : Resource
{
    [Export] public float DamagePerPellet { get; set; }
    [Export] public float AttackInterval { get; set; }
    [Export] public float TargetRange { get; set; }
    [Export] public float CritChance { get; set; }
    [Export] public float CritMultiplier { get; set; }
    [Export] public float KnockbackPerPellet { get; set; }
    [Export] public float RangedDamageScaling { get; set; }
    [Export(PropertyHint.Range, "1,16,1")] public int PelletCount { get; set; }
    [Export(PropertyHint.Range, "0,120,1")] public float SpreadDegrees { get; set; }
    [Export] public float PelletSpeed { get; set; }
    [Export] public float PelletLifetimeSeconds { get; set; }

    public WeaponStats Stats => new(DamagePerPellet, AttackInterval, TargetRange,
        CritChance, CritMultiplier, KnockbackPerPellet, RangedDamageScaling);
}
