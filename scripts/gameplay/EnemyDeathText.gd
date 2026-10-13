class_name EnemyDeathText
extends Node2D

@export var Lifetime: float = 0.75
@export var RiseDistance: float = 65
@export var SideDrift: float = 16

var _label
var _origin = Vector2.ZERO
var _age = 0.0
var _drift = 0.0

func _ready():
	_label = get_node("Label")
	_origin = position
	_drift = randf() * 2.0 * SideDrift - SideDrift
	scale = Vector2.ONE * 0.55

func SetMessage(message): _label.text = message

func _process(dt):
	var game = get_parent()
	if game != null and game.get("IsImpactPaused") == true: return
	_age += dt
	var progress = clampf(_age / maxf(0.01, Lifetime), 0.0, 1.0)
	var pop = lerpf(0.55, 1.4, progress / 0.18) if progress < 0.18 else lerpf(1.4, 0.9, (progress - 0.18) / 0.82)
	scale = Vector2.ONE * pop
	position = _origin + Vector2(_drift * progress, -RiseDistance * (1.0 - (1.0 - progress) * (1.0 - progress)))
	var opacity = 1.0 if progress < 0.35 else 1.0 - (progress - 0.35) / 0.65
	modulate = Color(1, 1, 1, opacity)
	if _age >= Lifetime: queue_free()
