using Godot;

public partial class EnemyDeathText : Node2D
{
    [Export] public float Lifetime { get; set; } = 0.75f;
    [Export] public float RiseDistance { get; set; } = 65f;
    [Export] public float SideDrift { get; set; } = 16f;

    private Label _label;
    private Vector2 _origin;
    private float _age;
    private float _drift;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _origin = Position;
        _drift = GD.Randf() * 2f * SideDrift - SideDrift;
        Scale = Vector2.One * 0.55f;
    }

    public void SetMessage(string message) => _label.Text = message;

    public override void _Process(double delta)
    {
        if (GetParent() is GameController game && game.IsImpactPaused) return;
        _age += (float)delta;
        float progress = Mathf.Clamp(_age / Mathf.Max(0.01f, Lifetime), 0f, 1f);
        float pop = progress < 0.18f
            ? Mathf.Lerp(0.55f, 1.4f, progress / 0.18f)
            : Mathf.Lerp(1.4f, 0.9f, (progress - 0.18f) / 0.82f);
        Scale = Vector2.One * pop;
        Position = _origin + new Vector2(_drift * progress, -RiseDistance * (1f - (1f - progress) * (1f - progress)));
        float opacity = progress < 0.35f ? 1f : 1f - (progress - 0.35f) / 0.65f;
        Modulate = new Color(1f, 1f, 1f, opacity);
        if (_age >= Lifetime) QueueFree();
    }
}
