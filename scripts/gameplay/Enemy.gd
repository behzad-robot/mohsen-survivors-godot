class_name Enemy
extends Node2D

@export var Ranged: bool = false
@export var BaseHealth: float = 18
@export_range(0, 200, 0.25) var HealthGainPerWave: float = 0.0
@export_range(0, 100, 0.05) var BaseDamage: float = 1
@export_range(0, 100, 0.05) var DamagePerWave: float = 0.0
@export_range(0, 100, 0.05) var BaseProjectileDamage: float = 1
@export_range(0, 100, 0.05) var ProjectileDamagePerWave: float = 0.0
@export var BaseSpeed: float = 82
@export_range(0, 1, 0.05) var BaseKnockbackResistance: float = 0.0
@export var FireRange: float = 260
@export var RetreatDistance: float = 140
@export var AttackInterval: float = 2.4
@export_range(0, 0.8, 0.02) var RangedTellSeconds: float = 0.24
@export var ProjectileSpeed: float = 230
@export var ContactRadius: float = 27
@export var HurtboxHalfExtents: Vector2 = Vector2.ZERO
@export_range(0.02, 0.5, 0.01) var HitFlashSeconds: float = 0.07
@export_range(0.25, 5.0, 0.01) var VisualScale: float = 1.12
@export_range(0, 0.25, 0.01) var WalkSquash: float = 0.10
@export var MouthOffset: Vector2 = Vector2(0, 2)
@export_range(0, 80, 1) var ProjectileMuzzleDistance: float = 12

var Game
var Health = 0.0
var MaxHealth = 0.0
var ContactDamage = 1.0
var ProjectileDamage = 1.0
var BurstDamageMultiplier = 1.0
var ConfiguredSpeedMultiplier = 1.0
var MovementSpeed: float:
	get: return _speed
var _speed = 0.0
var _resistance = 0.0
var _attackTimer = 0.0
var _rangedTellRemaining = 0.0
var _rangedTellDirection = Vector2.ZERO
var _flashTime = 0.0
var _defeated = false
var _sprite
var _flashMaterial

func HitWallMargin(): return 48.0

func SetWarningFlash(amount):
	if _flashMaterial != null: _flashMaterial.set_shader_parameter("warning_amount", clampf(amount, 0.0, 1.0))

func _ready():
	_sprite = get_node("Visual")
	if _sprite.material is ShaderMaterial:
		_flashMaterial = _sprite.material.duplicate()
		_sprite.material = _flashMaterial
	_sprite.scale = Vector2.ONE * VisualScale

func Configure(game, waveNumber, waveHealthMultiplier, waveDamageMultiplier, healthMultiplier, speedMultiplier, resistanceAdjustment, burstDamageMultiplier = 1.0):
	Game = game
	var waveHealth = BaseHealth + HealthGainPerWave * maxi(0, waveNumber - 1)
	MaxHealth = maxf(1.0, waveHealth * maxf(0.1, waveHealthMultiplier) * maxf(0.1, healthMultiplier))
	Health = MaxHealth
	ConfiguredSpeedMultiplier = maxf(0.1, speedMultiplier)
	_speed = maxf(1.0, BaseSpeed * ConfiguredSpeedMultiplier)
	var damageScale = maxf(0.1, waveDamageMultiplier) * maxf(0.1, burstDamageMultiplier)
	var completedWaves = maxi(0, waveNumber - 1)
	ContactDamage = maxf(0.0, BaseDamage + DamagePerWave * completedWaves) * damageScale
	ProjectileDamage = maxf(0.0, BaseProjectileDamage + ProjectileDamagePerWave * completedWaves) * damageScale
	BurstDamageMultiplier = maxf(0.1, burstDamageMultiplier)
	_resistance = clampf(BaseKnockbackResistance + resistanceAdjustment, 0.0, 0.95)
	_attackTimer = AttackInterval * 0.5
	_rangedTellRemaining = 0.0

