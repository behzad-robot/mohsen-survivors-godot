using Godot;

public partial class Player : Node2D
{
    private const float RegenTickSeconds = 1f;
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void HealedEventHandler(float amount);
    [Signal] public delegate void DamagedEventHandler(float amount);
    [Export(PropertyHint.Range, "0.1,5,0.1")] public float InvulnerabilitySeconds { get; set; } = 1f;
    [Export(PropertyHint.Range, "20,200,1")] public float CoinPickupRadius { get; set; } = 78f;
    [Export] public float MaxHealth { get; set; } = 10f;
    [Export] public float MovementSpeed { get; set; } = 240f;
    public float EffectiveMovementSpeed => Mathf.Max(1f, MovementSpeed) + _coffeeSpeedBonus;
    [Export(PropertyHint.Range, "0.05,2,0.05")] public float RegenTextBatchSeconds { get; set; } = 0.5f;
    [Export] public float Knockback { get; set; }
    [Export] public float BonusDamagePercent { get; set; }
    [Export] public float RangedDamagePercent { get; set; }
    [Export] public float MeleeDamagePercent { get; set; }
    [Export] public float CriticalChancePercent { get; set; }
    [Export] public float ProjectileSizePercent { get; set; }
    public float ProjectileSizeMultiplier => Mathf.Max(0.1f, 1f + ProjectileSizePercent / 100f);
    public bool CannotShootWhileMoving { get; private set; }
    public bool IsMoving => ReadMovementInput() != Vector2.Zero;
    [Export] public float HealthRegenPerSecond { get; set; }
    [Export] public int Spread { get; set; }
    [Export] public int BulletPierce { get; set; }
    [Export(PropertyHint.Range, "0.02,0.3,0.01")] public float ShotShakeDuration { get; set; } = 0.09f;
    [Export(PropertyHint.Range, "0,12,0.25")] public float MaximumShotShakePixels { get; set; } = 5f;

