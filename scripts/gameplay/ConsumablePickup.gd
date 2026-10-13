class_name ConsumablePickup
extends Node2D

enum PickupKind { Water, Coffee }

@export var Kind: PickupKind = PickupKind.Water
@export_range(0.1, 1, 0.05) var FlightSeconds: float = 0.48
@export_range(0, 100, 1) var ArcHeight: float = 42
@export_range(0, 30, 1) var LandingBounceHeight: float = 10
@export_range(0.2, 2, 0.05) var VisualScale: float = 1
@export_range(0, 300, 1) var CoffeeSpeedBonusPercent: float = 50
@export_range(0.1, 30, 0.1) var CoffeeDurationSeconds: float = 5
@export var InitialMagnetSpeed: float = 80
@export var MagnetAcceleration: float = 1800
@export var MaximumMagnetSpeed: float = 900
@export var CollectionDistance: float = 14
@export var SpringAwaySeconds: float = 0.12
@export var SpringAwayDistance: float = 18

var Game
var _visual
var _origin = Vector2.ZERO
var _landing = Vector2.ZERO
var _flightAge = 0.0
var _bounceAge = 0.0
var _flying = false
var _bouncing = false
var _collected = false
var _magnetized = false
var _magnetSpeed = 0.0
var _springAge = 0.0
var _springOrigin = Vector2.ZERO
var _springDirection = Vector2.ZERO

func _ready():
	_visual = get_node("Visual")
	_visual.scale = Vector2.ONE * VisualScale

func Launch(origin, landing):
	_origin = origin
	_landing = landing
	position = origin
	_flightAge = 0.0
	_flying = true
	_bouncing = false
	_magnetized = false
	_springAge = 0.0

func _process(dt):
	if Game == null or not Game.WaveActive or Game.IsImpactPaused: return
	if _flying:
		_flightAge += dt
		var t = clampf(_flightAge / maxf(0.01, FlightSeconds), 0.0, 1.0)
		position = _origin.lerp(_landing, t)
		_visual.position = Vector2(0, -ArcHeight * 4.0 * t * (1.0 - t))
		_visual.scale = Vector2.ONE * VisualScale * lerpf(0.65, 1.0, t)
		if t >= 1.0:
			_flying = false
			_bouncing = true
			_bounceAge = 0.0
		return
	if _bouncing:
		_bounceAge += dt
		var t = clampf(_bounceAge / 0.24, 0.0, 1.0)
		_visual.position = Vector2(0, -LandingBounceHeight * sin(t * PI))
		_visual.scale = Vector2.ONE * VisualScale * (1.0 + 0.12 * sin(t * PI))
		if t >= 1.0:
			_bouncing = false
			_visual.position = Vector2.ZERO
			_visual.scale = Vector2.ONE * VisualScale
		return
	if _collected or Game.Mohsen == null or not Game.Mohsen.Active: return
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
	if position.distance_squared_to(target) > CollectionDistance * CollectionDistance: return
	_collected = true
	Game.CollectConsumable(self)
	queue_free()
