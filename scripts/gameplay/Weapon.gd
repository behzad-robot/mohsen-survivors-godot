class_name Weapon
extends Node2D

@export var Kind: WeaponData.Kind = WeaponData.Kind.Knife
@export_range(1, 4, 1) var Tier: int = 1
@export_range(-180, 180, 1) var IdleRotationDegrees: float = 0.0
@export_range(180, 2880, 90) var AimTurnDegreesPerSecond: float = 1440
@export_range(0, 15, 1) var FireAngleToleranceDegrees: float = 6
@export_range(0, 8, 0.25) var ShotShakePixels: float = 2
@export_range(0, 16, 0.5) var RecoilPixels: float = 0.0
@export_range(0.04, 0.4, 0.01) var RecoilReturnSeconds: float = 0.12
@export_range(0.02, 0.15, 0.01) var MuzzleFlashSeconds: float = 0.06

var _cooldown = 0.0
var _recoilTime = 0.0
var _flashTime = 0.0
var _rotationInitialized = false
var _visual
var _muzzleFlash
var _muzzleOffset = Vector2.ZERO
var _game
var _player

func _ready():
	_visual = get_node("Visual")
	_muzzleFlash = get_node_or_null("MuzzleFlash")
	if _muzzleFlash != null:
		_muzzleOffset = _muzzleFlash.position
		_muzzleFlash.visible = false

func Equip(game, player, slot):
	_game = game
	_player = player
	SetSlot(slot)

func SetSlot(slot):
	position = [Vector2(36, 0), Vector2(-36, 0), Vector2(0, -42), Vector2(0, 42)][clampi(slot, 0, 3)]
	if not _rotationInitialized:
		rotation = deg_to_rad(IdleRotationDegrees) + (PI if slot == 1 else 0.0)
		_rotationInitialized = true

func ResetTransientEffects():
	_recoilTime = 0.0
	_flashTime = 0.0
	_visual.position = Vector2.ZERO
	if _muzzleFlash != null:
		_muzzleFlash.visible = false
		_muzzleFlash.position = _muzzleOffset

func ShotOrigin(direction):
	return global_position + direction * 5.0 if _muzzleFlash == null else _muzzleFlash.global_position

func Tick(dt):
	if _game == null or _player == null or not _game.WaveActive: return
	_cooldown = maxf(0.0, _cooldown - dt)
	_recoilTime = maxf(0.0, _recoilTime - dt)
	_flashTime = maxf(0.0, _flashTime - dt)
	var recoilFraction = 0.0 if RecoilReturnSeconds <= 0.0 else _recoilTime / RecoilReturnSeconds
	_visual.position = Vector2(-RecoilPixels * recoilFraction * recoilFraction, 0.0)
	if _muzzleFlash != null:
		_muzzleFlash.position = _muzzleOffset + _visual.position
		_muzzleFlash.visible = _flashTime > 0.0
		var alpha = 0.0 if MuzzleFlashSeconds <= 0.0 else minf(1.0, 2.0 * _flashTime / MuzzleFlashSeconds)
		_muzzleFlash.modulate = Color(1, 1, 1, alpha)
	var stats = WeaponData.Get(Kind, Tier)
	var target = null
	var nearestDistanceSquared = stats.Range * stats.Range
	for enemy in _game.Enemies:
		if not is_instance_valid(enemy): continue
		var distanceSquared = enemy.global_position.distance_squared_to(_player.global_position)
		if distanceSquared > nearestDistanceSquared: continue
		nearestDistanceSquared = distanceSquared
		target = enemy
	if target == null: return
	var direction = (target.global_position - global_position).normalized()
	var desiredRotation = direction.angle() - _player.global_rotation
	var angleToTarget = wrapf(desiredRotation - rotation, -PI, PI)
	var maxTurn = deg_to_rad(AimTurnDegreesPerSecond) * dt
	rotation = wrapf(rotation + clampf(angleToTarget, -maxTurn, maxTurn), -PI, PI)
	if _cooldown > 0.0 or (_player.CannotShootWhileMoving and _player.IsMoving) or absf(wrapf(desiredRotation - rotation, -PI, PI)) > deg_to_rad(FireAngleToleranceDegrees): return
	_cooldown = stats.Interval
	_game.FireWeapon(self, direction, stats)
	if RecoilPixels > 0.0:
		_recoilTime = RecoilReturnSeconds
		_visual.position = Vector2(-RecoilPixels, 0.0)
	if _muzzleFlash != null:
		_flashTime = MuzzleFlashSeconds
		_muzzleFlash.position = _muzzleOffset + _visual.position
		_muzzleFlash.modulate = Color.WHITE
		_muzzleFlash.visible = true
