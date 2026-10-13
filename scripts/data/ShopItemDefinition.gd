class_name ShopItemDefinition
extends Resource

@export var DisplayName: String = "Passive item"
@export var Description: String = ""
@export var Icon: Texture2D = null
@export var Price: int = 9
@export_range(1, 3, 1) var Rarity: int = 1
@export var Unique: bool = false
@export var Modifiers: Array[StatModifier] = []

