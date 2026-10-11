using Godot;

public partial class ConsumablePickup : Node2D
{
    public enum PickupKind { Water, Coffee }
    [Export] public PickupKind Kind { get; set; }
    [Export(PropertyHint.Range, "0.1,1,0.05")] public float FlightSeconds { get; set; } = 0.48f;
    [Export(PropertyHint.Range, "0,100,1")] public float ArcHeight { get; set; } = 42f;
    [Export(PropertyHint.Range, "0,30,1")] public float LandingBounceHeight { get; set; } = 10f;
    [Export(PropertyHint.Range, "0.2,2,0.05")] public float VisualScale { get; set; } = 1f;
    [Export(PropertyHint.Range, "0,300,1")] public float CoffeeSpeedBonusPercent { get; set; } = 50f;
    [Export(PropertyHint.Range, "0.1,30,0.1")] public float CoffeeDurationSeconds { get; set; } = 5f;
    [Export] public float InitialMagnetSpeed { get; set; } = 80f;
    [Export] public float MagnetAcceleration { get; set; } = 1800f;
    [Export] public float MaximumMagnetSpeed { get; set; } = 900f;
    [Export] public float CollectionDistance { get; set; } = 14f;
    [Export] public float SpringAwaySeconds { get; set; } = 0.12f;
    [Export] public float SpringAwayDistance { get; set; } = 18f;
    public GameController Game { get; set; }
    private Sprite2D _visual;
    private Vector2 _origin;
    private Vector2 _landing;
    private float _flightAge;
    private float _bounceAge;
    private bool _flying;
    private bool _bouncing;
    private bool _collected;
    private bool _magnetized;
    private float _magnetSpeed;
    private float _springAge;
    private Vector2 _springOrigin;
    private Vector2 _springDirection;

    public override void _Ready()
    {
        _visual = GetNode<Sprite2D>("Visual");
        _visual.Scale = Vector2.One * VisualScale;
    }

    public void Launch(Vector2 origin, Vector2 landing)
    {
        _origin = origin;
        _landing = landing;
        Position = origin;
        _flightAge = 0f;
        _flying = true;
        _bouncing = false;
        _magnetized = false;
        _springAge = 0f;
    }

    public override void _Process(double delta)
    {
        if (Game == null || !Game.WaveActive || Game.IsImpactPaused) return;
        float dt = (float)delta;
        if (_flying)
        {
            _flightAge += dt;
            float t = Mathf.Clamp(_flightAge / Mathf.Max(0.01f, FlightSeconds), 0f, 1f);
            Position = _origin.Lerp(_landing, t);
            _visual.Position = new Vector2(0f, -ArcHeight * 4f * t * (1f - t));
            _visual.Scale = Vector2.One * VisualScale * Mathf.Lerp(0.65f, 1f, t);
            if (t >= 1f) { _flying = false; _bouncing = true; _bounceAge = 0f; }
            return;
        }
        if (_bouncing)
        {
            _bounceAge += dt;
            float t = Mathf.Clamp(_bounceAge / 0.24f, 0f, 1f);
            _visual.Position = new Vector2(0f, -LandingBounceHeight * Mathf.Sin(t * Mathf.Pi));
            _visual.Scale = Vector2.One * VisualScale * (1f + 0.12f * Mathf.Sin(t * Mathf.Pi));
            if (t >= 1f) { _bouncing = false; _visual.Position = Vector2.Zero; _visual.Scale = Vector2.One * VisualScale; }
            return;
        }
        if (_collected || Game.Mohsen == null || !Game.Mohsen.Active) return;
        Vector2 target = Game.Mohsen.Position;
        if (!_magnetized)
        {
            float collectionRadius = Game.Mohsen.CoinPickupRadius;
            if (Position.DistanceSquaredTo(target) > collectionRadius * collectionRadius) return;
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
        if (Position.DistanceSquaredTo(target) > CollectionDistance * CollectionDistance) return;
        _collected = true;
        Game.CollectConsumable(this);
        QueueFree();
    }
}
