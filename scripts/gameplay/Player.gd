class_name Player
extends Node2D

@export_range(0.1, 5, 0.1) var InvulnerabilitySeconds: float = 1
@export_range(20, 200, 1) var CoinPickupRadius: float = 78
@export var MaxHealth: float = 10
@export var MovementSpeed: float = 240
@export_range(0.05, 2, 0.05) var RegenTextBatchSeconds: float = 0.5
@export var Knockback: float = 0.0
@export var BonusDamagePercent: float = 0.0
@export var RangedDamagePercent: float = 0.0
@export var MeleeDamagePercent: float = 0.0
@export var CriticalChancePercent: float = 0.0
@export var ProjectileSizePercent: float = 0.0
@export var HealthRegenPerSecond: float = 0.0
@export var Spread: int = 0
@export var BulletPierce: int = 0
@export_range(0.02, 0.3, 0.01) var ShotShakeDuration: float = 0.09
@export_range(0, 12, 0.25) var MaximumShotShakePixels: float = 5

signal Died
signal Healed(amount: float)
signal Damaged(amount: float)

var EffectiveMovementSpeed: float:
	get: return maxf(1.0, MovementSpeed) + _coffeeSpeedBonus
var ProjectileSizeMultiplier: float:
	get: return maxf(0.1, 1.0 + ProjectileSizePercent / 100.0)
var IsMoving: bool:
	get: return ReadMovementInput() != Vector2.ZERO
var CannotShootWhileMoving = false
var Health = 0.0
var Active = false
var Game
var WeaponMount
var _sprite
var _groundShadow
var _coffeeParticles
var _headHpBar
var _camera
var _shakeRandom = RandomNumberGenerator.new()
var _shakeTime = 0.0
var _shakePixels = 0.0
var _invulnerable = 0.0
var _dying = false
var _deathTime = 0.0
var _coffeeSpeedBonus = 0.0
var _coffeeSecondsLeft = 0.0
var _regenTextAmount = 0.0
var _regenTextAge = 0.0
var _regenTickElapsed = 0.0
var _regenRemainder = 0.0

func _ready():
	_sprite = get_node("Visual")
	_groundShadow = get_node("GroundShadow")
	_coffeeParticles = get_node("CoffeeParticles")
	_headHpBar = get_node("HeadHpBar")
	_camera = get_node("Camera")
	WeaponMount = get_node("WeaponMount")
	_shakeRandom.randomize()
	Health = MaxHealth
	UpdateHeadHpBar()

func _process(dt):
	if Game != null and Game.IsImpactPaused: return
	UpdateShotShake(dt)
	if _dying:
		_headHpBar.visible = false
		_deathTime += dt
		scale = Vector2.ONE * maxf(0.0, 1.0 - _deathTime / 0.65)
		return
	if not Active: return
	var direction = ReadMovementInput()
	if direction.x < 0.0: _sprite.flip_h = true
	elif direction.x > 0.0: _sprite.flip_h = false
	position += direction.normalized() * EffectiveMovementSpeed * dt
	var boundary = Game.WallThickness + 34.0
	position = Vector2(clampf(position.x, boundary, Game.ArenaSize.x - boundary), clampf(position.y, boundary, Game.ArenaSize.y - boundary))
	_sprite.scale = Vector2(1.0, 1.0 + (0.0 if direction == Vector2.ZERO else 0.035 * sin(Time.get_ticks_msec() / 70.0)))
	var shadowPulse = 0.0 if direction == Vector2.ZERO else sin(Time.get_ticks_msec() / 85.0)
	_groundShadow.scale = _groundShadow.scale.lerp(Vector2(1.0 + 0.06 * shadowPulse, 1.0 - 0.08 * shadowPulse), clampf(dt * 12.0, 0.0, 1.0))
	_regenTickElapsed += dt
	while _regenTickElapsed >= 1.0:
		_regenTickElapsed -= 1.0
		if Health >= MaxHealth: continue
		_regenRemainder += maxf(0.0, HealthRegenPerSecond)
		var wholeHealing = int(_regenRemainder + 0.000001)
		if wholeHealing <= 0: continue
		_regenRemainder = maxf(0.0, _regenRemainder - wholeHealing)
		_regenTextAmount += RestoreHealth(wholeHealing, false)
	_regenTextAge += dt
	if _regenTextAmount > 0.0 and (_regenTextAge >= RegenTextBatchSeconds or Health >= MaxHealth): FlushRegenText()
	if _coffeeSecondsLeft > 0.0:
		_coffeeSecondsLeft = maxf(0.0, _coffeeSecondsLeft - dt)
		if _coffeeSecondsLeft <= 0.0:
			_coffeeSpeedBonus = 0.0
			_coffeeParticles.SetEmitting(false)
	UpdateHeadHpBar()
	_invulnerable = maxf(0.0, _invulnerable - dt)
	_sprite.modulate = Color(1, 1, 1, 0.35) if _invulnerable > 0.0 and int(Time.get_ticks_msec() / 90.0) % 2 == 0 else Color.WHITE

