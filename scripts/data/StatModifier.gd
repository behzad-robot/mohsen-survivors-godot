class_name StatModifier
extends Resource

enum PlayerStat { MaxHealth, MovementSpeed, Knockback, BonusDamagePercent, RangedDamagePercent, MeleeDamagePercent, HealthRegenPerSecond, Spread, BulletPierce, CriticalChancePercent, ProjectileSizePercent, BlockFiringWhileMoving }

@export var Stat: PlayerStat = PlayerStat.BonusDamagePercent
@export var Amount: float = 0

