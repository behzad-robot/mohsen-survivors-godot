using Godot;

[GlobalClass]
public partial class SpawnBurstDefinition : Resource
{
    [Export(PropertyHint.Range, "0,45,0.5")] public float TriggerTime { get; set; }
    [Export(PropertyHint.Range, "0,10,0.1")] public float WarningDuration { get; set; } = 1f;
    [Export] public Godot.Collections.Array<EnemySpawnGroupDefinition> Groups { get; set; } = new();
}
