class_name WaterDispenser
extends Node2D

@export_range(1, 30, 0.5) var DropIntervalSeconds: float = 10

var Game
var _elapsed = 0.0

func _process(dt):
	if Game == null or not Game.WaveActive or Game.IsImpactPaused: return
	_elapsed += dt
	if _elapsed < DropIntervalSeconds: return
	_elapsed -= maxf(0.1, DropIntervalSeconds)
	Game.SpawnWater(self)
