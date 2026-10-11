using Godot;

public partial class ChargeEnemy : Enemy
{
    [Export(PropertyHint.Range, "40,800,5")] public float ChargeRadius { get; set; } = 525f;
    [Export(PropertyHint.Range, "50,1200,5")] public float ChargeSpeed { get; set; } = 450f;
    [Export(PropertyHint.Range, "50,1600,5")] public float MaximumChargeSpeed { get; set; } = 900f;
    [Export(PropertyHint.Range, "0,5000,25")] public float ChargeAcceleration { get; set; } = 1800f;
    [Export(PropertyHint.Range, "0.1,3,0.05")] public float WarningSeconds { get; set; } = 0.9f;
    [Export(PropertyHint.Range, "1,8,1")] public int WarningFlashes { get; set; } = 3;
    [Export(PropertyHint.Range, "1,40,1")] public float ArrivalTolerance { get; set; } = 6f;
    [Export(PropertyHint.Range, "0,10,0.1")] public float StopDuration { get; set; } = 3f;

    private enum ChargeState { Approach, Warn, Charge, Recover }
    private ChargeState _state;
    private Vector2 _targetDestination;
    private Vector2 _chargeDirection;
    private float _remainingDistance;
    private float _recoveryLeft;
    private float _warningElapsed;
    private float _currentChargeSpeed;
    private Sprite2D _visual;

    protected override float HitWallMargin => 34f;

    public override void _Ready()
    {
        base._Ready();
        _visual = GetNode<Sprite2D>("Visual");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Health <= 0f || Game == null || !Game.WaveActive || Game.IsImpactPaused) return;
        if (_state == ChargeState.Warn)
            _visual.FlipH = _chargeDirection.X < 0f;
        else if (_state == ChargeState.Charge)
        {
            _visual.FlipH = _chargeDirection.X < 0f;
            _visual.Scale *= new Vector2(1.12f, 0.9f);
        }
        else if (_state == ChargeState.Recover) _visual.Scale *= new Vector2(0.94f, 1.06f);
    }

    protected override void UpdateBehavior(float dt, Vector2 toward, float distance)
    {
        switch (_state)
        {
            case ChargeState.Approach:
                if (distance <= ChargeRadius && distance > 0.01f)
                {
                    // Capture Mohsen's position and direction exactly once per charge.
                    _targetDestination = Game.Mohsen.Position;
                    _chargeDirection = toward / distance;
                    _remainingDistance = distance;
                    _warningElapsed = 0f;
                    _state = ChargeState.Warn;
                    SetWarningFlash(0f);
                }
                else MoveWithinArena(toward.Normalized() * MovementSpeed * dt, 34f);
                break;

            case ChargeState.Warn:
                _warningElapsed += dt;
                float warningProgress = Mathf.Clamp(_warningElapsed / Mathf.Max(0.01f, WarningSeconds), 0f, 1f);
                float flashCycle = warningProgress * Mathf.Max(1, WarningFlashes);
                SetWarningFlash(warningProgress >= 1f ? 0f : Mathf.Sin(Mathf.Pi * (flashCycle - Mathf.Floor(flashCycle))));
                if (warningProgress >= 1f)
                {
                    _currentChargeSpeed = Mathf.Max(0f, ChargeSpeed) * ConfiguredSpeedMultiplier;
                    _state = ChargeState.Charge;
                }
                break;

            case ChargeState.Charge:
                float distanceAlongCharge = (_targetDestination - Position).Dot(_chargeDirection);
                if (distanceAlongCharge <= ArrivalTolerance || _remainingDistance <= ArrivalTolerance)
                {
                    FinishCharge();
                    break;
                }
                _currentChargeSpeed = Mathf.Min(Mathf.Max(ChargeSpeed, MaximumChargeSpeed) * ConfiguredSpeedMultiplier,
                    _currentChargeSpeed + Mathf.Max(0f, ChargeAcceleration) * ConfiguredSpeedMultiplier * dt);
                float step = Mathf.Min(Mathf.Min(_currentChargeSpeed * dt, distanceAlongCharge), _remainingDistance);
                Vector2 before = Position;
                MoveWithinArena(_chargeDirection * step, 34f);
                _remainingDistance = Mathf.Max(0f, _remainingDistance - Mathf.Max(0f, (Position - before).Dot(_chargeDirection)));
                if (_remainingDistance <= ArrivalTolerance || Position.DistanceTo(_targetDestination) <= ArrivalTolerance || Position == before)
                    FinishCharge();
                break;

            case ChargeState.Recover:
                _recoveryLeft -= dt;
                if (_recoveryLeft <= 0f) _state = ChargeState.Approach;
                break;
        }
    }

    private void FinishCharge()
    {
        // End at the captured destination, including after a small knockback deflection.
        Position = _targetDestination;
        _state = ChargeState.Recover;
        _recoveryLeft = Mathf.Max(0f, StopDuration);
        _remainingDistance = 0f;
        SetWarningFlash(0f);
    }

    protected override void OnDefeated() => SetWarningFlash(0f);
}
