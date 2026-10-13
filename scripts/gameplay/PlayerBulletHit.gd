class_name PlayerBulletHit
extends Node2D

@export_range(1, 60, 1) var FramesPerSecond: float = 24

var _visual
var _elapsed = 0.0

func _ready():
	_visual = get_node("Visual")
	_visual.frame = 0

func _process(dt):
	var game = get_parent()
	if game != null and game.get("IsImpactPaused") == true: return
	_elapsed += dt
	var frame = floori(_elapsed * maxf(1.0, FramesPerSecond))
	if frame >= _visual.hframes:
		queue_free()
		return
	_visual.frame = frame
