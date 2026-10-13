class_name SpawnWarning
extends Sprite2D

@export_range(16, 64, 1) var InitialRingRadius: float = 35
@export_range(4, 32, 1) var FinalRingRadius: float = 13
@export_range(1, 6, 0.5) var RingWidth: float = 2.5
@export_range(0, 8, 1) var PulseCount: float = 3

var _duration = 1.0
var _progress = 0.0

func SetTiming(duration, remaining):
	_duration = maxf(0.001, duration)
	_progress = clampf(1.0 - remaining / _duration, 0.0, 1.0)
	var pulse = 0.5 + 0.5 * sin(_progress * PulseCount * TAU)
	modulate = Color(1, 1, 1, 0.68 + 0.22 * pulse)
	queue_redraw()

func _draw():
	var radius = lerpf(InitialRingRadius, FinalRingRadius, _progress)
	draw_arc(Vector2.ZERO, radius, 0.0, TAU, 32, Color(1, 0.14, 0.14, 0.85), RingWidth, true)
