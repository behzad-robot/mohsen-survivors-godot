class_name DamageNumber
extends Node2D

@export var Lifetime: float = 0.7
@export var RiseDistance: float = 55
@export var SideDrift: float = 18
@export var SpawnJitter: Vector2 = Vector2(8, 6)
@export var CriticalColor: Color = Color(1, 0.85, 0.2)
@export var HealingColor: Color = Color(0.3, 1, 0.4)
@export var PlayerDamageColor: Color = Color(1, 0.24, 0.24)

var _label
var _origin = Vector2.ZERO
var _age = 0.0
var _drift = 0.0

func _ready():
	_label = get_node("Label")
	_origin = position + Vector2((randf() * 2.0 - 1.0) * SpawnJitter.x, (randf() * 2.0 - 1.0) * SpawnJitter.y)
	position = _origin
	_drift = randf() * 2.0 * SideDrift - SideDrift

func SetDamage(amount, criticalHit):
	_label.text = str(roundi(maxf(1.0, amount)))
	_label.add_theme_color_override("font_color", CriticalColor if criticalHit else Color.WHITE)

func SetHealing(amount):
	_label.text = "+" + str(snappedf(maxf(0.0, amount), 0.1))
	_label.add_theme_color_override("font_color", HealingColor)

func SetPlayerDamage(amount):
	_label.text = "-" + str(snappedf(maxf(0.0, amount), 0.01))
	_label.add_theme_color_override("font_color", PlayerDamageColor)

func _process(dt):
	var game = get_parent()
	if game != null and game.get("IsImpactPaused") == true: return
	_age += dt
	var progress = clampf(_age / maxf(0.01, Lifetime), 0.0, 1.0)
	var rise = 1.0 - (1.0 - progress) * (1.0 - progress)
	position = _origin + Vector2(_drift * progress, -RiseDistance * rise)
	scale = Vector2.ONE * lerpf(1.3, 0.9, progress)
	var opacity = 1.0 if progress < 0.35 else 1.0 - (progress - 0.35) / 0.65
	modulate = Color(1, 1, 1, opacity)
	if _age >= Lifetime: queue_free()
