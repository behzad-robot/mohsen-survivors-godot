class_name WaveDefinition
extends Resource

@export_range(1, 20, 1) var WaveNumber: int = 1
@export var WaveName: String = ""
@export_range(1, 180, 1) var Duration: float = 45
@export_range(0.1, 5, 0.05) var WaveDangerLevelHealthMultiplier: float = 1
@export_range(0.1, 10, 0.05) var WaveDangerLevelDamageMultiplier: float = 1
@export_range(0.1, 10, 0.05) var EnemySpeedMultiplier: float = 1
@export_range(0, 1, 0.01) var TankSpawnChance: float = 0.0
@export_range(0, 1, 0.01) var BossSpawnChance: float = 0.0
@export_range(0, 1, 0.01) var BossTankChance: float = 0.0
@export_range(0, 180, 0.5) var BossSpawnTime: float = 20
@export var BossSpawnOffset: Vector2 = Vector2(240, -220)
@export_range(0, 5, 0.1) var BossSpawnWarningSeconds: float = 1
@export var Bursts: Array[SpawnBurstDefinition] = []

