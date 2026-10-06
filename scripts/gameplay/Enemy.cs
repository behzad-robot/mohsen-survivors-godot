using Godot;

public partial class Enemy : Node2D
{
    [Export] public bool Ranged { get; set; }
    [Export] public float BaseHealth { get; set; } = 18f;
    [Export] public float BaseSpeed { get; set; } = 82f;
    [Export(PropertyHint.Range, "0,1,0.05")] public float BaseKnockbackResistance { get; set; }
    [Export] public float FireRange { get; set; } = 260f;
    [Export] public float RetreatDistance { get; set; } = 140f;
    [Export] public float AttackInterval { get; set; } = 2.4f;
    [Export] public float ProjectileSpeed { get; set; } = 230f;
    [Export] public float ContactRadius { get; set; } = 27f;
    [Export] public Vector2 HurtboxHalfExtents { get; set; } = Vector2.Zero;
    [Export(PropertyHint.Range, "0.02,0.5,0.01")] public float HitFlashSeconds { get; set; } = 0.07f;
    [Export(PropertyHint.Range, "0.25,5.0,0.01")] public float VisualScale { get; set; } = 1.12f;
    [Export(PropertyHint.Range, "0,0.25,0.01")] public float WalkSquash { get; set; } = 0.10f;
    [Export] public Vector2 MouthOffset { get; set; } = new(0f, 2f);
    [Export(PropertyHint.Range, "0,80,1")] public float ProjectileMuzzleDistance { get; set; } = 12f;

    public GameController Game { get; private set; }
    public float Health { get; private set; }
    private float _speed;
    private float _resistance;
    private float _attackTimer;
    private float _flashTime;
    private bool _defeated;
    private Sprite2D _sprite;
    private ShaderMaterial _flashMaterial;
    protected float MovementSpeed => Game?.Mohsen == null ? _speed : Mathf.Min(_speed, Game.Mohsen.EffectiveMovementSpeed * 0.95f);
    protected virtual float HitWallMargin => 48f;
    protected void SetWarningFlash(float amount) => _flashMaterial?.SetShaderParameter("warning_amount", Mathf.Clamp(amount, 0f, 1f));

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Visual");
        if (_sprite.Material is ShaderMaterial sharedMaterial)
        {
            _flashMaterial = (ShaderMaterial)sharedMaterial.Duplicate();
            _sprite.Material = _flashMaterial;
        }
        _sprite.Scale = Vector2.One * VisualScale;
    }

    public void Configure(GameController game, float healthMultiplier, float speedMultiplier, float resistanceAdjustment)
    {
        Game = game;
        Health = Mathf.Max(1f, BaseHealth * healthMultiplier);
        _speed = Mathf.Max(1f, BaseSpeed * speedMultiplier);
        _resistance = Mathf.Clamp(BaseKnockbackResistance + resistanceAdjustment, 0f, 0.95f);
        _attackTimer = AttackInterval * 0.5f;
    }

    public override void _Process(double delta)
    {
        if (Game?.IsImpactPaused == true) return;
        float dt = (float)delta;
        _flashTime = Mathf.Max(0f, _flashTime - dt);
        _flashMaterial?.SetShaderParameter("flash_amount", _flashTime > 0f ? 1f : 0f);
        if (_defeated)
        {
            if (_flashTime <= 0f) QueueFree();
            return;
        }
        if (Game == null || !Game.WaveActive) return;
        Vector2 toward = Game.Mohsen.Position - Position;
        float distance = toward.Length();
        _sprite.FlipH = toward.X < 0f;
        UpdateBehavior(dt, toward, distance);
        float walkPhase = Mathf.Sin((float)Time.GetTicksMsec() / 115f + (float)(GetInstanceId() % 8));
        _sprite.Scale = new Vector2(VisualScale * (1f - WalkSquash * 0.6f * walkPhase), VisualScale * (1f + WalkSquash * walkPhase));
        if (TouchesPlayer(toward, distance)) Game.Mohsen.Hurt();
    }

    private bool TouchesPlayer(Vector2 toward, float distance)
    {
        if (HurtboxHalfExtents.X <= 0f || HurtboxHalfExtents.Y <= 0f)
            return distance < ContactRadius + 18f;
        float x = toward.X / (HurtboxHalfExtents.X + 18f);
        float y = toward.Y / (HurtboxHalfExtents.Y + 18f);
        return x * x + y * y < 1f;
    }

    protected virtual void UpdateBehavior(float dt, Vector2 toward, float distance)
    {
        Vector2 move = Vector2.Zero;
        if (!Ranged || distance > FireRange) move = toward.Normalized();
        else if (distance < RetreatDistance) move = -toward.Normalized();
        MoveWithinArena(move * MovementSpeed * dt);
        if (Ranged)
        {
            _attackTimer -= dt;
            if (_attackTimer <= 0f && distance <= FireRange && distance > 1f)
            {
                Vector2 direction = toward.Normalized();
                Vector2 mouth = Position + MouthOffset + direction * ProjectileMuzzleDistance;
                Game.SpawnEnemyProjectile(mouth, direction, ProjectileSpeed);
                _attackTimer = Mathf.Max(0.2f, AttackInterval);
            }
        }
    }

    protected void MoveWithinArena(Vector2 displacement, float visualMargin = 48f)
    {
        Position += displacement;
        float boundary = Game.WallThickness + visualMargin;
        Position = new Vector2(Mathf.Clamp(Position.X, boundary, Game.ArenaSize.X - boundary), Mathf.Clamp(Position.Y, boundary, Game.ArenaSize.Y - boundary));
    }

    public void TakeHit(float damage, Vector2 direction, float knockback, bool criticalHit = false)
    {
        if (Health <= 0f) return;
        Game.PlaySound(GameSound.EnemyHit);
        if (this is BossEnemy) Game.RequestBossHitPause();
        float appliedDamage = Mathf.Max(1f, damage);
        Health -= appliedDamage;
        Game.SpawnDamageNumber(appliedDamage, Position + new Vector2(0f, -32f), criticalHit);
        _flashTime = HitFlashSeconds;
        _flashMaterial?.SetShaderParameter("flash_amount", 1f);
        Position += direction.Normalized() * Mathf.Max(0f, knockback) * (1f - _resistance);
        float boundary = Game.WallThickness + HitWallMargin;
        Position = new Vector2(Mathf.Clamp(Position.X, boundary, Game.ArenaSize.X - boundary), Mathf.Clamp(Position.Y, boundary, Game.ArenaSize.Y - boundary));
        if (Health <= 0f)
        {
            _defeated = true;
            OnDefeated();
            Game.EnemyDefeated(this);
        }
    }

    protected virtual void OnDefeated() { }
}
