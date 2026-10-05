using Godot;

public partial class Coin : Node2D
{
    [Export] public float InitialMagnetSpeed { get; set; } = 80f;
    [Export] public float MagnetAcceleration { get; set; } = 1800f;
    [Export] public float MaximumMagnetSpeed { get; set; } = 900f;
    [Export] public float CollectionDistance { get; set; } = 14f;
    [Export] public float SpawnGrowSeconds { get; set; } = 0.22f;
    [Export] public float SpringAwaySeconds { get; set; } = 0.12f;
    [Export] public float SpringAwayDistance { get; set; } = 18f;

    public GameController Game { get; set; }
    private bool _magnetized;
    private float _magnetSpeed;
    private float _spawnAge;
    private float _springAge;
    private Vector2 _springOrigin;
    private Vector2 _springDirection;
    private Sprite2D _visual;

    public override void _Ready()
    {
        _visual = GetNode<Sprite2D>("Visual");
        _visual.Scale = Vector2.Zero;
    }

    public override void _Process(double delta)
    {
        if (Game?.IsImpactPaused == true) return;
        float dt = (float)delta;
        _spawnAge += dt;
        float grow = Mathf.Clamp(_spawnAge / Mathf.Max(0.01f, SpawnGrowSeconds), 0f, 1f);
        float visualScale = grow < 0.75f ? Mathf.Lerp(0f, 1.15f, grow / 0.75f) : Mathf.Lerp(1.15f, 1f, (grow - 0.75f) / 0.25f);
        _visual.Scale = Vector2.One * visualScale;
        if (Game == null || !Game.WaveActive || Game.Mohsen == null) return;

        Vector2 target = Game.Mohsen.Position;
        if (!_magnetized)
        {
            float radius = Game.Mohsen.CoinPickupRadius;
            if (Position.DistanceSquaredTo(target) > radius * radius) return;
            _magnetized = true;
            _magnetSpeed = InitialMagnetSpeed;
            _springOrigin = Position;
            _springDirection = (Position - target).Normalized();
            if (_springDirection == Vector2.Zero) _springDirection = Vector2.Right.Rotated(GD.Randf() * Mathf.Tau);
        }

        if (_springAge < SpringAwaySeconds)
        {
            _springAge = Mathf.Min(SpringAwaySeconds, _springAge + dt);
            float progress = _springAge / Mathf.Max(0.01f, SpringAwaySeconds);
            Position = _springOrigin + _springDirection * SpringAwayDistance * Mathf.Sin(progress * Mathf.Pi * 0.5f);
            return;
        }

        _magnetSpeed = Mathf.Min(MaximumMagnetSpeed, _magnetSpeed + MagnetAcceleration * dt);
        Position = Position.MoveToward(target, _magnetSpeed * dt);
        if (Position.DistanceSquaredTo(target) <= CollectionDistance * CollectionDistance)
        {
            Game.CollectCoin(this);
            QueueFree();
        }
    }
}