    public float Health { get; private set; }
    public bool Active { get; set; }
    public GameController Game { get; set; }
    public Node2D WeaponMount { get; private set; }
    private Sprite2D _sprite;
    private Polygon2D _groundShadow;
    private CoffeeSpeedParticles _coffeeParticles;
    private ProgressBar _headHpBar;
    private Camera2D _camera;
    private readonly RandomNumberGenerator _shakeRandom = new();
    private float _shakeTime;
    private float _shakePixels;
    private float _invulnerable;
    private bool _dying;
    private float _deathTime;
    private float _coffeeSpeedBonus;
    private float _coffeeSecondsLeft;
    private float _regenTextAmount;
    private float _regenTextAge;
    private float _regenTickElapsed;
    private float _regenRemainder;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Visual");
        _groundShadow = GetNode<Polygon2D>("GroundShadow");
        _coffeeParticles = GetNode<CoffeeSpeedParticles>("CoffeeParticles");
        _headHpBar = GetNode<ProgressBar>("HeadHpBar");
        _camera = GetNode<Camera2D>("Camera");
        WeaponMount = GetNode<Node2D>("WeaponMount");
        _shakeRandom.Randomize();
        Health = MaxHealth;
        UpdateHeadHpBar();
    }

    public override void _Process(double delta)
    {
        if (Game?.IsImpactPaused == true) return;
        float dt = (float)delta;
        UpdateShotShake(dt);
        if (_dying)
        {
            _headHpBar.Visible = false;
            _deathTime += dt;
            Scale = Vector2.One * Mathf.Max(0f, 1f - _deathTime / 0.65f);
            return;
        }
        if (!Active) return;
        Vector2 direction = ReadMovementInput();
        if (direction.X < 0f) _sprite.FlipH = true;
        else if (direction.X > 0f) _sprite.FlipH = false;
        Position += direction.Normalized() * EffectiveMovementSpeed * dt;
        float boundary = Game.WallThickness + 34f;
        Position = new Vector2(Mathf.Clamp(Position.X, boundary, Game.ArenaSize.X - boundary), Mathf.Clamp(Position.Y, boundary, Game.ArenaSize.Y - boundary));
        _sprite.Scale = new Vector2(1f, 1f + (direction == Vector2.Zero ? 0f : 0.035f * Mathf.Sin((float)Time.GetTicksMsec() / 70f)));
        float shadowPulse = direction == Vector2.Zero ? 0f : Mathf.Sin((float)Time.GetTicksMsec() / 85f);
        Vector2 shadowTargetScale = new(1f + 0.06f * shadowPulse, 1f - 0.08f * shadowPulse);
        _groundShadow.Scale = _groundShadow.Scale.Lerp(shadowTargetScale, Mathf.Clamp(dt * 12f, 0f, 1f));
        _regenTickElapsed += dt;
        while (_regenTickElapsed >= RegenTickSeconds)
        {
            _regenTickElapsed -= RegenTickSeconds;
            if (Health >= MaxHealth) continue;
            _regenRemainder += Mathf.Max(0f, HealthRegenPerSecond);
            // Avoid a binary float landing just below a whole HP (for example, 0.7 × 10).
            int wholeHealing = (int)(_regenRemainder + 0.000001f);
            if (wholeHealing <= 0) continue;
            _regenRemainder = Mathf.Max(0f, _regenRemainder - wholeHealing);
            float regenerated = RestoreHealth(wholeHealing, false);
            if (regenerated > 0f) _regenTextAmount += regenerated;
        }
        _regenTextAge += dt;
        if (_regenTextAmount > 0f && (_regenTextAge >= RegenTextBatchSeconds || Health >= MaxHealth)) FlushRegenText();
        if (_coffeeSecondsLeft > 0f)
        {
            _coffeeSecondsLeft = Mathf.Max(0f, _coffeeSecondsLeft - dt);
            if (_coffeeSecondsLeft <= 0f)
            {
                _coffeeSpeedBonus = 0f;
                _coffeeParticles.SetEmitting(false);
            }
        }
        UpdateHeadHpBar();
        _invulnerable = Mathf.Max(0f, _invulnerable - dt);
        _sprite.Modulate = _invulnerable > 0f && ((int)(Time.GetTicksMsec() / 90) % 2 == 0) ? new Color(1, 1, 1, 0.35f) : Colors.White;
    }

    public void Refill()
    {
        FlushRegenText();
        RestoreHealth(MaxHealth - Health);
        _invulnerable = 0f;
    }

    public float Heal(float amount) => RestoreHealth(amount);

    private float RestoreHealth(float amount, bool showText = true)
    {
        float before = Health;
        Health = Mathf.Min(MaxHealth, Health + Mathf.Max(0f, amount));
        float restored = Health - before;
        if (restored > 0f && showText) EmitSignal(SignalName.Healed, restored);
        UpdateHeadHpBar();
        return restored;
    }

    private void FlushRegenText()
    {
        if (_regenTextAmount > 0f) EmitSignal(SignalName.Healed, _regenTextAmount);
        _regenTextAmount = 0f;
        _regenTextAge = 0f;
    }

    public void ApplyCoffeeBoost(float percent, float seconds)
    {
        // Repeated pickups refresh a single bonus based on current permanent speed.
        _coffeeSpeedBonus = Mathf.Max(1f, MovementSpeed) * Mathf.Max(0f, percent) / 100f;
        _coffeeSecondsLeft = Mathf.Max(0f, seconds);
        _coffeeParticles.SetEmitting(_coffeeSpeedBonus > 0f && _coffeeSecondsLeft > 0f);
    }

    public void ClearTemporaryEffects()
    {
        FlushRegenText();
        _regenTickElapsed = 0f;
        _regenRemainder = 0f;
        _coffeeSpeedBonus = 0f;
        _coffeeSecondsLeft = 0f;
        _coffeeParticles.SetEmitting(false);
    }

    private void UpdateHeadHpBar()
    {
        _headHpBar.MaxValue = Mathf.Max(1f, MaxHealth);
        _headHpBar.Value = Health;
        _headHpBar.Visible = Health > 0f && Health < MaxHealth - 0.001f;
    }

    public void ShakeOnShot(float pixels)
    {
        if (pixels <= 0f) return;
        _shakePixels = Mathf.Min(MaximumShotShakePixels, Mathf.Max(_shakePixels, pixels));
        _shakeTime = ShotShakeDuration;
    }

    private void UpdateShotShake(float delta)
    {
        if (_shakeTime <= 0f)
        {
            _camera.Offset = Vector2.Zero;
            _shakePixels = 0f;
            return;
        }
        _shakeTime = Mathf.Max(0f, _shakeTime - delta);
        float fade = ShotShakeDuration <= 0f ? 0f : _shakeTime / ShotShakeDuration;
        _camera.Offset = new Vector2(_shakeRandom.RandfRange(-1f, 1f), _shakeRandom.RandfRange(-1f, 1f)) * _shakePixels * fade;
    }

    private static Vector2 ReadMovementInput()
    {
        Vector2 direction = Vector2.Zero;
        if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left)) direction.X -= 1;
        if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right)) direction.X += 1;
        if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up)) direction.Y -= 1;
        if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down)) direction.Y += 1;
        return direction;
    }

    public void ApplyModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        switch (modifier.Stat)
        {
            case PlayerStat.MaxHealth:
                MaxHealth = Mathf.Max(1f, MaxHealth + modifier.Amount);
                if (modifier.Amount > 0f) RestoreHealth(modifier.Amount);
                else { Health = Mathf.Clamp(Health + modifier.Amount, 1f, MaxHealth); UpdateHeadHpBar(); }
                break;
            case PlayerStat.MovementSpeed: MovementSpeed += modifier.Amount; break;
            case PlayerStat.Knockback: Knockback += modifier.Amount; break;
            case PlayerStat.BonusDamagePercent: BonusDamagePercent += modifier.Amount; break;
            case PlayerStat.RangedDamagePercent: RangedDamagePercent += modifier.Amount; break;
            case PlayerStat.MeleeDamagePercent: MeleeDamagePercent += modifier.Amount; break;
            case PlayerStat.CriticalChancePercent: CriticalChancePercent += modifier.Amount; break;
            case PlayerStat.ProjectileSizePercent: ProjectileSizePercent += modifier.Amount; break;
            case PlayerStat.BlockFiringWhileMoving:
                if (modifier.Amount > 0f) CannotShootWhileMoving = true;
                break;
            case PlayerStat.HealthRegenPerSecond: HealthRegenPerSecond += modifier.Amount; break;
            case PlayerStat.Spread: Spread = Mathf.Max(0, Spread + Mathf.RoundToInt(modifier.Amount)); break;
            case PlayerStat.BulletPierce: BulletPierce = Mathf.Max(0, BulletPierce + Mathf.RoundToInt(modifier.Amount)); break;
        }
    }

    public void Hurt(float damage = 1f)
    {
        if (!Active || _invulnerable > 0f || _dying) return;
        Game?.PlaySound(GameSound.MohsenHit);
        float before = Health;
        Health = Mathf.Max(0f, Health - damage);
        UpdateHeadHpBar();
        if (Health < before) EmitSignal(SignalName.Damaged, before - Health);
        _invulnerable = InvulnerabilitySeconds;
        if (Health <= 0f)
        {
            Active = false;
            _dying = true;
            EmitSignal(SignalName.Died);
        }
    }
}
