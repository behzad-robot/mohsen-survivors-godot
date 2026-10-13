class_name CoffeeSpeedParticles
extends Node2D

@export_range(1, 50, 1) var DotsPerSecond: float = 18
@export_range(5, 60, 1) var HorizontalSpread: float = 23
@export_range(0.1, 2, 0.05) var DotLifetime: float = 0.75
@export_range(5, 150, 1) var RiseSpeed: float = 58
@export var DotColor: Color = Color(1, 0.88, 0.62, 0.72)

var _dots = []
var _random = RandomNumberGenerator.new()
var _spawnFraction = 0.0
var _emitting = false

func _ready(): _random.randomize()

func SetEmitting(enabled):
	if _emitting == enabled: return
	_emitting = enabled
	if enabled: return
	_dots.clear()
	_spawnFraction = 0.0
	queue_redraw()

func _process(dt):
	var player = get_parent()
	if not _emitting: return
	var game = player.get("Game") if player != null else null
	if game != null and game.IsImpactPaused: return
	for i in range(_dots.size() - 1, -1, -1):
		var dot = _dots[i]
		dot.age += dt
		if dot.age >= DotLifetime:
			_dots.remove_at(i)
			continue
		dot.position += dot.velocity * dt
	_spawnFraction += maxf(0.0, DotsPerSecond) * dt
	while _spawnFraction >= 1.0:
		_spawnFraction -= 1.0
		_dots.append({"position": Vector2(_random.randf_range(-HorizontalSpread, HorizontalSpread), _random.randf_range(12.0, 29.0)), "velocity": Vector2(_random.randf_range(-8.0, 8.0), -RiseSpeed * _random.randf_range(0.8, 1.2)), "radius": _random.randf_range(1.2, 2.1), "age": 0.0})
	queue_redraw()

func _draw():
	for dot in _dots:
		var opacity = DotColor.a * (1.0 - dot.age / maxf(0.01, DotLifetime))
		draw_circle(dot.position, dot.radius, Color(DotColor.r, DotColor.g, DotColor.b, opacity))