func _process(dt):
	if Game != null and Game.IsImpactPaused: return
	_flashTime = maxf(0.0, _flashTime - dt)
	if _flashMaterial != null: _flashMaterial.set_shader_parameter("flash_amount", 1.0 if _flashTime > 0.0 else 0.0)
	if _defeated:
		if _flashTime <= 0.0: queue_free()
		return
	if Game == null or not Game.WaveActive: return
	var toward = Game.Mohsen.position - position
	var distance = toward.length()
	_sprite.flip_h = toward.x < 0.0
	UpdateBehavior(dt, toward, distance)
	var walkPhase = sin(Time.get_ticks_msec() / 115.0 + float(get_instance_id() % 8))
	_sprite.scale = Vector2(VisualScale * (1.0 - WalkSquash * 0.6 * walkPhase), VisualScale * (1.0 + WalkSquash * walkPhase))
	if TouchesPlayer(toward, distance): Game.Mohsen.Hurt(ContactDamage)

func TouchesPlayer(toward, distance):
	if HurtboxHalfExtents.x <= 0.0 or HurtboxHalfExtents.y <= 0.0: return distance < ContactRadius + 18.0
	var x = toward.x / (HurtboxHalfExtents.x + 18.0)
	var y = toward.y / (HurtboxHalfExtents.y + 18.0)
	return x * x + y * y < 1.0

func UpdateBehavior(dt, toward, distance):
	var move = Vector2.ZERO
	if not Ranged or distance > FireRange: move = toward.normalized()
	elif distance < RetreatDistance: move = -toward.normalized()
	MoveWithinArena(move * MovementSpeed * dt)
	if not Ranged: return
	if _rangedTellRemaining > 0.0:
		_rangedTellRemaining = maxf(0.0, _rangedTellRemaining - dt)
		if distance > FireRange or distance <= 1.0:
			_rangedTellRemaining = 0.0
			SetWarningFlash(0.0)
			return
		if _rangedTellRemaining > 0.0:
			var progress = 1.0 - _rangedTellRemaining / maxf(0.001, RangedTellSeconds)
			SetWarningFlash(0.25 + 0.65 * progress)
			return
		SetWarningFlash(0.0)
		var mouth = position + MouthOffset + _rangedTellDirection * ProjectileMuzzleDistance
		Game.SpawnEnemyProjectile(mouth, _rangedTellDirection, ProjectileSpeed, ProjectileDamage)
		_attackTimer = maxf(0.2, AttackInterval - RangedTellSeconds)
		return
	_attackTimer -= dt
	if _attackTimer <= 0.0 and distance <= FireRange and distance > 1.0:
		_rangedTellDirection = toward.normalized()
		if RangedTellSeconds > 0.0:
			_rangedTellRemaining = RangedTellSeconds
			SetWarningFlash(0.25)
		else:
			var mouth = position + MouthOffset + _rangedTellDirection * ProjectileMuzzleDistance
			Game.SpawnEnemyProjectile(mouth, _rangedTellDirection, ProjectileSpeed, ProjectileDamage)
			_attackTimer = maxf(0.2, AttackInterval)

func MoveWithinArena(displacement, visualMargin = 48.0):
	position += displacement
	var boundary = Game.WallThickness + visualMargin
	position = Vector2(clampf(position.x, boundary, Game.ArenaSize.x - boundary), clampf(position.y, boundary, Game.ArenaSize.y - boundary))

func TakeHit(damage, direction, knockback, criticalHit = false):
	if Health <= 0.0: return
	Game.PlaySound(GameAudio.Sound.EnemyHit)
	# Avoid a base -> subclass script dependency cycle.
	if has_method("FireRing"): Game.RequestBossHitPause()
	var appliedDamage = maxf(1.0, damage)
	Health -= appliedDamage
	Game.SpawnDamageNumber(appliedDamage, position + Vector2(0, -32), criticalHit)
	_flashTime = HitFlashSeconds
	if _flashMaterial != null: _flashMaterial.set_shader_parameter("flash_amount", 1.0)
	position += direction.normalized() * maxf(0.0, knockback) * (1.0 - _resistance)
	var boundary = Game.WallThickness + HitWallMargin()
	position = Vector2(clampf(position.x, boundary, Game.ArenaSize.x - boundary), clampf(position.y, boundary, Game.ArenaSize.y - boundary))
	if Health <= 0.0:
		_defeated = true
		OnDefeated()
		Game.EnemyDefeated(self)

func OnDefeated(): pass
