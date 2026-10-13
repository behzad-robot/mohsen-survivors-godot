class_name ShotgunTierDefinition
extends Resource

@export var DamagePerPellet: float = 0.0
@export var AttackInterval: float = 0.0
@export var TargetRange: float = 0.0
@export var CritChance: float = 0.0
@export var CritMultiplier: float = 0.0
@export var KnockbackPerPellet: float = 0.0
@export var RangedDamageScaling: float = 0.0
@export_range(1, 16, 1) var PelletCount: int = 0
@export_range(0, 120, 1) var SpreadDegrees: float = 0.0
@export var PelletSpeed: float = 0.0
@export var PelletLifetimeSeconds: float = 0.0

var Stats: Dictionary:
	get:
		return {"Damage": DamagePerPellet, "Interval": AttackInterval, "Range": TargetRange, "CritChance": CritChance, "CritMultiplier": CritMultiplier, "Knockback": KnockbackPerPellet, "Scaling": RangedDamageScaling}
