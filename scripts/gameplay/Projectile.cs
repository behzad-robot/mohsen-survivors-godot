using Godot;
using System.Collections.Generic;

public partial class Projectile : Node2D
{
    [Export] public bool Hostile { get; set; }
    [Export] public bool Knife { get; set; }
    [Export(PropertyHint.Range, "0.05,15,0.05")] public float LifetimeSeconds { get; set; } = 1.5f;
    [Export(PropertyHint.Range, "1,80,1")] public float HitRadius { get; set; } = 21f;
    [Export] public bool EaseSpeed { get; set; }
    [Export(PropertyHint.Range, "0,1,0.05")] public float EndSpeedMultiplier { get; set; } = 0.25f;
    [Export(PropertyHint.Range, "0.5,4,0.1")] public float EaseExponent { get; set; } = 1.5f;
    [Export(PropertyHint.Range, "0.02,0.5,0.01")] public float TrailLifetimeSeconds { get; set; } = 0.16f;

    public GameController Game { get; set; }
    public Vector2 Direction { get; set; }
    [Export(PropertyHint.Range, "0,1000,10")] public float Speed { get; set; }
    public float Damage { get; set; }
    public bool CriticalHit { get; set; }
    public float Knockback { get; set; }
    public float RemainingDistance { get; set; }
    public float RangeAtSpawn { get; set; }
    public PackedScene SourceScene { get; set; }
    public bool CanSpread { get; set; } = true;
    public int PierceCapacity { get; set; }
    public int RemainingPierces { get; set; }

    private readonly HashSet<ulong> _hitEnemies = new();
    private readonly Queue<float> _trailPointTimes = new();
    private float _remainingLife;
    private float _trailClock;
    private Line2D _trail;

    public void IgnoreEnemyForLife(ulong enemyId) => _hitEnemies.Add(enemyId);

    public override void _Ready()
    {
        _remainingLife = LifetimeSeconds;
        Rotation = Direction.Angle();
        GetNodeOrNull<AnimatedSprite2D>("Visual")?.Play("default");
        _trail = GetNodeOrNull<Line2D>("Trail");
        if (_trail != null)
        {
            _trail.TopLevel = true;
            _trail.GlobalPosition = Vector2.Zero;
            var gradient = new Gradient();
            gradient.SetColor(0, new Color(1f, 1f, 1f, 0f));
            gradient.SetColor(1, new Color(1f, 1f, 1f, 0.9f));
            _trail.Gradient = gradient;
            _trail.AddPoint(GlobalPosition);
            _trailPointTimes.Enqueue(0f);
        }
    }

