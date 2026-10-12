using Godot;

public partial class BossEnemy : Enemy
{
    [Export(PropertyHint.Range, "0.5,10,0.1")] public float MoveSeconds { get; set; } = 2.8f;
    [Export(PropertyHint.Range, "0.3,5,0.1")] public float StopSeconds { get; set; } = 1.2f;
    [Export(PropertyHint.Range, "0.1,2,0.05")] public float VolleyInterval { get; set; } = 0.45f;
    [Export(PropertyHint.Range, "0.1,1.5,0.05")] public float WarningSeconds { get; set; } = 0.42f;
    [Export(PropertyHint.Range, "1,20,0.5")] public float WarningFlashHz { get; set; } = 7f;
    [Export(PropertyHint.Range, "1,5,1")] public int VolleysPerStop { get; set; } = 2;
    [Export(PropertyHint.Range, "8,32,2")] public int BulletSlots { get; set; } = 16;
    [Export(PropertyHint.Range, "1,7,2")] public int SafeGapSlots { get; set; } = 3;
    [Export(PropertyHint.Range, "50,500,10")] public float BulletSpeed { get; set; } = 200f;
    [Export(PropertyHint.Range, "20,150,5")] public float ProjectileSpawnDistance { get; set; } = 100f;
    [Export(PropertyHint.Range, "100,500,10")] public float PreferredDistance { get; set; } = 270f;
    [Export(PropertyHint.Range, "0,1,0.05")] public float OrbitWeight { get; set; } = 0.45f;
    [Export(PropertyHint.Range, "50,200,5")] public float WallMargin { get; set; } = 135f;

    private float _phaseTime;
    private bool _stopping;
    private int _volleysFired;
    private float _aimAngle;
    private float _orbitSign = 1f;
    private Label _waveNameLabel;
    private ProgressBar _headHpBar;
    protected override float HitWallMargin => WallMargin;

    public override void _Ready()
    {
        base._Ready();
        _orbitSign = (GetInstanceId() & 1UL) == 0UL ? 1f : -1f;
        _waveNameLabel = GetNode<Label>("WaveName");
        _headHpBar = GetNode<ProgressBar>("HeadHpBar");
    }

    public void SetWaveName(string waveName, int waveNumber)
    {
        string name = waveName?.Trim() ?? "";
        _waveNameLabel.Text = string.IsNullOrEmpty(name) ? $"Wave {waveNumber}" : name;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _headHpBar.MaxValue = Mathf.Max(1f, MaxHealth);
        _headHpBar.Value = Mathf.Clamp(Health, 0f, MaxHealth);
        bool alive = Health > 0f;
        _headHpBar.Visible = alive;
        _waveNameLabel.Visible = alive;
    }

    protected override void UpdateBehavior(float dt, Vector2 toward, float distance)
    {
        _phaseTime += dt;
        if (_stopping)
        {
            if (_phaseTime < WarningSeconds)
            {
                SetWarningFlash(Mathf.Sin(_phaseTime * Mathf.Tau * WarningFlashHz) >= 0f ? 1f : 0.15f);
                return;
            }
            SetWarningFlash(0f);
            while (_volleysFired < VolleysPerStop && _phaseTime >= WarningSeconds + _volleysFired * VolleyInterval)
            {
                FireRing();
                _volleysFired++;
            }
            float minimumStop = WarningSeconds + Mathf.Max(0, VolleysPerStop - 1) * VolleyInterval + 0.05f;
            if (_phaseTime >= Mathf.Max(StopSeconds, minimumStop))
            {
                _stopping = false;
                _phaseTime = 0f;
            }
            return;
        }

        Vector2 forward = distance > 0.001f ? toward / distance : Vector2.Right;
        Vector2 orbit = new(-forward.Y * _orbitSign, forward.X * _orbitSign);
        Vector2 move = distance > PreferredDistance + 30f
            ? (forward + orbit * OrbitWeight).Normalized()
            : distance < PreferredDistance - 30f
                ? (-forward + orbit * OrbitWeight).Normalized()
                : orbit;
        Vector2 before = Position;
        Vector2 displacement = move * MovementSpeed * dt;
        MoveWithinArena(displacement, WallMargin);
        if (displacement.LengthSquared() > 0f && before.DistanceSquaredTo(Position) < displacement.LengthSquared() * 0.25f)
            _orbitSign = -_orbitSign;

        if (_phaseTime >= MoveSeconds)
        {
            _stopping = true;
            _phaseTime = 0f;
            _volleysFired = 0;
            _aimAngle = toward.Angle();
            SetWarningFlash(1f);
            Game.PlaySound(GameSound.BossWarning);
        }
    }

    private void FireRing()
    {
        int slots = Mathf.Max(8, BulletSlots);
        int halfGap = Mathf.Clamp(SafeGapSlots / 2, 0, (slots - 2) / 4);
        int opposite = slots / 2;
        for (int i = 0; i < slots; i++)
        {
            int fromAim = Mathf.Min(i, slots - i);
            int fromOpposite = Mathf.Abs(i - opposite);
            if (fromAim <= halfGap || fromOpposite <= halfGap) continue;
            float angle = _aimAngle + Mathf.Pi * 2f * i / slots;
            Vector2 direction = Vector2.Right.Rotated(angle);
            Game.SpawnBossProjectile(Position + direction * ProjectileSpawnDistance, direction, BulletSpeed, ProjectileDamage);
        }
    }
}
