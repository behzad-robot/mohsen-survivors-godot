using Godot;

public partial class DamageNumber : Node2D
{
    [Export] public float Lifetime { get; set; } = 0.7f;
    [Export] public float RiseDistance { get; set; } = 55f;
    [Export] public float SideDrift { get; set; } = 18f;
    [Export] public Vector2 SpawnJitter { get; set; } = new(8f, 6f);
    [Export] public Color CriticalColor { get; set; } = new(1f, 0.85f, 0.2f);

    private Label _label;
    private Vector2 _origin;
    private float _age;
    private float _drift;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _origin = Position + new Vector2(
            (GD.Randf() * 2f - 1f) * SpawnJitter.X,
            (GD.Randf() * 2f - 1f) * SpawnJitter.Y);
        Position = _origin;
        _drift = GD.Randf() * 2f * SideDrift - SideDrift;
    }

    public void SetDamage(float amount, bool criticalHit)
    {
        _label.Text = Mathf.RoundToInt(Mathf.Max(1f, amount)).ToString();
        _label.AddThemeColorOverride("font_color", criticalHit ? CriticalColor : Colors.White);
    }

    public override void _Process(double delta)
    {
        if (GetParent() is GameController game && game.IsImpactPaused) return;
        _age += (float)delta;
        float progress = Mathf.Clamp(_age / Mathf.Max(0.01f, Lifetime), 0f, 1f);
        // Rise quickly, then ease into a short fade; a brief scale pop makes the hit readable.
        float rise = 1f - (1f - progress) * (1f - progress);
        Position = _origin + new Vector2(_drift * progress, -RiseDistance * rise);
        Scale = Vector2.One * Mathf.Lerp(1.3f, 0.9f, progress);
        float opacity = progress < 0.35f ? 1f : 1f - (progress - 0.35f) / 0.65f;
        Modulate = new Color(1f, 1f, 1f, opacity);
        if (_age >= Lifetime) QueueFree();
    }
}
