using Godot;

[GlobalClass]
public partial class WaveDefinition : Resource
{
    [Export(PropertyHint.Range, "1,20,1")] public int WaveNumber { get; set; } = 1;
    [Export] public string WaveName { get; set; } = "";
    [Export(PropertyHint.Range, "1,180,1")] public float Duration { get; set; } = 45f;
    [Export(PropertyHint.Range, "0,1,0.01")] public float TankSpawnChance { get; set; }
    [Export(PropertyHint.Range, "0,1,0.01")] public float BossSpawnChance { get; set; }
    [Export(PropertyHint.Range, "0,1,0.01")] public float BossTankChance { get; set; }
    [Export(PropertyHint.Range, "0,180,0.5")] public float BossSpawnTime { get; set; } = 20f;
    [Export] public Vector2 BossSpawnOffset { get; set; } = new(240f, -220f);
    [Export(PropertyHint.Range, "0,5,0.1")] public float BossSpawnWarningSeconds { get; set; } = 1f;
    [Export] public Godot.Collections.Array<SpawnBurstDefinition> Bursts { get; set; } = new();
}
