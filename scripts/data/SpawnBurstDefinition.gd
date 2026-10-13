class_name SpawnBurstDefinition
extends Resource

@export_range(0, 45, 0.5) var TriggerTime: float = 0.0
@export_range(0, 10, 0.1) var WarningDuration: float = 1
@export_range(0.1, 10, 0.05) var DamageMultiplier: float = 1
@export var Groups: Array[EnemySpawnGroupDefinition] = []

