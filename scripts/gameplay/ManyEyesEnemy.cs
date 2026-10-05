using Godot;

public partial class ManyEyesEnemy : Enemy
{
    [Export(PropertyHint.Range, "0.3,8,0.1")] public float WanderTurnSeconds { get; set; } = 2.2f;
    [Export(PropertyHint.Range, "0.3,10,0.1")] public float VolleyIntervalSeconds { get; set; } = 2.8f;
    [Export(PropertyHint.Range, "0,10,0.1")] public float InitialVolleyDelaySeconds { get; set; } = 1.4f;
    [Export(PropertyHint.Range, "50,500,10")] public float VolleyProjectileSpeed { get; set; } = 190f;
    [Export(PropertyHint.Range, "0,80,1")] public float ProjectileOriginRadius { get; set; } = 28f;
    [Export] public PackedScene VolleyProjectileScene { get; set; }
    [Export] public PackedScene OffspringScene { get; set; }
    [Export(PropertyHint.Range, "0,16,1")] public int OffspringCount { get; set; } = 6;
    [Export(PropertyHint.Range, "0,100,1")] public float OffspringSpawnRadius { get; set; } = 32f;
    [Export(PropertyHint.Range, "0.1,5,0.1")] public float OffspringHealthMultiplier { get; set; } = 1f;
    [Export(PropertyHint.Range, "0.1,5,0.1")] public float OffspringSpeedMultiplier { get; set; } = 1f;

    private readonly RandomNumberGenerator _random = new();
    private Vector2 _wanderDirection;
    private float _turnRemaining;
    private float _volleyRemaining;

    public override void _Ready()
    {
        base._Ready();
        _random.Randomize();
        ChooseWanderDirection();
        _volleyRemaining = InitialVolleyDelaySeconds;
    }

    protected override void UpdateBehavior(float dt, Vector2 toward, float distance)
    {
        _turnRemaining -= dt;
        if (_turnRemaining <= 0f) ChooseWanderDirection();
        Vector2 before = Position;
        Vector2 displacement = _wanderDirection * MovementSpeed * dt;
        MoveWithinArena(displacement);
        if (displacement.LengthSquared() > 0f && before.DistanceSquaredTo(Position) < displacement.LengthSquared() * 0.25f)
            ChooseWanderDirection();

        _volleyRemaining -= dt;
        if (_volleyRemaining <= 0f)
        {
            FireVolley();
            _volleyRemaining = Mathf.Max(0.3f, VolleyIntervalSeconds);
        }
    }

    private void ChooseWanderDirection()
    {
        _wanderDirection = Vector2.Right.Rotated(_random.RandfRange(0f, Mathf.Tau));
        _turnRemaining = Mathf.Max(0.3f, WanderTurnSeconds) * _random.RandfRange(0.75f, 1.25f);
    }

    private void FireVolley()
    {
        if (VolleyProjectileScene == null) return;
        float start = _random.RandfRange(0f, Mathf.Tau);
        for (int i = 0; i < 8; i++)
        {
            Vector2 direction = Vector2.Right.Rotated(start + Mathf.Tau * i / 8f);
            Game.SpawnManyEyesProjectile(VolleyProjectileScene,
                Position + direction * ProjectileOriginRadius, direction, VolleyProjectileSpeed);
        }
    }

    protected override void OnDefeated()
    {
        if (OffspringScene == null) return;
        int count = Mathf.Clamp(OffspringCount, 0, 16);
        float start = _random.RandfRange(0f, Mathf.Tau);
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Vector2.Right.Rotated(start + Mathf.Tau * i / count) * OffspringSpawnRadius;
            Game.SpawnEnemyOffspring(OffspringScene, Position + offset,
                OffspringHealthMultiplier, OffspringSpeedMultiplier);
        }
    }
}