    public override void _Process(double delta)
    {
        if (Game == null || !Game.WaveActive) return;
        if (Game.IsImpactPaused) return;

        float activeDelta = Mathf.Min((float)delta, _remainingLife);
        Vector2 from = Position;
        float lifeProgress = LifetimeSeconds <= 0f ? 1f : 1f - _remainingLife / LifetimeSeconds;
        float speed = EaseSpeed
            ? Mathf.Lerp(Speed, Speed * EndSpeedMultiplier, Mathf.Pow(lifeProgress, EaseExponent))
            : Speed;
        float travel = Mathf.Min(speed * activeDelta, RemainingDistance);
        Vector2 to = from + Direction * travel;
        Position = to;
        UpdateTrail((float)delta);
        RemainingDistance -= travel;
        _remainingLife -= activeDelta;

        if (Hostile)
        {
            if (SegmentHitTime(from, to, Game.Mohsen.Position, HitRadius + 1f) >= 0f)
            {
                Game.Mohsen.Hurt();
                QueueFree();
                return;
            }
        }
        else if (Knife)
        {
            // One thrown knife can hit many enemies, but only once per enemy.
            foreach (Enemy enemy in Game.Enemies.ToArray())
            {
                if (!GodotObject.IsInstanceValid(enemy) || _hitEnemies.Contains(enemy.GetInstanceId())) continue;
                if (EnemyHitTime(from, to, enemy, HitRadius) < 0f) continue;
                _hitEnemies.Add(enemy.GetInstanceId());
                enemy.TakeHit(Damage, Direction, Knockback, CriticalHit);
                if (CriticalHit) Game.RequestKnifeCritPause();
            }
        }
        else
        {
            // Resolve every intersected enemy in travel order, up to the bullet's pierce limit.
            var hits = new List<(Enemy enemy, float time)>();
            foreach (Enemy enemy in Game.Enemies.ToArray())
            {
                if (!GodotObject.IsInstanceValid(enemy) || enemy.Health <= 0f || _hitEnemies.Contains(enemy.GetInstanceId())) continue;
                float hitTime = EnemyHitTime(from, to, enemy, HitRadius);
                if (hitTime >= 0f) hits.Add((enemy, hitTime));
            }
            hits.Sort((a, b) => a.time.CompareTo(b.time));
            foreach (var hit in hits)
            {
                if (!GodotObject.IsInstanceValid(hit.enemy) || hit.enemy.Health <= 0f) continue;
                ulong originalTargetId = hit.enemy.GetInstanceId();
                if (!_hitEnemies.Add(originalTargetId)) continue;
                Vector2 hitPosition = from.Lerp(to, hit.time);
                hit.enemy.TakeHit(Damage, Direction, Knockback, CriticalHit);
                Game.SpawnPlayerBulletHit(hitPosition);
                if (CanSpread && SourceScene != null && Game.Mohsen.Spread > 0)
                    Game.SpawnSpreadBullets(this, hitPosition, originalTargetId);
                if (RemainingPierces <= 0)
                {
                    QueueFree();
                    return;
                }
                RemainingPierces--;
            }
        }

        if (_remainingLife <= 0f || RemainingDistance <= 0f || Position.X < 0f || Position.Y < 0f || Position.X > Game.ArenaSize.X || Position.Y > Game.ArenaSize.Y)
            QueueFree();
    }

    private void UpdateTrail(float delta)
    {
        if (_trail == null) return;
        _trailClock += delta;
        _trail.AddPoint(GlobalPosition);
        _trailPointTimes.Enqueue(_trailClock);
        while (_trailPointTimes.Count > 2 && _trailPointTimes.Peek() < _trailClock - TrailLifetimeSeconds)
        {
            _trailPointTimes.Dequeue();
            _trail.RemovePoint(0);
        }
    }

    private static float SegmentHitTime(Vector2 from, Vector2 to, Vector2 target, float radius)
    {
        Vector2 segment = to - from;
        float lengthSquared = segment.LengthSquared();
        float time = lengthSquared <= 0.0001f ? 0f : Mathf.Clamp((target - from).Dot(segment) / lengthSquared, 0f, 1f);
        Vector2 closest = from + segment * time;
        return closest.DistanceSquaredTo(target) <= radius * radius ? time : -1f;
    }

    private static float EnemyHitTime(Vector2 from, Vector2 to, Enemy enemy, float projectileRadius)
    {
        Vector2 body = enemy.HurtboxHalfExtents;
        if (body.X <= 0f || body.Y <= 0f)
            return SegmentHitTime(from, to, enemy.Position, projectileRadius);

        // Expand the boss's oval by the projectile radius, then intersect the
        // movement segment with that oval in normalized coordinates.
        float rx = body.X + projectileRadius;
        float ry = body.Y + projectileRadius;
        Vector2 start = new((from.X - enemy.Position.X) / rx, (from.Y - enemy.Position.Y) / ry);
        Vector2 finish = new((to.X - enemy.Position.X) / rx, (to.Y - enemy.Position.Y) / ry);
        Vector2 movement = finish - start;
        if (start.LengthSquared() <= 1f) return 0f;
        float a = movement.LengthSquared();
        if (a <= 0.000001f) return -1f;
        float b = 2f * start.Dot(movement);
        float c = start.LengthSquared() - 1f;
        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) return -1f;
        float entry = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
        return entry >= 0f && entry <= 1f ? entry : -1f;
    }
}
