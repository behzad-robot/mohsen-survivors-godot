class_name Projectile
extends Node2D

@export var Hostile: bool = false
@export var Knife: bool = false
@export_range(0.05, 15, 0.05) var LifetimeSeconds: float = 1.5
@export_range(1, 80, 1) var HitRadius: float = 21
@export var EaseSpeed: bool = false
@export_range(0, 1, 0.05) var EndSpeedMultiplier: float = 0.25
@export_range(0.5, 4, 0.1) var EaseExponent: float = 1.5
@export_range(0.02, 0.5, 0.01) var TrailLifetimeSeconds: float = 0.16
@export_range(0, 1000, 10) var Speed: float = 0.0

var Game
var Direction = Vector2.ZERO
var Damage = 0.0
var SizeMultiplier = 1.0
var CriticalHit = false
var Knockback = 0.0
var RemainingDistance = 0.0
var RangeAtSpawn = 0.0
var SourceScene
var CanSpread = true
var PierceCapacity = 0
var RemainingPierces = 0
var _hitEnemies = {}
var _trailPointTimes = []
var _remainingLife = 0.0
var _trailClock = 0.0
var _trail
var EffectiveHitRadius: float:
	get: return HitRadius * maxf(0.1, SizeMultiplier)

func IgnoreEnemyForLife(enemyId): _hitEnemies[enemyId] = true

func _ready():
	_remainingLife = LifetimeSeconds
	rotation = Direction.angle()
	var visual = get_node_or_null("Visual")
	var visualSize = maxf(0.1, SizeMultiplier)
	if visual != null: visual.scale *= visualSize
	if visual is AnimatedSprite2D: visual.play("default")
	_trail = get_node_or_null("Trail")
	if _trail != null:
		_trail.width *= visualSize
		_trail.top_level = true
		_trail.global_position = Vector2.ZERO
		var gradient = Gradient.new()
		gradient.set_color(0, Color(1, 1, 1, 0))
		gradient.set_color(1, Color(1, 1, 1, 0.9))
		_trail.gradient = gradient
		_trail.add_point(global_position)
		_trailPointTimes.append(0.0)

func _process(dt):
	if Game == null or not Game.WaveActive or Game.IsImpactPaused: return
	var activeDelta = minf(dt, _remainingLife)
	var from = position
	var lifeProgress = 1.0 if LifetimeSeconds <= 0.0 else 1.0 - _remainingLife / LifetimeSeconds
	var speed = lerpf(Speed, Speed * EndSpeedMultiplier, pow(lifeProgress, EaseExponent)) if EaseSpeed else Speed
	var travel = minf(speed * activeDelta, RemainingDistance)
	var to = from + Direction * travel
	position = to
	UpdateTrail(dt)
	RemainingDistance -= travel
	_remainingLife -= activeDelta
	if Hostile:
		if SegmentHitTime(from, to, Game.Mohsen.position, EffectiveHitRadius + 1.0) >= 0.0:
			Game.Mohsen.Hurt(Damage)
			queue_free()
			return
	elif Knife:
		for enemy in Game.Enemies.duplicate():
			if not is_instance_valid(enemy) or _hitEnemies.has(enemy.get_instance_id()): continue
			if EnemyHitTime(from, to, enemy, EffectiveHitRadius) < 0.0: continue
			_hitEnemies[enemy.get_instance_id()] = true
			enemy.TakeHit(Damage, Direction, Knockback, CriticalHit)
			if CriticalHit: Game.RequestKnifeCritPause()
	else:
		var hits = []
		for enemy in Game.Enemies.duplicate():
			if not is_instance_valid(enemy) or enemy.Health <= 0.0 or _hitEnemies.has(enemy.get_instance_id()): continue
			var hitTime = EnemyHitTime(from, to, enemy, EffectiveHitRadius)
			if hitTime >= 0.0: hits.append({"enemy": enemy, "time": hitTime})
		hits.sort_custom(func(a, b): return a.time < b.time)
		for hit in hits:
			if not is_instance_valid(hit.enemy) or hit.enemy.Health <= 0.0: continue
			var targetId = hit.enemy.get_instance_id()
			if _hitEnemies.has(targetId): continue
			_hitEnemies[targetId] = true
			var hitPosition = from.lerp(to, hit.time)
			hit.enemy.TakeHit(Damage, Direction, Knockback, CriticalHit)
			Game.SpawnPlayerBulletHit(hitPosition)
			if CanSpread and SourceScene != null and Game.Mohsen.Spread > 0:
				Game.SpawnSpreadBullets(self, hitPosition, targetId)
			if RemainingPierces <= 0:
				queue_free()
				return
			RemainingPierces -= 1
	if _remainingLife <= 0.0 or RemainingDistance <= 0.0 or position.x < 0.0 or position.y < 0.0 or position.x > Game.ArenaSize.x or position.y > Game.ArenaSize.y:
		queue_free()

func UpdateTrail(dt):
	if _trail == null: return
	_trailClock += dt
	_trail.add_point(global_position)
	_trailPointTimes.append(_trailClock)
	while _trailPointTimes.size() > 2 and _trailPointTimes[0] < _trailClock - TrailLifetimeSeconds:
		_trailPointTimes.pop_front()
		_trail.remove_point(0)

static func SegmentHitTime(from, to, target, radius):
	var segment = to - from
	var lengthSquared = segment.length_squared()
	var time = 0.0 if lengthSquared <= 0.0001 else clampf((target - from).dot(segment) / lengthSquared, 0.0, 1.0)
	var closest = from + segment * time
	return time if closest.distance_squared_to(target) <= radius * radius else -1.0

static func EnemyHitTime(from, to, enemy, projectileRadius):
	var body = enemy.HurtboxHalfExtents
	if body.x <= 0.0 or body.y <= 0.0: return SegmentHitTime(from, to, enemy.position, projectileRadius)
	var rx = body.x + projectileRadius
	var ry = body.y + projectileRadius
	var start = Vector2((from.x - enemy.position.x) / rx, (from.y - enemy.position.y) / ry)
	var finish = Vector2((to.x - enemy.position.x) / rx, (to.y - enemy.position.y) / ry)
	var movement = finish - start
	if start.length_squared() <= 1.0: return 0.0
	var a = movement.length_squared()
	if a <= 0.000001: return -1.0
	var b = 2.0 * start.dot(movement)
	var c = start.length_squared() - 1.0
	var discriminant = b * b - 4.0 * a * c
	if discriminant < 0.0: return -1.0
	var entry = (-b - sqrt(discriminant)) / (2.0 * a)
	return entry if entry >= 0.0 and entry <= 1.0 else -1.0
