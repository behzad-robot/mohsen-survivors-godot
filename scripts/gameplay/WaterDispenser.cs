using Godot;

public partial class WaterDispenser : Node2D
{
    [Export(PropertyHint.Range, "1,30,0.5")] public float DropIntervalSeconds { get; set; } = 10f;
    public GameController Game { get; set; }
    private float _elapsed;

    public override void _Process(double delta)
    {
        if (Game == null || !Game.WaveActive || Game.IsImpactPaused) return;
        _elapsed += (float)delta;
        if (_elapsed < DropIntervalSeconds) return;
        _elapsed -= Mathf.Max(0.1f, DropIntervalSeconds);
        Game.SpawnWater(this);
    }
}
