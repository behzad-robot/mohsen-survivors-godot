class_name BossEnemy
extends Enemy

@export_range(0.5, 10, 0.1) var MoveSeconds: float = 2.8
@export_range(0.3, 5, 0.1) var StopSeconds: float = 1.2
@export_range(0.1, 2, 0.05) var VolleyInterval: float = 0.45
@export_range(0.1, 1.5, 0.05) var WarningSeconds: float = 0.42
@export_range(1, 20, 0.5) var WarningFlashHz: float = 7
@export_range(1, 5, 1) var VolleysPerStop: int = 2
@export_range(8, 32, 2) var BulletSlots: int = 16
@export_range(1, 7, 2) var SafeGapSlots: int = 3
@export_range(50, 500, 10) var BulletSpeed: float = 200
@export_range(20, 150, 5) var ProjectileSpawnDistance: float = 100
@export_range(100, 500, 10) var PreferredDistance: float = 270
@export_range(0, 1, 0.05) var OrbitWeight: float = 0.45
@export_range(50, 200, 5) var WallMargin: float = 135

var _phaseTime = 0.0
var _stopping = false
var _volleysFired = 0
var _aimAngle = 0.0
var _orbitSign = 1.0
var _waveNameLabel
var _headHpBar

func HitWallMargin(): return WallMargin

func _ready():
	super._ready()
	_orbitSign = 1.0 if (get_instance_id() & 1) == 0 else -1.0
	_waveNameLabel = get_node("WaveName")
	_headHpBar = get_node("HeadHpBar")

func SetWaveName(waveName, waveNumber):
	var title = waveName.strip_edges()
	_waveNameLabel.text = "Wave %d" % waveNumber if title.is_empty() else title

func _process(dt):
	super._process(dt)
	_headHpBar.max_value = maxf(1.0, MaxHealth)
	_headHpBar.value = clampf(Health, 0.0, MaxHealth)
	_headHpBar.visible = Health > 0.0
	_waveNameLabel.visible = Health > 0.0

func UpdateBehavior(dt, toward, distance):
	_phaseTime += dt
	if _stopping:
		if _phaseTime < WarningSeconds:
			SetWarningFlash(1.0 if sin(_phaseTime * TAU * WarningFlashHz) >= 0.0 else 0.15)
			return
		SetWarningFlash(0.0)
		while _volleysFired < VolleysPerStop and _phaseTime >= WarningSeconds + _volleysFired * VolleyInterval:
			FireRing()
			_volleysFired += 1
		var minimumStop = WarningSeconds + maxi(0, VolleysPerStop - 1) * VolleyInterval + 0.05
		if _phaseTime >= maxf(StopSeconds, minimumStop):
			_stopping = false
			_phaseTime = 0.0
		return
	var forward = toward / distance if distance > 0.001 else Vector2.RIGHT
	var orbit = Vector2(-forward.y * _orbitSign, forward.x * _orbitSign)
	var move = orbit
	if distance > PreferredDistance + 30.0: move = (forward + orbit * OrbitWeight).normalized()
	elif distance < PreferredDistance - 30.0: move = (-forward + orbit * OrbitWeight).normalized()
	var before = position
	var displacement = move * MovementSpeed * dt
	MoveWithinArena(displacement, WallMargin)
	if displacement.length_squared() > 0.0 and before.distance_squared_to(position) < displacement.length_squared() * 0.25:
		_orbitSign = -_orbitSign
	if _phaseTime >= MoveSeconds:
		_stopping = true
		_phaseTime = 0.0
		_volleysFired = 0
		_aimAngle = toward.angle()
		SetWarningFlash(1.0)
		Game.PlaySound(GameAudio.Sound.BossWarning)

func FireRing():
	var slots = maxi(8, BulletSlots)
	var halfGap = clampi(int(SafeGapSlots / 2.0), 0, int((slots - 2) / 4.0))
	var opposite = int(slots / 2.0)
	for i in range(slots):
		var fromAim = mini(i, slots - i)
		var fromOpposite = absi(i - opposite)
		if fromAim <= halfGap or fromOpposite <= halfGap: continue
		var direction = Vector2.RIGHT.rotated(_aimAngle + TAU * i / slots)
		Game.SpawnBossProjectile(position + direction * ProjectileSpawnDistance, direction, BulletSpeed, ProjectileDamage)
