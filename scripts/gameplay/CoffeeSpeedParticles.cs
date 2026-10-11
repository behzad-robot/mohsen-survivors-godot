using Godot;
using System.Collections.Generic;

public partial class CoffeeSpeedParticles : Node2D
{
    [Export(PropertyHint.Range, "1,50,1")] public float DotsPerSecond { get; set; } = 18f;
    [Export(PropertyHint.Range, "5,60,1")] public float HorizontalSpread { get; set; } = 23f;
    [Export(PropertyHint.Range, "0.1,2,0.05")] public float DotLifetime { get; set; } = 0.75f;
    [Export(PropertyHint.Range, "5,150,1")] public float RiseSpeed { get; set; } = 58f;
    [Export] public Color DotColor { get; set; } = new(1f, 0.88f, 0.62f, 0.72f);

    private sealed class Dot
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;
        public float Age;
    }

    private readonly List<Dot> _dots = new();
    private readonly RandomNumberGenerator _random = new();
    private float _spawnFraction;
    private bool _emitting;

    public override void _Ready() => _random.Randomize();

    public void SetEmitting(bool enabled)
    {
        if (_emitting == enabled) return;
        _emitting = enabled;
        if (enabled) return;
        _dots.Clear();
        _spawnFraction = 0f;
        QueueRedraw();
    }

    public override void _Process(double delta)
    {
        if (!_emitting || (GetParent() is Player player && player.Game?.IsImpactPaused == true)) return;
        float dt = (float)delta;
        for (int i = _dots.Count - 1; i >= 0; i--)
        {
            Dot dot = _dots[i];
            dot.Age += dt;
            if (dot.Age >= DotLifetime) { _dots.RemoveAt(i); continue; }
            dot.Position += dot.Velocity * dt;
        }
        _spawnFraction += Mathf.Max(0f, DotsPerSecond) * dt;
        while (_spawnFraction >= 1f)
        {
            _spawnFraction -= 1f;
            _dots.Add(new Dot
            {
                Position = new Vector2(_random.RandfRange(-HorizontalSpread, HorizontalSpread), _random.RandfRange(12f, 29f)),
                Velocity = new Vector2(_random.RandfRange(-8f, 8f), -RiseSpeed * _random.RandfRange(0.8f, 1.2f)),
                Radius = _random.RandfRange(1.2f, 2.1f)
            });
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (Dot dot in _dots)
        {
            float opacity = DotColor.A * (1f - dot.Age / Mathf.Max(0.01f, DotLifetime));
            DrawCircle(dot.Position, dot.Radius, new Color(DotColor.R, DotColor.G, DotColor.B, opacity));
        }
    }
}
