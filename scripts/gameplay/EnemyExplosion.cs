using Godot;

public partial class EnemyExplosion : Node2D
{
    [Export(PropertyHint.Range, "1,60,1")] public float FramesPerSecond { get; set; } = 30f;

    private Sprite2D _visual;
    private float _elapsed;

    public override void _Ready()
    {
        _visual = GetNode<Sprite2D>("Visual");
        _visual.Frame = 0;
    }

    public override void _Process(double delta)
    {
        if (GetParent() is GameController game && game.IsImpactPaused) return;
        _elapsed += (float)delta;
        int frame = Mathf.FloorToInt(_elapsed * Mathf.Max(1f, FramesPerSecond));
        if (frame >= _visual.Hframes)
        {
            QueueFree();
            return;
        }
        _visual.Frame = frame;
    }
}
