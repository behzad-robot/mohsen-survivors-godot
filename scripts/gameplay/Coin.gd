class_name Coin
extends Node2D

@export var InitialMagnetSpeed: float = 80
@export var MagnetAcceleration: float = 1800
@export var MaximumMagnetSpeed: float = 900
@export var CollectionDistance: float = 14
@export var SpawnGrowSeconds: float = 0.22
@export var SpringAwaySeconds: float = 0.12
@export var SpringAwayDistance: float = 18

var Game
var _magnetized = false
var _magnetSpeed = 0.0
var _spawnAge = 0.0
var _springAge = 0.0
var _springOrigin = Vector2.ZERO
var _springDirection = Vector2.ZERO
var _visual

func _ready():
	_visual = get_node("Visual")
	_visual.scale = Vector2.ZERO

func _process(dt):
	if Game != null and Game.IsImpactPaused: return
	_spawnAge += dt
	var grow = clampf(_spawnAge / maxf(0.01, SpawnGrowSeconds), 0.0, 1.0)
	var visualScale = lerpf(0.0, 1.15, grow / 0.75) if grow < 0.75 else lerpf(1.15, 1.0, (grow - 0.75) / 0.25)
	_visual.scale = Vector2.ONE * visualScale
	if Game == null or not Game.WaveActive or Game.Mohsen == null: return
	var target = Game.Mohsen.position
	if not _magnetized:
		var radius = Game.Mohsen.CoinPickupRadius
		if position.distance_squared_to(target) > radius * radius: return
		_magnetized = true
		_magnetSpeed = InitialMagnetSpeed
		_springOrigin = position
		_springDirection = (position - target).normalized()
		if _springDirection == Vector2.ZERO: _springDirection = Vector2.RIGHT.rotated(randf() * TAU)
	if _springAge < SpringAwaySeconds:
		_springAge = minf(SpringAwaySeconds, _springAge + dt)
		var progress = _springAge / maxf(0.01, SpringAwaySeconds)
		position = _springOrigin + _springDirection * SpringAwayDistance * sin(progress * PI * 0.5)
		return
	_magnetSpeed = minf(MaximumMagnetSpeed, _magnetSpeed + MagnetAcceleration * dt)
	position = position.move_toward(target, _magnetSpeed * dt)
	if position.distance_squared_to(target) <= CollectionDistance * CollectionDistance:
		Game.CollectCoin(self)
		queue_free()
