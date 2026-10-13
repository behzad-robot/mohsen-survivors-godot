class_name EnemySpawnGroupDefinition
extends Resource

@export var EnemyScene: PackedScene = null
@export var AlternateEnemyScene: PackedScene = null
@export_range(0, 100, 1) var Count: int = 1
@export_range(0.1, 10, 0.05) var HealthMultiplier: float = 1
@export_range(0.1, 10, 0.05) var SpeedMultiplier: float = 1
@export_range(-1, 1, 0.05) var KnockbackResistanceAdjustment: float = 0.0

