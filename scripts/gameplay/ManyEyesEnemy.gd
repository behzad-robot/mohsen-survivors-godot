class_name ManyEyesEnemy
extends Enemy

@export_range(0.3, 8, 0.1) var WanderTurnSeconds: float = 2.2
@export_range(0.3, 10, 0.1) var VolleyIntervalSeconds: float = 2.8
@export_range(0, 10, 0.1) var InitialVolleyDelaySeconds: float = 1.4
@export_range(50, 500, 10) var VolleyProjectileSpeed: float = 190
@export_range(0, 80, 1) var ProjectileOriginRadius: float = 28
@export var VolleyProjectileScene: PackedScene = null
@export var OffspringScene: PackedScene = null
@export_range(0, 16, 1) var OffspringCount: int = 6
@export_range(0, 100, 1) var OffspringSpawnRadius: float = 32
@export_range(0.1, 5, 0.1) var OffspringHealthMultiplier: float = 1
@export_range(0.1, 5, 0.1) var OffspringSpeedMultiplier: float = 1

var _random = RandomNumberGenerator.new()
var _wanderDirection = Vector2.ZERO
var _turnRemaining = 0.0
var _volleyRemaining = 0.0

func _ready():
	super._ready()
	_random.randomize()
	ChooseWanderDirection()
	_volleyRemaining = InitialVolleyDelaySeconds

func UpdateBehavior(dt, _toward, _distance):
	_turnRemaining -= dt
	if _turnRemaining <= 0.0: ChooseWanderDirection()
	var before = position
	var displacement = _wanderDirection * MovementSpeed * dt
	MoveWithinArena(displacement)
	if displacement.length_squared() > 0.0 and before.distance_squared_to(position) < displacement.length_squared() * 0.25: ChooseWanderDirection()
	_volleyRemaining -= dt
	if _volleyRemaining <= 0.0:
		FireVolley()
		_volleyRemaining = maxf(0.3, VolleyIntervalSeconds)

func ChooseWanderDirection():
	_wanderDirection = Vector2.RIGHT.rotated(_random.randf_range(0.0, TAU))
	_turnRemaining = maxf(0.3, WanderTurnSeconds) * _random.randf_range(0.75, 1.25)

func FireVolley():
	if VolleyProjectileScene == null: return
	var start = _random.randf_range(0.0, TAU)
	for i in range(8):
		var direction = Vector2.RIGHT.rotated(start + TAU * i / 8.0)
		Game.SpawnManyEyesProjectile(VolleyProjectileScene, position + direction * ProjectileOriginRadius, direction, VolleyProjectileSpeed, ProjectileDamage)

func OnDefeated():
	if OffspringScene == null: return
	var count = clampi(OffspringCount, 0, 16)
	var start = _random.randf_range(0.0, TAU)
	for i in range(count):
		var offset = Vector2.RIGHT.rotated(start + TAU * i / count) * OffspringSpawnRadius
		Game.SpawnEnemyOffspring(OffspringScene, position + offset, OffspringHealthMultiplier, OffspringSpeedMultiplier * ConfiguredSpeedMultiplier, BurstDamageMultiplier)
