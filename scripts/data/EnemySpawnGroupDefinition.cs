using Godot;

[GlobalClass]
public partial class EnemySpawnGroupDefinition : Resource
{
    [Export] public PackedScene EnemyScene { get; set; }
    [Export] public PackedScene AlternateEnemyScene { get; set; }
    [Export(PropertyHint.Range, "0,100,1")] public int Count { get; set; } = 1;
    [Export(PropertyHint.Range, "0.1,10,0.05")] public float HealthMultiplier { get; set; } = 1f;
    [Export(PropertyHint.Range, "0.1,10,0.05")] public float SpeedMultiplier { get; set; } = 1f;
    [Export(PropertyHint.Range, "-1,1,0.05")] public float KnockbackResistanceAdjustment { get; set; }
}
