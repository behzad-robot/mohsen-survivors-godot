using Godot;
using System.Linq;

public partial class Weapon : Node2D
{
    [Export] public WeaponKind Kind { get; set; }
    [Export(PropertyHint.Range, "1,4,1")] public int Tier { get; set; } = 1;
    [Export(PropertyHint.Range, "-180,180,1")] public float IdleRotationDegrees { get; set; }
    [Export(PropertyHint.Range, "0.01,0.3,0.01")] public float AttackPoseSeconds { get; set; } = .08f;
    [Export(PropertyHint.Range, "0,8,0.25")] public float ShotShakePixels { get; set; } = 2f;
    [Export(PropertyHint.Range, "0,16,0.5")] public float RecoilPixels { get; set; }
    [Export(PropertyHint.Range, "0.04,0.4,0.01")] public float RecoilReturnSeconds { get; set; } = 0.12f;
    [Export(PropertyHint.Range, "0.02,0.15,0.01")] public float MuzzleFlashSeconds { get; set; } = 0.06f;

    private float _cooldown;
    private float _poseTime;
    private float _recoilTime;
    private float _flashTime;
    private float _idleRotation;
    private Sprite2D _visual;
    private Node2D _muzzleFlash;
    private Vector2 _muzzleOffset;
    private GameController _game;
    private Player _owner;

    public override void _Ready()
    {
        _idleRotation = Mathf.DegToRad(IdleRotationDegrees);
        _visual = GetNode<Sprite2D>("Visual");
        _muzzleFlash = GetNodeOrNull<Node2D>("MuzzleFlash");
        if (_muzzleFlash != null)
        {
            _muzzleOffset = _muzzleFlash.Position;
            _muzzleFlash.Visible = false;
        }
    }

    public void Equip(GameController game, Player owner, int slot)
    {
        _game = game;
        _owner = owner;
        SetSlot(slot);
    }

    public void SetSlot(int slot)
    {
        Position = slot switch
        {
            0 => new Vector2(36, 0),
            1 => new Vector2(-36, 0),
            2 => new Vector2(0, -42),
            _ => new Vector2(0, 42)
        };
        _idleRotation = Mathf.DegToRad(IdleRotationDegrees) + (slot == 1 ? Mathf.Pi : 0f);
        if (_poseTime <= 0f) Rotation = _idleRotation;
    }

    public void ReturnToIdle()
    {
        _poseTime = 0f;
        _recoilTime = 0f;
        _flashTime = 0f;
        _visual.Position = Vector2.Zero;
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Visible = false;
            _muzzleFlash.Position = _muzzleOffset;
        }
        Rotation = _idleRotation;
    }

    public Vector2 ShotOrigin(Vector2 direction) => _muzzleFlash == null
        ? GlobalPosition + direction * 5f
        : _muzzleFlash.GlobalPosition;

    public void Tick(float delta)
    {
        if (_game == null || _owner == null || !_game.WaveActive) return;
        _cooldown = Mathf.Max(0f, _cooldown - delta);
        _poseTime = Mathf.Max(0f, _poseTime - delta);
        _recoilTime = Mathf.Max(0f, _recoilTime - delta);
        _flashTime = Mathf.Max(0f, _flashTime - delta);
        float recoilFraction = RecoilReturnSeconds <= 0f ? 0f : _recoilTime / RecoilReturnSeconds;
        _visual.Position = new Vector2(-RecoilPixels * recoilFraction * recoilFraction, 0f);
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Position = _muzzleOffset + _visual.Position;
            _muzzleFlash.Visible = _flashTime > 0f;
            float alpha = MuzzleFlashSeconds <= 0f ? 0f : Mathf.Min(1f, 2f * _flashTime / MuzzleFlashSeconds);
            _muzzleFlash.Modulate = new Color(1f, 1f, 1f, alpha);
        }
        if (_poseTime <= 0f) Rotation = _idleRotation;
        if (_cooldown > 0f) return;

        WeaponStats stats = WeaponData.Get(Kind, Tier);
        Enemy target = _game.Enemies
            .Where(e => GodotObject.IsInstanceValid(e) && e.GlobalPosition.DistanceSquaredTo(_owner.GlobalPosition) <= stats.Range * stats.Range)
            .OrderBy(e => e.GlobalPosition.DistanceSquaredTo(_owner.GlobalPosition))
            .FirstOrDefault();
        if (target == null) return;

        Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
        Rotation = direction.Angle() - _owner.GlobalRotation;
        _poseTime = AttackPoseSeconds;
        _cooldown = stats.Interval;
        _game.FireWeapon(this, direction, stats);
        if (RecoilPixels > 0f)
        {
            _recoilTime = RecoilReturnSeconds;
            _visual.Position = new Vector2(-RecoilPixels, 0f);
        }
        if (_muzzleFlash != null)
        {
            _flashTime = MuzzleFlashSeconds;
            _muzzleFlash.Position = _muzzleOffset + _visual.Position;
            _muzzleFlash.Modulate = Colors.White;
            _muzzleFlash.Visible = true;
        }
    }
}