func Refill():
	FlushRegenText()
	RestoreHealth(MaxHealth - Health)
	_invulnerable = 0.0

func Heal(amount): return RestoreHealth(amount)

func RestoreHealth(amount, showText = true):
	var before = Health
	Health = minf(MaxHealth, Health + maxf(0.0, amount))
	var restored = Health - before
	if restored > 0.0 and showText: Healed.emit(restored)
	UpdateHeadHpBar()
	return restored

func FlushRegenText():
	if _regenTextAmount > 0.0: Healed.emit(_regenTextAmount)
	_regenTextAmount = 0.0
	_regenTextAge = 0.0

func ApplyCoffeeBoost(percent, seconds):
	_coffeeSpeedBonus = maxf(1.0, MovementSpeed) * maxf(0.0, percent) / 100.0
	_coffeeSecondsLeft = maxf(0.0, seconds)
	_coffeeParticles.SetEmitting(_coffeeSpeedBonus > 0.0 and _coffeeSecondsLeft > 0.0)

func ClearTemporaryEffects():
	FlushRegenText()
	_regenTickElapsed = 0.0
	_regenRemainder = 0.0
	_coffeeSpeedBonus = 0.0
	_coffeeSecondsLeft = 0.0
	_coffeeParticles.SetEmitting(false)

func UpdateHeadHpBar():
	_headHpBar.max_value = maxf(1.0, MaxHealth)
	_headHpBar.value = Health
	_headHpBar.visible = Health > 0.0 and Health < MaxHealth - 0.001

func ShakeOnShot(pixels):
	if pixels <= 0.0: return
	_shakePixels = minf(MaximumShotShakePixels, maxf(_shakePixels, pixels))
	_shakeTime = ShotShakeDuration

func UpdateShotShake(dt):
	if _shakeTime <= 0.0:
		_camera.offset = Vector2.ZERO
		_shakePixels = 0.0
		return
	_shakeTime = maxf(0.0, _shakeTime - dt)
	var fade = 0.0 if ShotShakeDuration <= 0.0 else _shakeTime / ShotShakeDuration
	_camera.offset = Vector2(_shakeRandom.randf_range(-1.0, 1.0), _shakeRandom.randf_range(-1.0, 1.0)) * _shakePixels * fade

static func ReadMovementInput():
	var direction = Vector2.ZERO
	if Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_LEFT): direction.x -= 1
	if Input.is_key_pressed(KEY_D) or Input.is_key_pressed(KEY_RIGHT): direction.x += 1
	if Input.is_key_pressed(KEY_W) or Input.is_key_pressed(KEY_UP): direction.y -= 1
	if Input.is_key_pressed(KEY_S) or Input.is_key_pressed(KEY_DOWN): direction.y += 1
	return direction

func ApplyModifier(modifier):
	if modifier == null: return
	match modifier.Stat:
		StatModifier.PlayerStat.MaxHealth:
			MaxHealth = maxf(1.0, MaxHealth + modifier.Amount)
			if modifier.Amount > 0.0: RestoreHealth(modifier.Amount)
			else:
				Health = clampf(Health + modifier.Amount, 1.0, MaxHealth)
				UpdateHeadHpBar()
		StatModifier.PlayerStat.MovementSpeed: MovementSpeed += modifier.Amount
		StatModifier.PlayerStat.Knockback: Knockback += modifier.Amount
		StatModifier.PlayerStat.BonusDamagePercent: BonusDamagePercent += modifier.Amount
		StatModifier.PlayerStat.RangedDamagePercent: RangedDamagePercent += modifier.Amount
		StatModifier.PlayerStat.MeleeDamagePercent: MeleeDamagePercent += modifier.Amount
		StatModifier.PlayerStat.CriticalChancePercent: CriticalChancePercent += modifier.Amount
		StatModifier.PlayerStat.ProjectileSizePercent: ProjectileSizePercent += modifier.Amount
		StatModifier.PlayerStat.BlockFiringWhileMoving:
			if modifier.Amount > 0.0: CannotShootWhileMoving = true
		StatModifier.PlayerStat.HealthRegenPerSecond: HealthRegenPerSecond += modifier.Amount
		StatModifier.PlayerStat.Spread: Spread = maxi(0, Spread + roundi(modifier.Amount))
		StatModifier.PlayerStat.BulletPierce: BulletPierce = maxi(0, BulletPierce + roundi(modifier.Amount))

func Hurt(damage = 1.0):
	if not Active or _invulnerable > 0.0 or _dying or damage <= 0.0: return
	if Game != null: Game.PlaySound(GameAudio.Sound.MohsenHit)
	var before = Health
	Health = maxf(0.0, Health - damage)
	UpdateHeadHpBar()
	if Health < before: Damaged.emit(before - Health)
	_invulnerable = InvulnerabilitySeconds
	if Health <= 0.0:
		Active = false
		_dying = true
		Died.emit()
