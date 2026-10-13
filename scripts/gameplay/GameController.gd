class_name GameController
extends Node2D

@export var PlayerScene: PackedScene = load("res://scenes/player/player.tscn")
@export var KnifeScene: PackedScene = load("res://scenes/weapons/knife.tscn")
@export var PistolScene: PackedScene = load("res://scenes/weapons/pistol.tscn")
@export var SmgScene: PackedScene = load("res://scenes/weapons/smg.tscn")
@export var ShotgunScene: PackedScene = load("res://scenes/weapons/shotgun.tscn")
@export var Waves: Array[WaveDefinition] = []
@export var WarningScene: PackedScene = load("res://scenes/effects/spawn_warning.tscn")
@export var CoinScene: PackedScene = load("res://scenes/pickups/coin.tscn")
@export var WaterDispenserScene: PackedScene = load("res://scenes/pickups/water_dispenser.tscn")
@export var WaterPickupScene: PackedScene = load("res://scenes/pickups/glass_of_water.tscn")
@export var CoffeePickupScene: PackedScene = load("res://scenes/pickups/coffee_cup.tscn")
@export_range(0, 1, 0.01) var CoffeeDropChance: float = 0.1
@export_range(1, 10, 1) var MinimumWaterDispensers: int = 1
@export_range(1, 10, 1) var MaximumWaterDispensers: int = 3
@export_range(0, 300, 1) var WaterDropMinimumDistance: float = 45
@export_range(0, 300, 1) var WaterDropMaximumDistance: float = 105
@export_range(0, 100, 0.5) var WaterHealing: float = 3
@export var DamageNumberScene: PackedScene = load("res://scenes/effects/damage_number.tscn")
@export var EnemyDeathTextScene: PackedScene = load("res://scenes/effects/enemy_death_text.tscn")
@export var PlayerBulletHitScene: PackedScene = load("res://scenes/effects/player_bullet_hit.tscn")
@export var EnemyExplosionScene: PackedScene = load("res://scenes/effects/enemy_explosion.tscn")
@export var PistolBulletScene: PackedScene = load("res://scenes/projectiles/pistol_bullet.tscn")
@export var SmgBulletScene: PackedScene = load("res://scenes/projectiles/smg_bullet.tscn")
@export var ShotgunPelletScene: PackedScene = load("res://scenes/projectiles/shotgun_pellet.tscn")
@export var ShotgunTiers: Array[ShotgunTierDefinition] = []
@export var EnemyBulletScene: PackedScene = load("res://scenes/projectiles/enemy_bullet.tscn")
@export var KnifeAttackScene: PackedScene = load("res://scenes/projectiles/knife_attack.tscn")
@export var BossScene: PackedScene = load("res://scenes/enemies/orange_boss.tscn")
@export var MeleeTankScene: PackedScene = load("res://scenes/enemies/melee_tank.tscn")
@export var FemaleMeleeTankScene: PackedScene = load("res://scenes/enemies/melee_female_tank.tscn")
@export var RangedTankScene: PackedScene = load("res://scenes/enemies/ranged_tank.tscn")
@export var ManyEyesTankScene: PackedScene = load("res://scenes/enemies/many_eyes_tank.tscn")
@export var OneEyeMinionTankScene: PackedScene = load("res://scenes/enemies/one_eye_minion_tank.tscn")
@export var BossTankScene: PackedScene = load("res://scenes/enemies/orange_boss_tank.tscn")
@export var WaveOneBossOffset: Vector2 = Vector2(0, -240)
@export var ItemPool: Array[ShopItemDefinition] = []
@export var MinimumSpawnDistance: float = 390
@export_range(0, 32, 1) var CoinDropScatterRadius: float = 12
@export var TierOneWeaponPrice: int = 18
@export var WeaponTierPriceStep: int = 12
@export_range(0, 0.2, 0.005) var KnifeCritPauseSeconds: float = 0.055
@export_range(0, 0.2, 0.005) var BossHitPauseSeconds: float = 0.045
@export_range(0, 0.2, 0.005) var DefeatPauseSeconds: float = 0.04
@export_range(0, 0.5, 0.01) var ImpactPauseMinGapSeconds: float = 0.11
@export_range(0, 0.5, 0.01) var HudDamagePunchSeconds: float = 0.22
@export_range(0, 0.3, 0.01) var HudDamagePunchScale: float = 0.12
@export_range(0, 2, 0.05) var WaveOpeningBeatSeconds: float = 0.75
@export_range(0, 2, 0.05) var WaveClearBeatSeconds: float = 0.65
@export_range(0, 0.5, 0.01) var HudCoinBounceSeconds: float = 0.18
@export_range(0, 0.3, 0.01) var HudCoinBounceScale: float = 0.16
@export var MenuScene: PackedScene = load("res://scenes/ui/menu.tscn")
@export var ChoiceScene: PackedScene = load("res://scenes/ui/weapon_choice.tscn")
@export var ShopScene: PackedScene = load("res://scenes/ui/shop.tscn")
@export var ShopOfferScene: PackedScene = load("res://scenes/ui/shop_offer.tscn")
@export var ShopInventoryEntryScene: PackedScene = load("res://scenes/ui/shop_inventory_entry.tscn")
@export var VictoryScene: PackedScene = load("res://scenes/ui/victory.tscn")
@export var DefeatScene: PackedScene = load("res://scenes/ui/defeat.tscn")
@export var HudScene: PackedScene = load("res://scenes/ui/hud.tscn")
@export var GameAudioScene: PackedScene = load("res://scenes/effects/game_audio.tscn")

const ShopGreeting = "welcome to VX the Greatest function of Snappfood"
const DeathMessages = ["CLOSED", "RESOLVED", "PENDING", "CANCELLED", "NOT VALID", "NOT ENOUGH INFO"]
var ArenaSize: Vector2:
	get: return _arena.Size
var WallThickness: float:
	get: return _arena.WallThickness
var IsImpactPaused: bool:
	get: return WaveActive and (_impactPauseRemaining > 0.0 or _impactPauseFrame)
var Mohsen
var WaveActive = false
var Enemies = []
var _coins = []
var _waterDispensers = []
var _consumables = []
var _projectiles = []
var _weapons = []
var _purchasedItems = {}
var _pending = []
var _offers = [null, null, null, null]
var _locks = [false, false, false, false]
var _arena
var _canvas
var _screen
var _hudScreen
var _audio
var _hudHpBar
var _hudHpText
var _hudDamageEdges
var _hudCoinText
var _hudCoinIcon
var _hudWaveName
var _hudTimer
var _hudWaveBeat
var _hudEquipment
var _waveIndex = 0
var _coinsHeld = 0
var _pendingCheatCoins = 0
var _rerollCount = 0
var _waveClock = 0.0
var _rng = RandomNumberGenerator.new()
var _currentBurst = 0
var _bossOpportunityPending = false
var _message = ""
var _messageVersion = 0
var _lastShownMessageVersion = -1
var _hasEnteredShop = false
var _productManagerDebugOffers = false
var _finishDelay = 0.0
var _waveBeatRemaining = 0.0
var _waveBeatDuration = 0.0
var _waveClearRemaining = 0.0
var _waveClearForceShop = false
var _impactPauseRemaining = 0.0
var _hudDamagePunchRemaining = 0.0
var _hudCoinBounceRemaining = 0.0
var _impactPauseCooldown = 0.0
var _impactPauseFrame = false

func _ready():
	_arena = get_node("Arena")
	WeaponData.ConfigureShotgunProfiles(ShotgunTiers)
	_rng.randomize()
	if GameAudioScene != null:
		_audio = GameAudioScene.instantiate()
		add_child(_audio)
	if Waves.is_empty():
		for i in range(1, 11):
			var wave = load("res://resources/waves/wave_%02d.tres" % i)
			if wave != null: Waves.append(wave)
	if ItemPool.is_empty():
		var itemPaths = ["pooria", "narges", "behzad", "sara", "milad", "saman", "arsalan", "mohammad-hossein", "amir-hossein", "hojat", "omid", "hossein", "saeed", "arezoo", "mania", "sahar", "sahel", "sepideh", "sahar-2", "maryam", "devops-buddy-1", "devops-buddy-2", "devops-buddy-3", "hookah", "propranolol_pills", "heavy_smoker", "strawberry_milkshake", "bluetooth_headset"]
		for path in itemPaths:
			var item = load("res://resources/items/%s.tres" % path)
			if item != null: ItemPool.append(item)
	_canvas = CanvasLayer.new()
	add_child(_canvas)
	_hudScreen = HudScene.instantiate()
	_canvas.add_child(_hudScreen)
	_hudHpBar = _hudScreen.get_node("HpBar")
	_hudHpText = _hudScreen.get_node("HpText")
	_hudDamageEdges = _hudScreen.get_node("DamageEdges")
	_hudCoinText = _hudScreen.get_node("CoinText")
	_hudCoinIcon = _hudScreen.get_node("CoinIcon")
	_hudWaveName = _hudScreen.get_node("WaveName")
	_hudTimer = _hudScreen.get_node("Timer")
	_hudWaveBeat = _hudScreen.get_node("WaveBeat")
	_hudEquipment = _hudScreen.get_node("Equipment")
	_hudScreen.visible = false
	ShowMenu()

func _process(dt):
	UpdateHudDamagePunch(dt)
	UpdateHudCoinBounce(dt)
	UpdateWaveBeat(dt)
	if _waveClearRemaining > 0.0:
		_waveClearRemaining = maxf(0.0, _waveClearRemaining - dt)
		if _waveClearRemaining <= 0.0: CompleteWaveTransition()
		return
	if _finishDelay > 0.0:
		_finishDelay -= dt
		if _finishDelay <= 0.0: ShowFinish(false)
	_impactPauseCooldown = maxf(0.0, _impactPauseCooldown - dt)
	if _impactPauseRemaining > 0.0:
		_impactPauseRemaining = maxf(0.0, _impactPauseRemaining - dt)
		_impactPauseFrame = true
		return
	_impactPauseFrame = false
	if not WaveActive: return
	_waveClock += dt
	var wave = Waves[_waveIndex]
	while _currentBurst < wave.Bursts.size() and wave.Bursts[_currentBurst].TriggerTime <= _waveClock:
		BeginBurst(wave.Bursts[_currentBurst])
		_currentBurst += 1
	if _bossOpportunityPending and _waveClock >= wave.BossSpawnTime:
		_bossOpportunityPending = false
		BeginBossOpportunity(wave)
	for i in range(_pending.size() - 1, -1, -1):
		for entry in _pending[i].Entries:
			if is_instance_valid(entry.warning): entry.warning.SetTiming(_pending[i].WarningDuration, _pending[i].Due - _waveClock)
		if _pending[i].Due <= _waveClock:
			SpawnPending(_pending[i])
			_pending.remove_at(i)
	Enemies = Enemies.filter(func(enemy): return is_instance_valid(enemy))
	for weapon in _weapons: weapon.Tick(dt)
	UpdateHud()
	if _waveClock >= wave.Duration: EndWave()

func _input(event):
	if not OS.has_feature("editor") or not event is InputEventKey or not event.pressed or event.echo: return
	var inShop = not WaveActive and is_instance_valid(_screen) and _screen.has_node("Offers")
	match event.keycode:
		KEY_C:
			_coinsHeld += 100
			if not WaveActive and not inShop: _pendingCheatCoins += 100
			_hudCoinText.text = "Coins: %d" % _coinsHeld
			if inShop: _screen.get_node("Coins").text = ShopCoinsText()
			get_viewport().set_input_as_handled()
		KEY_P:
			if inShop:
				DebugProductManagerReroll()
				get_viewport().set_input_as_handled()
		KEY_O:
			if WaveActive:
				EndWave(true)
				get_viewport().set_input_as_handled()
		KEY_R:
			get_viewport().set_input_as_handled()
			get_tree().reload_current_scene()

func ShowMenu():
	WaveActive = false
	_hudScreen.visible = false
	ShowScreen(MenuScene)
	_screen.get_node("Play").pressed.connect(ShowStartingChoice)

func ShowStartingChoice():
	ShowScreen(ChoiceScene)
	_screen.get_node("Panel/Box/Knife").pressed.connect(StartRun.bind(WeaponData.Kind.Knife))
	_screen.get_node("Panel/Box/Pistol").pressed.connect(StartRun.bind(WeaponData.Kind.Pistol))
	_screen.get_node("Panel/Box/Smg").pressed.connect(StartRun.bind(WeaponData.Kind.Smg))
	_screen.get_node("Panel/Box/Shotgun").pressed.connect(StartRun.bind(WeaponData.Kind.Shotgun))

func StartRun(startingWeapon):
	ClearWorld()
	ResetImpactPause()
	_offers.fill(null)
	_locks.fill(false)
	_purchasedItems.clear()
	_waveIndex = 0
	_coinsHeld = _pendingCheatCoins
	_pendingCheatCoins = 0
	_rerollCount = 0
	_message = ""
	_messageVersion = 0
	_lastShownMessageVersion = -1
	_hasEnteredShop = false
	_productManagerDebugOffers = false
	Mohsen = PlayerScene.instantiate()
	Mohsen.position = ArenaSize / 2.0
	Mohsen.Game = self
	add_child(Mohsen)
	Mohsen.Died.connect(OnPlayerDied)
	Mohsen.Healed.connect(OnPlayerHealed)
	Mohsen.Damaged.connect(OnPlayerDamaged)
	AddWeapon(startingWeapon, 1)
	StartWave()

func StartWave():
	if _waveIndex >= Waves.size():
		ShowFinish(true)
		return
	_waveClearRemaining = 0.0
	ClearScreen()
	FreeObjects(_coins)
	ClearWaterAndCoffee()
	Mohsen.ClearTemporaryEffects()
	_waveClock = 0.0
	_currentBurst = 0
	var wave = Waves[_waveIndex]
	_bossOpportunityPending = wave.WaveNumber >= 3 and _rng.randf() < clampf(wave.BossSpawnChance, 0.0, 1.0)
	_pending.clear()
	_finishDelay = 0.0
	ResetImpactPause()
	Mohsen.Refill()
	Mohsen.Active = true
	for weapon in _weapons: weapon.ResetTransientEffects()
	WaveActive = true
	if wave.WaveNumber > 1: SpawnWaterDispensers()
	PlaySound(GameAudio.Sound.WaveStart)
	if _waveIndex == 0: SpawnWaveOneBoss()
	_hudScreen.visible = true
	UpdateHud()
	BeginWaveBeat("WAVE %d START" % wave.WaveNumber, WaveOpeningBeatSeconds)

func SpawnWaveOneBoss():
	if BossScene == null: return
	var wave = Waves[_waveIndex]
	var boss = BossScene.instantiate()
	boss.position = Mohsen.position + WaveOneBossOffset
	add_child(boss)
	boss.Configure(self, wave.WaveNumber, wave.WaveDangerLevelHealthMultiplier, wave.WaveDangerLevelDamageMultiplier, 1.0, wave.EnemySpeedMultiplier, 0.0)
	boss.SetWaveName(wave.WaveName, wave.WaveNumber)
	Enemies.append(boss)

func BeginBossOpportunity(wave):
	if BossScene == null or WarningScene == null: return
	var warningSeconds = maxf(0.0, wave.BossSpawnWarningSeconds)
	if _waveClock + warningSeconds > wave.Duration: return
	var margin = WallThickness + 135.0
	var desired = Mohsen.position + wave.BossSpawnOffset
	var point = Vector2(clampf(desired.x, margin, ArenaSize.x - margin), clampf(desired.y, margin, ArenaSize.y - margin))
	var scene = SelectTankScene(BossScene, wave.BossTankChance)
	var warning = WarningScene.instantiate()
	warning.position = point
	add_child(warning)
	warning.SetTiming(warningSeconds, warningSeconds)
	_pending.append({"Due": _waveClock + warningSeconds, "WarningDuration": warningSeconds, "Entries": [SpawnEntry(scene, point, 1.0, 1.0, 0.0, 1.0, warning)]})

func EndWave(forceShop = false):
	ResetImpactPause()
	_bossOpportunityPending = false
	WaveActive = false
	Mohsen.Active = false
	Mohsen.ClearTemporaryEffects()
	for weapon in _weapons: weapon.ResetTransientEffects()
	ClearWaveObjects()
	_waveClearForceShop = forceShop
	BeginWaveBeat("WAVE CLEARED", WaveClearBeatSeconds)
	_waveClearRemaining = WaveClearBeatSeconds
	if _waveClearRemaining <= 0.0: CompleteWaveTransition()

func CompleteWaveTransition():
	_waveClearRemaining = 0.0
	_waveBeatRemaining = 0.0
	_hudWaveBeat.visible = false
	_hudScreen.visible = false
	var forceShop = _waveClearForceShop
	_waveClearForceShop = false
	if _waveIndex == Waves.size() - 1 and not forceShop:
		ShowFinish(true)
		return
	_productManagerDebugOffers = false
	_rerollCount = 0
	SetShopMessage("" if _hasEnteredShop else ShopGreeting)
	_hasEnteredShop = true
	GenerateOffers()
	ShowShop()

func OnPlayerDied():
	ResetImpactPause()
	_bossOpportunityPending = false
	WaveActive = false
	Mohsen.ClearTemporaryEffects()
	for weapon in _weapons: weapon.ResetTransientEffects()
	ClearWaveObjects()
	_waveBeatRemaining = 0.0
	_waveClearRemaining = 0.0
	_hudWaveBeat.visible = false
	UpdateHud()
	_finishDelay = 0.7

func SpawnEntry(scene, point, hp, speed, resistance, damage, warning):
	return {"scene": scene, "position": point, "hp": hp, "speed": speed, "resistance": resistance, "damage": damage, "warning": warning}

func BeginBurst(burst):
	if burst == null or burst.WarningDuration < 0.0 or burst.TriggerTime + burst.WarningDuration > Waves[_waveIndex].Duration: return
	var pending = {"Due": _waveClock + burst.WarningDuration, "WarningDuration": burst.WarningDuration, "Entries": []}
	for group in burst.Groups:
		if group == null or group.EnemyScene == null or group.Count < 0: continue
		var alternatePhase = _rng.randi_range(0, 1)
		for n in range(mini(group.Count, 100)):
			var point = ChooseSpawnPosition()
			var warning = WarningScene.instantiate()
			warning.position = point
			add_child(warning)
			warning.SetTiming(burst.WarningDuration, burst.WarningDuration)
			var scene = group.AlternateEnemyScene if group.AlternateEnemyScene != null and (n + alternatePhase) % 2 == 0 else group.EnemyScene
			scene = SelectTankScene(scene, Waves[_waveIndex].TankSpawnChance)
			pending.Entries.append(SpawnEntry(scene, point, group.HealthMultiplier, group.SpeedMultiplier, group.KnockbackResistanceAdjustment, burst.DamageMultiplier, warning))
	_pending.append(pending)

func SelectTankScene(baseScene, chance):
	if baseScene == null or Waves[_waveIndex].WaveNumber < 3 or _rng.randf() >= clampf(chance, 0.0, 1.0): return baseScene
	var tank = {
		"res://scenes/enemies/melee.tscn": MeleeTankScene,
		"res://scenes/enemies/melee_female.tscn": FemaleMeleeTankScene,
		"res://scenes/enemies/ranged.tscn": RangedTankScene,
		"res://scenes/enemies/many_eyes.tscn": ManyEyesTankScene,
		"res://scenes/enemies/one_eye_minion.tscn": OneEyeMinionTankScene,
		"res://scenes/enemies/orange_boss.tscn": BossTankScene}.get(baseScene.resource_path)
	return tank if tank != null else baseScene

func ChooseSpawnPosition():
	var chosen = ArenaSize / 2.0
	var margin = WallThickness + 50.0
	for attempt in range(40):
		chosen = Vector2(_rng.randf_range(margin, ArenaSize.x - margin), _rng.randf_range(margin, ArenaSize.y - margin))
		if chosen.distance_to(Mohsen.position) < MinimumSpawnDistance: continue
		var nearby = false
		for pending in _pending:
			for entry in pending.Entries:
				if entry.position.distance_to(chosen) < 42.0: nearby = true
		if not nearby: return chosen
	return chosen

func SpawnPending(pending):
	for entry in pending.Entries:
		if is_instance_valid(entry.warning): entry.warning.queue_free()
		var enemy = entry.scene.instantiate()
		if not enemy is Enemy:
			enemy.free()
			continue
		enemy.position = entry.position
		add_child(enemy)
		var wave = Waves[_waveIndex]
		enemy.Configure(self, wave.WaveNumber, wave.WaveDangerLevelHealthMultiplier, wave.WaveDangerLevelDamageMultiplier, entry.hp, maxf(0.1, entry.speed * wave.EnemySpeedMultiplier), entry.resistance, entry.damage)
		if enemy.has_method("SetWaveName"): enemy.SetWaveName(wave.WaveName, wave.WaveNumber)
		Enemies.append(enemy)
func EnemyDefeated(enemy):
	Enemies.erase(enemy)
	if not WaveActive: return
	RequestImpactPause(DefeatPauseSeconds)
	PlaySound(GameAudio.Sound.EnemyDefeat)
	if EnemyExplosionScene != null:
		var explosion = EnemyExplosionScene.instantiate()
		add_child(explosion)
		explosion.global_position = enemy.global_position
	if EnemyDeathTextScene != null:
		var effect = EnemyDeathTextScene.instantiate()
		effect.position = enemy.position + Vector2(0, -32)
		add_child(effect)
		effect.SetMessage(DeathMessages[_rng.randi_range(0, DeathMessages.size() - 1)])
	var roll = _rng.randi_range(0, 99)
	var dropCount = 0 if roll < 10 else (1 if roll < 40 else (2 if roll < 75 else 3))
	for i in range(dropCount):
		if CoinScene == null: break
		var coin = CoinScene.instantiate()
		coin.Game = self
		coin.position = enemy.position + Vector2.RIGHT.rotated(_rng.randf_range(0.0, TAU)) * _rng.randf_range(0.0, CoinDropScatterRadius)
		add_child(coin)
		_coins.append(coin)
	if CoffeePickupScene != null and _rng.randf() < clampf(CoffeeDropChance, 0.0, 1.0):
		var coffee = CoffeePickupScene.instantiate()
		coffee.Game = self
		coffee.position = enemy.position + Vector2.RIGHT.rotated(_rng.randf_range(0.0, TAU)) * _rng.randf_range(0.0, CoinDropScatterRadius)
		add_child(coffee)
		_consumables.append(coffee)

func SpawnWaterDispensers():
	if WaterDispenserScene == null: return
	var minimum = maxi(1, MinimumWaterDispensers)
	var count = _rng.randi_range(minimum, maxi(minimum, MaximumWaterDispensers))
	var margin = WallThickness + 72.0
	for i in range(count):
		var point = Vector2.ZERO
		for attempt in range(20):
			point = Vector2(_rng.randf_range(margin, ArenaSize.x - margin), _rng.randf_range(margin, ArenaSize.y - margin))
			if point.distance_to(Mohsen.position) < 180.0: continue
			var nearby = false
			for dispenser in _waterDispensers:
				if dispenser.position.distance_to(point) < 130.0: nearby = true
			if nearby: continue
			break
		var dispenser = WaterDispenserScene.instantiate()
		dispenser.Game = self
		dispenser.position = point
		add_child(dispenser)
		_waterDispensers.append(dispenser)

func SpawnWater(dispenser):
	if not WaveActive or WaterPickupScene == null or not is_instance_valid(dispenser): return
	var angle = _rng.randf_range(0.0, TAU)
	var distance = _rng.randf_range(minf(WaterDropMinimumDistance, WaterDropMaximumDistance), maxf(WaterDropMinimumDistance, WaterDropMaximumDistance))
	var margin = WallThickness + 25.0
	var destination = dispenser.position + Vector2.RIGHT.rotated(angle) * distance
	destination = Vector2(clampf(destination.x, margin, ArenaSize.x - margin), clampf(destination.y, margin, ArenaSize.y - margin))
	var water = WaterPickupScene.instantiate()
	water.Game = self
	add_child(water)
	water.Launch(dispenser.position, destination)
	_consumables.append(water)

func CollectConsumable(pickup):
	_consumables.erase(pickup)
	if not WaveActive or Mohsen == null: return
	if pickup.Kind == ConsumablePickup.PickupKind.Water: Mohsen.Heal(WaterHealing)
	else: Mohsen.ApplyCoffeeBoost(pickup.CoffeeSpeedBonusPercent, pickup.CoffeeDurationSeconds)
	PlaySound(GameAudio.Sound.CoinPickup)
	UpdateHud()

func OnPlayerHealed(amount):
	if amount <= 0.0 or DamageNumberScene == null: return
	var number = DamageNumberScene.instantiate()
	number.position = Mohsen.position + Vector2(0, -42)
	add_child(number)
	number.SetHealing(amount)

func OnPlayerDamaged(amount):
	if amount <= 0.0: return
	if DamageNumberScene != null:
		var number = DamageNumberScene.instantiate()
		number.position = Mohsen.position + Vector2(0, -42)
		add_child(number)
		number.SetPlayerDamage(amount)
	if not _hudScreen.visible: return
	_hudDamagePunchRemaining = HudDamagePunchSeconds
	_hudDamageEdges.Pulse()

func UpdateHudDamagePunch(dt):
	_hudDamagePunchRemaining = maxf(0.0, _hudDamagePunchRemaining - dt)
	var progress = 0.0 if HudDamagePunchSeconds <= 0.0 else _hudDamagePunchRemaining / HudDamagePunchSeconds
	var punchScale = Vector2.ONE * (1.0 + HudDamagePunchScale * progress * progress)
	if _hudHpBar != null: _hudHpBar.scale = punchScale
	if _hudHpText != null: _hudHpText.scale = punchScale

func UpdateHudCoinBounce(dt):
	_hudCoinBounceRemaining = maxf(0.0, _hudCoinBounceRemaining - dt)
	var progress = 0.0 if HudCoinBounceSeconds <= 0.0 else _hudCoinBounceRemaining / HudCoinBounceSeconds
	var bounceScale = Vector2.ONE * (1.0 + HudCoinBounceScale * progress * progress)
	if _hudCoinText != null: _hudCoinText.scale = bounceScale
	if _hudCoinIcon != null: _hudCoinIcon.scale = bounceScale

func BeginWaveBeat(text, seconds):
	_waveBeatDuration = maxf(0.0, seconds)
	_waveBeatRemaining = _waveBeatDuration
	_hudWaveBeat.text = text
	_hudWaveBeat.visible = seconds > 0.0
	_hudWaveBeat.pivot_offset = Vector2(450, 34)
	_hudWaveBeat.scale = Vector2.ONE * 1.12
	_hudWaveBeat.modulate = Color.WHITE

func UpdateWaveBeat(dt):
	if _waveBeatRemaining <= 0.0: return
	_waveBeatRemaining = maxf(0.0, _waveBeatRemaining - dt)
	var progress = 0.0 if _waveBeatDuration <= 0.0 else _waveBeatRemaining / _waveBeatDuration
	_hudWaveBeat.scale = Vector2.ONE * (1.0 + 0.12 * progress)
	_hudWaveBeat.modulate = Color(1, 1, 1, minf(1.0, progress * 3.0))
	if _waveBeatRemaining <= 0.0: _hudWaveBeat.visible = false

func ClearWaterAndCoffee():
	FreeObjects(_waterDispensers)
	FreeObjects(_consumables)

func SpawnEnemyOffspring(scene, at, healthMultiplier, speedMultiplier, damageMultiplier):
	if not WaveActive or scene == null: return
	var chosenScene = SelectTankScene(scene, Waves[_waveIndex].TankSpawnChance)
	var offspring = chosenScene.instantiate()
	if not offspring is Enemy:
		offspring.free()
		return
	var boundary = WallThickness + 48.0
	offspring.position = Vector2(clampf(at.x, boundary, ArenaSize.x - boundary), clampf(at.y, boundary, ArenaSize.y - boundary))
	add_child(offspring)
	var wave = Waves[_waveIndex]
	offspring.Configure(self, wave.WaveNumber, wave.WaveDangerLevelHealthMultiplier, wave.WaveDangerLevelDamageMultiplier, healthMultiplier, maxf(0.1, speedMultiplier), 0.0, damageMultiplier)
	Enemies.append(offspring)

func CollectCoin(coin):
	_coins.erase(coin)
	_coinsHeld += 1
	_hudCoinBounceRemaining = HudCoinBounceSeconds
	if _audio != null: _audio.PlayCoinCollected()
	UpdateHud()

func PlaySound(cue):
	if _audio != null: _audio.Play(cue)

func RequestKnifeCritPause(): RequestImpactPause(KnifeCritPauseSeconds)
func RequestBossHitPause(): RequestImpactPause(BossHitPauseSeconds)

func RequestImpactPause(seconds):
	if not WaveActive or seconds <= 0.0: return
	if _impactPauseRemaining <= 0.0 and _impactPauseCooldown > 0.0: return
	_impactPauseRemaining = maxf(_impactPauseRemaining, minf(seconds, 0.2))
	_impactPauseCooldown = maxf(_impactPauseCooldown, _impactPauseRemaining + ImpactPauseMinGapSeconds)

func ResetImpactPause():
	_impactPauseRemaining = 0.0
	_impactPauseCooldown = 0.0
	_impactPauseFrame = false

func SpawnDamageNumber(amount, at, criticalHit):
	if DamageNumberScene == null: return
	var number = DamageNumberScene.instantiate()
	number.position = at
	add_child(number)
	number.SetDamage(amount, criticalHit)

func SpawnPlayerBulletHit(at):
	if PlayerBulletHitScene == null: return
	var effect = PlayerBulletHitScene.instantiate()
	effect.position = at
	add_child(effect)

func SpawnEnemyProjectile(from, direction, speed, damage): SpawnHostileProjectile(EnemyBulletScene, from, direction, speed, damage)
func SpawnBossProjectile(from, direction, speed, damage): SpawnHostileProjectile(EnemyBulletScene, from, direction, speed, damage)
func SpawnManyEyesProjectile(scene, from, direction, speed, damage): SpawnHostileProjectile(scene, from, direction, speed, damage)

func SpawnHostileProjectile(scene, from, direction, speed, damage):
	if scene == null: return
	var projectile = scene.instantiate()
	projectile.Game = self
	projectile.position = from
	projectile.Direction = direction
	projectile.Speed = speed
	projectile.RemainingDistance = 3000.0
	projectile.Damage = maxf(0.0, damage)
	add_child(projectile)
	_projectiles.append(projectile)

func FireWeapon(weapon, direction, stats):
	PlaySound([GameAudio.Sound.KnifeShot, GameAudio.Sound.PistolShot, GameAudio.Sound.SmgShot, GameAudio.Sound.ShotgunShot][weapon.Kind])
	if weapon.Kind == WeaponData.Kind.Shotgun:
		FireShotgun(weapon, direction, stats)
		Mohsen.ShakeOnShot(weapon.ShotShakePixels)
		return
	var damage = EffectiveWeaponDamage(weapon.Kind, stats)
	var critical = _rng.randf() < EffectiveCritChance(stats)
	if critical: damage *= stats.CritMultiplier
	var knife = weapon.Kind == WeaponData.Kind.Knife
	var scene = [KnifeAttackScene, PistolBulletScene, SmgBulletScene][weapon.Kind]
	var shot = scene.instantiate()
	shot.Game = self
	shot.SourceScene = scene
	shot.position = weapon.ShotOrigin(direction)
	shot.Direction = direction
	if not knife: shot.Speed = 650.0 if weapon.Kind == WeaponData.Kind.Smg else 580.0
	shot.RemainingDistance = maxf(20.0, stats.Range - weapon.position.length())
	shot.RangeAtSpawn = shot.RemainingDistance
	shot.Damage = damage
	shot.SizeMultiplier = Mohsen.ProjectileSizeMultiplier
	shot.CriticalHit = critical
	shot.Knockback = stats.Knockback + Mohsen.Knockback
	if not knife:
		shot.PierceCapacity = Mohsen.BulletPierce
		shot.RemainingPierces = Mohsen.BulletPierce
	add_child(shot)
	_projectiles.append(shot)
	Mohsen.ShakeOnShot(weapon.ShotShakePixels)

func FireShotgun(weapon, direction, stats):
	var profile = WeaponData.Shotgun(weapon.Tier)
	var pelletCount = maxi(1, profile.PelletCount)
	var origin = weapon.ShotOrigin(direction)
	var travelRange = maxf(1.0, stats.Range - origin.distance_to(Mohsen.global_position))
	for i in range(pelletCount):
		var angle = 0.0 if pelletCount == 1 else deg_to_rad(profile.SpreadDegrees) * (float(i) / (pelletCount - 1) - 0.5)
		var pellet = ShotgunPelletScene.instantiate()
		pellet.Game = self
		pellet.SourceScene = ShotgunPelletScene
		pellet.position = origin
		pellet.Direction = direction.rotated(angle)
		pellet.Speed = maxf(1.0, profile.PelletSpeed)
		pellet.LifetimeSeconds = maxf(0.01, profile.PelletLifetimeSeconds)
		pellet.RemainingDistance = travelRange
		pellet.RangeAtSpawn = travelRange
		pellet.Damage = EffectiveWeaponDamage(WeaponData.Kind.Shotgun, stats)
		pellet.SizeMultiplier = Mohsen.ProjectileSizeMultiplier
		pellet.CriticalHit = _rng.randf() < EffectiveCritChance(stats)
		if pellet.CriticalHit: pellet.Damage *= stats.CritMultiplier
		pellet.Knockback = stats.Knockback + Mohsen.Knockback
		pellet.PierceCapacity = Mohsen.BulletPierce
		pellet.RemainingPierces = Mohsen.BulletPierce
		add_child(pellet)
		_projectiles.append(pellet)

func SpawnSpreadBullets(source, at, originalTargetId):
	if not WaveActive or source.SourceScene == null or source.Hostile or source.Knife or not source.CanSpread: return
	var count = maxi(0, Mohsen.Spread)
	for i in range(count):
		var child = source.SourceScene.instantiate()
		child.Game = self
		child.SourceScene = source.SourceScene
		child.CanSpread = false
		child.position = at
		child.Direction = source.Direction.rotated(TAU * i / count)
		for property in ["Hostile", "Knife", "Speed", "LifetimeSeconds", "HitRadius", "SizeMultiplier", "EaseSpeed", "EndSpeedMultiplier", "EaseExponent", "TrailLifetimeSeconds", "Damage", "CriticalHit", "Knockback", "PierceCapacity"]:
			child.set(property, source.get(property))
		child.RemainingDistance = source.RangeAtSpawn
		child.RangeAtSpawn = source.RangeAtSpawn
		child.RemainingPierces = source.PierceCapacity
		child.IgnoreEnemyForLife(originalTargetId)
		add_child(child)
		_projectiles.append(child)

func AddWeapon(kind, tier):
	var scene = [KnifeScene, PistolScene, SmgScene, ShotgunScene][kind]
	var weapon = scene.instantiate()
	weapon.Tier = tier
	Mohsen.WeaponMount.add_child(weapon)
	_weapons.append(weapon)
	RefreshWeaponSlots()
	return weapon

func RefreshWeaponSlots():
	for i in range(_weapons.size()): _weapons[i].Equip(self, Mohsen, i)

func MatchingWeapon(kind, tier, excluded = null):
	for weapon in _weapons:
		if weapon != excluded and weapon.Kind == kind and weapon.Tier == tier and tier < 4: return weapon
	return null

func CanBuyWeapon(kind, tier): return MatchingWeapon(kind, tier) != null or _weapons.size() < 4

# Return both outputs explicitly; GDScript has no C# out parameters.
func GiveWeapon(kind, tier):
	var matching = MatchingWeapon(kind, tier)
	if matching == null: return {"weapon": AddWeapon(kind, tier), "merged": false}
	matching.Tier += 1
	while matching.Tier < 4:
		var next = MatchingWeapon(kind, matching.Tier, matching)
		if next == null: break
		_weapons.erase(next)
		if is_instance_valid(next): next.queue_free()
		matching.Tier += 1
	RefreshWeaponSlots()
	return {"weapon": matching, "merged": true}
func GenerateOffers():
	for i in range(4):
		if not _locks[i]: _offers[i] = RandomOffer()

func DebugProductManagerReroll():
	_productManagerDebugOffers = true
	_locks.fill(false)
	for i in range(4): _offers[i] = RandomProductManagerOffer()
	SetShopMessage("No eligible Product Manager items." if _offers.all(func(offer): return offer == null) else "Debug: Product Manager items only.")
	ShowShop()

func ItemOffer(item): return {"Item": item, "Kind": WeaponData.Kind.Knife, "Tier": 0, "Price": item.Price}

func RandomProductManagerOffer():
	var eligible = ItemPool.filter(func(item): return IsEligibleItem(item) and item.Description.strip_edges().to_lower() == "product manager.")
	if eligible.is_empty(): return null
	return ItemOffer(WeightedItem(eligible))

func RandomOfferForCurrentShop(): return RandomProductManagerOffer() if _productManagerDebugOffers else RandomOffer()

func RandomOffer():
	var eligibleItems = ItemPool.filter(IsEligibleItem)
	var maxTier = mini(4, 1 + int(_waveIndex / 2.0))
	var eligibleWeapons = []
	for kind in WeaponData.Kind.values():
		for tier in range(1, maxTier + 1):
			if CanBuyWeapon(kind, tier): eligibleWeapons.append({"kind": kind, "tier": tier})
	var chooseItem = not eligibleItems.is_empty() and (eligibleWeapons.is_empty() or _rng.randf() >= 0.4)
	if chooseItem: return ItemOffer(WeightedItem(eligibleItems))
	if eligibleWeapons.is_empty(): return null
	var weapon = eligibleWeapons[_rng.randi_range(0, eligibleWeapons.size() - 1)]
	return {"Item": null, "Kind": weapon.kind, "Tier": weapon.tier, "Price": TierOneWeaponPrice + (weapon.tier - 1) * WeaponTierPriceStep}

func ItemWeight(item):
	var completedWave = clampi(_waveIndex + 1, 1, 20)
	match clampi(item.Rarity, 1, 3):
		1: return 12.0
		2: return 4.0 + 0.25 * (completedWave - 1)
		_: return 1.0 + 0.25 * (completedWave - 1)

func WeightedItem(items):
	var total = 0.0
	for item in items: total += ItemWeight(item)
	var roll = _rng.randf() * total
	for item in items:
		roll -= ItemWeight(item)
		if roll < 0.0: return item
	return items[-1]

func IsEligibleItem(item): return item != null and (not item.Unique or not _purchasedItems.has(item))

func IsSafeHealthOffer(item):
	if item == null: return false
	var healthChange = 0.0
	for modifier in item.Modifiers:
		if modifier != null and modifier.Stat == StatModifier.PlayerStat.MaxHealth: healthChange += modifier.Amount
	return Mohsen.MaxHealth + healthChange > 0.0

func ReplaceIneligibleOffers():
	for i in range(4):
		if _offers[i] == null or _offers[i].Item == null or IsEligibleItem(_offers[i].Item): continue
		_locks[i] = false
		_offers[i] = RandomOfferForCurrentShop()

func ShowShop():
	ReplaceIneligibleOffers()
	ShowScreen(ShopScene)
	_screen.get_node("Title").text = ShopTitleText()
	_screen.get_node("Coins").text = ShopCoinsText()
	_screen.get_node("StatsPanel/Box/Stats").text = ShopStatsText()
	_screen.get_node("SpeechBubble").ShowMessage(_message, _lastShownMessageVersion != _messageVersion)
	_lastShownMessageVersion = _messageVersion
	var rows = _screen.get_node("Offers")
	for i in range(4):
		var offer = _offers[i]
		var row = ShopOfferScene.instantiate()
		rows.add_child(row)
		var icon = row.get_node("Content/Header/Icon")
		var title = row.get_node("Content/Header/Heading/Name")
		var kind = row.get_node("Content/Header/Heading/Kind")
		var description = row.get_node("Content/Description")
		var buy = row.get_node("Content/BuyRow/Buy")
		var buyCoin = row.get_node("Content/BuyRow/CoinIcon")
		var lockButton = row.get_node("Content/Lock")
		if offer == null:
			icon.visible = false
			title.text = "Empty"
			kind.text = "Slot %d" % (i + 1)
			description.text = "Empty. Reroll or continue to refill this slot."
			buy.visible = false
			buyCoin.visible = false
			lockButton.visible = false
		else:
			if offer.Item != null: ApplyItemRarityFrame(row, offer.Item.Rarity)
			icon.texture = offer.Item.Icon if offer.Item != null else load(WeaponData.Icon(offer.Kind))
			title.text = OfferTitle(offer)
			kind.text = "%s · Rarity %d%s" % [offer.Item.Description, offer.Item.Rarity, " · Unique" if offer.Item.Unique else ""] if offer.Item != null else "Weapon · Tier " + Roman(offer.Tier)
			description.text = OfferDescription(offer)
			if offer.Item != null:
				var tooltip = ItemTooltip(offer.Item, 1)
				row.tooltip_text = tooltip
				icon.tooltip_text = tooltip
				description.tooltip_text = tooltip
			buy.text = "Buy · %d" % offer.Price
			buy.pressed.connect(Buy.bind(i))
			lockButton.text = "Unlock" if _locks[i] else "Lock"
			lockButton.pressed.connect(ToggleLock.bind(i))
	_screen.get_node("Reroll").text = "Reroll · %d" % (5 * (_rerollCount + 1))
	_screen.get_node("Reroll").pressed.connect(Reroll)
	_screen.get_node("RerollCoinButton").pressed.connect(Reroll)
	_screen.get_node("Next").text = "Finish" if _waveIndex == Waves.size() - 1 else "Start Wave %d" % (_waveIndex + 2)
	_screen.get_node("Next").pressed.connect(StartNextWave)
	PopulateShopInventory()

func ToggleLock(slot):
	_locks[slot] = not _locks[slot]
	ShowShop()

func StartNextWave():
	_waveIndex += 1
	StartWave()

func OfferTitle(offer): return offer.Item.DisplayName if offer.Item != null else WeaponData.Name(offer.Kind) + " " + Roman(offer.Tier)
func ShopCoinsText(): return "Coins: %d" % _coinsHeld

func ShopTitleText():
	var next = Waves[_waveIndex + 1] if _waveIndex + 1 < Waves.size() else null
	if next == null: return "Shop — Last Wave"
	var title = next.WaveName.strip_edges()
	return "Shop — Wave-%d" % next.WaveNumber + ("" if title.is_empty() else "-" + title)

func ItemCount():
	var count = 0
	for value in _purchasedItems.values(): count += value
	return count

# Trim optional decimal places like the original C# 0.# / 0.## formatting.
static func Number(value, decimals = 1):
	var text = ("%." + str(decimals) + "f") % value
	if decimals > 0:
		while text.ends_with("0"): text = text.left(-1)
		if text.ends_with("."): text = text.left(-1)
	return text

static func Signed(value): return ("+" if value > 0.0 else "") + Number(value, 2)

func ShopStatsText():
	return "\n".join([
		"HP: %d/%d" % [ceili(Mohsen.Health), ceili(Mohsen.MaxHealth)],
		"Max HP: " + Number(Mohsen.MaxHealth),
		"HP regeneration: " + Number(Mohsen.HealthRegenPerSecond, 2) + "/s",
		"Movement speed: " + Number(Mohsen.EffectiveMovementSpeed),
		"Knockback: " + Number(Mohsen.Knockback),
		"Bonus damage: " + Number(Mohsen.BonusDamagePercent) + "%",
		"Critical chance bonus: " + Signed(Mohsen.CriticalChancePercent) + "%",
		"Size: %.0f%% · %s" % [Mohsen.ProjectileSizeMultiplier * 100.0, "No moving shots" if Mohsen.CannotShootWhileMoving else "Moving shots OK"],
		"Ranged damage: " + Number(Mohsen.RangedDamagePercent) + "%",
		"Melee damage: " + Number(Mohsen.MeleeDamagePercent) + "%",
		"Pickup radius: " + Number(Mohsen.CoinPickupRadius),
		"Spread: +%d" % Mohsen.Spread,
		"Bullet pierce: +%d" % Mohsen.BulletPierce,
		"Weapons: %d/4" % _weapons.size(),
		"Bought items: %d" % ItemCount()])

func OfferDescription(offer):
	if offer.Item != null:
		var lines = []
		for modifier in offer.Item.Modifiers:
			if modifier != null: lines.append(ColoredModifierSummary(modifier))
		return "\n".join(lines)
	var stats = WeaponData.Get(offer.Kind, offer.Tier)
	if offer.Kind == WeaponData.Kind.Shotgun:
		var profile = WeaponData.Shotgun(offer.Tier)
		return "\n".join([
			"Damage: " + Number(EffectiveWeaponDamage(offer.Kind, stats)) + " per pellet",
			"Pellets: %d · Spread: %.0f°" % [profile.PelletCount, profile.SpreadDegrees],
			"Attack speed: " + Number(1.0 / stats.Interval, 2) + "/s",
			"Range: %.0f px" % stats.Range,
			"Speed: %.0f px/s · Life: %ss" % [profile.PelletSpeed, Number(profile.PelletLifetimeSeconds, 2)],
			"Knockback: %s · Crit: %.0f%%" % [Number(stats.Knockback + Mohsen.Knockback), EffectiveCritChance(stats) * 100.0],
			"Each pellet can hit %d enemies" % (Mohsen.BulletPierce + 1) if Mohsen.BulletPierce > 0 else "Each pellet stops on hit"])
	var behavior = "Piercing throw" if offer.Kind == WeaponData.Kind.Knife else ("Bullet can hit %d enemies" % (Mohsen.BulletPierce + 1) if Mohsen.BulletPierce > 0 else "Bullet stops on hit")
	return "\n".join([
		"Damage: " + Number(EffectiveWeaponDamage(offer.Kind, stats)),
		"Attack speed: " + Number(1.0 / stats.Interval, 2) + "/s",
		"Range: %.0f px" % stats.Range,
		"Knockback: " + Number(stats.Knockback + Mohsen.Knockback),
		"Crit: %.0f%% · ×%s" % [EffectiveCritChance(stats) * 100.0, Number(stats.CritMultiplier)], behavior])

static func StatName(stat):
	return ["Max HP", "movement speed", "knockback", "all damage", "ranged damage", "melee damage", "HP regeneration/s", "Spread bullets on hit", "bullet pierce", "critical chance", "bigger projectiles", "BlockFiringWhileMoving"][stat]

static func IsPercentStat(stat):
	return stat in [StatModifier.PlayerStat.BonusDamagePercent, StatModifier.PlayerStat.RangedDamagePercent, StatModifier.PlayerStat.MeleeDamagePercent, StatModifier.PlayerStat.CriticalChancePercent, StatModifier.PlayerStat.ProjectileSizePercent]

static func ModifierSummary(modifier):
	return Signed(modifier.Amount) + ("%" if IsPercentStat(modifier.Stat) else "") + " " + StatName(modifier.Stat)

static func ColoredModifierSummary(modifier):
	if modifier.Stat == StatModifier.PlayerStat.BlockFiringWhileMoving: return "[color=#ff7676]You cant shoot while moving.[/color]"
	var tint = "#76df80" if modifier.Amount > 0.0 else ("#ff7676" if modifier.Amount < 0.0 else "#dddddd")
	return "[color=%s]%s[/color]" % [tint, ModifierSummary(modifier)]

func PopulateShopInventory():
	var equipped = _screen.get_node("WeaponsScroll/WeaponsGrid")
	for weapon in _weapons:
		var entry = ShopInventoryEntryScene.instantiate()
		equipped.add_child(entry)
		entry.get_node("Icon").texture = load(WeaponData.Icon(weapon.Kind))
		entry.get_node("Count").text = Roman(weapon.Tier)
		entry.tooltip_text = WeaponTooltip(weapon)
	var bought = _screen.get_node("ItemsScroll/ItemsGrid")
	for item in _purchasedItems:
		var count = _purchasedItems[item]
		var entry = ShopInventoryEntryScene.instantiate()
		bought.add_child(entry)
		ApplyItemRarityFrame(entry, item.Rarity)
		entry.get_node("Icon").texture = item.Icon
		entry.get_node("Count").text = "×%d" % count if count > 1 else ""
		entry.tooltip_text = ItemTooltip(item, count)
	if _purchasedItems.is_empty():
		var empty = Label.new()
		empty.text = "No items bought yet"
		bought.add_child(empty)
	_screen.get_node("WeaponsTitle").text = "Weapons (%d/4)" % _weapons.size()
	_screen.get_node("ItemsTitle").text = "Items (%d)" % ItemCount()

static func ApplyItemRarityFrame(frame, rarity):
	if rarity < 2 or rarity > 3: return
	var original = frame.get_theme_stylebox("panel")
	if not original is StyleBoxFlat: return
	var tinted = original.duplicate()
	tinted.bg_color = Color("#173b55") if rarity == 2 else Color("#38244f")
	tinted.border_color = Color("#5c9fc4") if rarity == 2 else Color("#aa83d2")
	tinted.border_width_left = 3
	tinted.border_width_top = 3
	tinted.border_width_right = 3
	tinted.border_width_bottom = 3
	frame.add_theme_stylebox_override("panel", tinted)

func EffectiveWeaponDamage(kind, stats):
	var typedBonus = Mohsen.MeleeDamagePercent if kind == WeaponData.Kind.Knife else Mohsen.RangedDamagePercent
	return stats.Damage * (1.0 + (Mohsen.BonusDamagePercent + stats.Scaling * typedBonus) / 100.0)

func EffectiveCritChance(stats): return clampf(stats.CritChance + Mohsen.CriticalChancePercent / 100.0, 0.0, 1.0)

func WeaponTooltip(weapon):
	var stats = WeaponData.Get(weapon.Kind, weapon.Tier)
	if weapon.Kind == WeaponData.Kind.Shotgun:
		var profile = WeaponData.Shotgun(weapon.Tier)
		return "\n".join([
			"Shotgun " + Roman(weapon.Tier),
			"Damage: %s per pellet (base %s)" % [Number(EffectiveWeaponDamage(weapon.Kind, stats)), Number(stats.Damage)],
			"Pellets: %d in %.0f° spread" % [profile.PelletCount, profile.SpreadDegrees],
			"Attack speed: %s/s (%ss between blasts)" % [Number(1.0 / stats.Interval, 2), Number(stats.Interval, 2)],
			"Range: %.0f px · Pellet speed: %.0f px/s · Life: %ss" % [stats.Range, profile.PelletSpeed, Number(profile.PelletLifetimeSeconds, 2)],
			"Knockback: %s per pellet · Crit: %.0f%% ×%s" % [Number(stats.Knockback + Mohsen.Knockback), EffectiveCritChance(stats) * 100.0, Number(stats.CritMultiplier)],
			"Each pellet can hit %d enemies" % (Mohsen.BulletPierce + 1) if Mohsen.BulletPierce > 0 else "Each pellet stops on its first enemy hit"])
	var behavior = "Thrown knife: hits each enemy once during flight" if weapon.Kind == WeaponData.Kind.Knife else ("Bullet: can hit %d enemies" % (Mohsen.BulletPierce + 1) if Mohsen.BulletPierce > 0 else "Bullet: disappears on the first enemy hit")
	return "\n".join([
		WeaponData.Name(weapon.Kind) + " " + Roman(weapon.Tier),
		"Damage: %s (base %s)" % [Number(EffectiveWeaponDamage(weapon.Kind, stats)), Number(stats.Damage)],
		"Attack speed: %s/s (%ss between attacks)" % [Number(1.0 / stats.Interval, 2), Number(stats.Interval, 2)],
		"Range: %.0f px  •  Knockback: %s" % [stats.Range, Number(stats.Knockback + Mohsen.Knockback)],
		"Critical hit: %.0f%% chance, ×%s damage" % [EffectiveCritChance(stats) * 100.0, Number(stats.CritMultiplier)], behavior])

func ItemTooltip(item, count):
	var lines = ["%s ×%d" % [item.DisplayName, count], "Passive item" if item.Description.strip_edges().is_empty() else item.Description, "Rarity %d%s" % [item.Rarity, " · Unique" if item.Unique else " · Repeatable"]]
	var names = ["Max HP", "Movement speed", "Knockback", "All weapon damage", "Ranged damage", "Melee damage", "HP regeneration / s", "Spread bullets on hit", "Bullet pierce", "Critical chance", "Projectile size", "BlockFiringWhileMoving"]
	for modifier in item.Modifiers:
		if modifier == null: continue
		if modifier.Stat == StatModifier.PlayerStat.BlockFiringWhileMoving:
			lines.append("You cant shoot while moving.")
			continue
		var suffix = "%" if IsPercentStat(modifier.Stat) else ""
		lines.append("%s: %s%s each, %s%s total" % [names[modifier.Stat], Signed(modifier.Amount), suffix, Signed(modifier.Amount * count), suffix])
	lines.append("Current all-damage bonus: " + Number(Mohsen.BonusDamagePercent) + "%")
	lines.append("Current critical chance bonus: " + Signed(Mohsen.CriticalChancePercent) + "%")
	lines.append("Current projectile size: %.0f%%" % (Mohsen.ProjectileSizeMultiplier * 100.0))
	for weapon in _weapons:
		var stats = WeaponData.Get(weapon.Kind, weapon.Tier)
		var damageUnit = " damage/pellet" if weapon.Kind == WeaponData.Kind.Shotgun else " damage"
		lines.append("%s %s: %s%s, %s attacks/s" % [WeaponData.Name(weapon.Kind), Roman(weapon.Tier), Number(EffectiveWeaponDamage(weapon.Kind, stats)), damageUnit, Number(1.0 / stats.Interval, 2)])
	return "\n".join(lines)

func Buy(slot):
	var offer = _offers[slot]
	if offer == null: return
	if offer.Item != null and not IsEligibleItem(offer.Item):
		_locks[slot] = false
		_offers[slot] = RandomOfferForCurrentShop()
		SetShopMessage("That item is no longer eligible.")
		ShowShop()
		return
	if offer.Item != null and not IsSafeHealthOffer(offer.Item):
		SetShopMessage("You can't buy that item. It would drop your max HP to 0 or below.")
		ShowShop()
		return
	if _coinsHeld < offer.Price:
		SetShopMessage("Not enough coins.")
		ShowShop()
		return
	if offer.Item == null and not CanBuyWeapon(offer.Kind, offer.Tier):
		SetShopMessage("Four weapons equipped; this offer cannot merge.")
		ShowShop()
		return
	_coinsHeld -= offer.Price
	PlaySound(GameAudio.Sound.ShopPurchase)
	var upgradedWeapon = null
	var merged = false
	if offer.Item != null:
		for modifier in offer.Item.Modifiers: Mohsen.ApplyModifier(modifier)
		_purchasedItems[offer.Item] = _purchasedItems.get(offer.Item, 0) + 1
	else:
		var result = GiveWeapon(offer.Kind, offer.Tier)
		upgradedWeapon = result.weapon
		merged = result.merged
	_offers[slot] = null
	_locks[slot] = false
	if offer.Item != null: SetShopMessage("Bought %s." % offer.Item.DisplayName)
	elif merged: SetShopMessage("%s upgraded to tier %s!" % [WeaponData.Name(offer.Kind), Roman(upgradedWeapon.Tier)])
	else: SetShopMessage("Weapon equipped.")
	ShowShop()
	if merged: ShowMergeFeedback(slot, upgradedWeapon)

func ShowMergeFeedback(offerSlot, upgradedWeapon):
	var offers = _screen.get_node("Offers")
	if offerSlot < 0 or offerSlot >= offers.get_child_count(): return
	var card = offers.get_child(offerSlot)
	var icon = card.get_node("Content/Header/Icon")
	icon.texture = load(WeaponData.Icon(upgradedWeapon.Kind))
	icon.visible = true
	card.get_node("Content/Header/Heading/Name").text = "TIER UP!"
	card.get_node("Content/Header/Heading/Kind").text = "%s · Tier %s" % [WeaponData.Name(upgradedWeapon.Kind), Roman(upgradedWeapon.Tier)]
	card.get_node("Content/Description").text = "Weapon merged successfully."
	var grid = _screen.get_node("WeaponsScroll/WeaponsGrid")
	var weaponSlot = _weapons.find(upgradedWeapon)
	if weaponSlot < 0 or weaponSlot >= grid.get_child_count(): return
	var entry = grid.get_child(weaponSlot)
	entry.get_node("Count").text = Roman(upgradedWeapon.Tier)
	AnimateTierUp(card, Vector2(112, 195), 1.04)
	AnimateTierUp(entry, Vector2(36, 36), 1.28)

static func AnimateTierUp(control, pivot, peakScale):
	control.pivot_offset = pivot
	control.scale = Vector2.ONE * peakScale
	control.modulate = Color(1, 0.83, 0.36)
	var tween = control.create_tween()
	tween.tween_property(control, "scale", Vector2.ONE, 0.36).set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT)
	tween.parallel().tween_property(control, "modulate", Color.WHITE, 0.48)

func Reroll():
	var cost = 5 * (_rerollCount + 1)
	if _coinsHeld < cost:
		SetShopMessage("Not enough coins to reroll.")
		ShowShop()
		return
	_coinsHeld -= cost
	_rerollCount += 1
	_productManagerDebugOffers = false
	GenerateOffers()
	SetShopMessage("Unlocked slots rerolled.")
	ShowShop()

func SetShopMessage(message):
	_message = message
	_messageVersion += 1

func ShowFinish(victory):
	WaveActive = false
	_hudScreen.visible = false
	ShowScreen(VictoryScene if victory else DefeatScene)
	_screen.get_node("ImageFrame/Content/PlayAgain" if victory else "PlayAgain").pressed.connect(ShowMenu)

static func FreeObjects(objects):
	for object in objects:
		if is_instance_valid(object): object.queue_free()
	objects.clear()

func ClearWaveObjects():
	FreeObjects(Enemies)
	FreeObjects(_coins)
	ClearWaterAndCoffee()
	FreeObjects(_projectiles)
	for pending in _pending:
		for entry in pending.Entries:
			if is_instance_valid(entry.warning): entry.warning.queue_free()
	_pending.clear()

func ClearWorld():
	WaveActive = false
	ClearWaveObjects()
	FreeObjects(_weapons)
	if is_instance_valid(Mohsen): Mohsen.queue_free()
	Mohsen = null

func UpdateHud():
	if Mohsen == null or _waveIndex >= Waves.size(): return
	var wave = Waves[_waveIndex]
	var waveName = wave.WaveName.strip_edges()
	_hudWaveName.text = "Wave-%d" % wave.WaveNumber + ("" if waveName.is_empty() else "-" + waveName)
	var seconds = ceili(maxf(0.0, wave.Duration - _waveClock))
	_hudTimer.text = "%ds" % seconds
	_hudTimer.add_theme_color_override("font_color", Color.RED if seconds < 10 else (Color.YELLOW if seconds < 20 else Color.WHITE))
	_hudHpBar.max_value = maxf(1.0, Mohsen.MaxHealth)
	_hudHpBar.value = clampf(Mohsen.Health, 0.0, Mohsen.MaxHealth)
	_hudHpText.text = "%d/%d" % [ceili(Mohsen.Health), ceili(Mohsen.MaxHealth)]
	_hudCoinText.text = "Coins: %d" % _coinsHeld
	var equipment = []
	for weapon in _weapons: equipment.append(WeaponData.Name(weapon.Kind) + " " + Roman(weapon.Tier))
	_hudEquipment.text = "Weapons: " + ", ".join(equipment) + "\nDamage bonus: %.0f%%" % Mohsen.BonusDamagePercent

func ClearScreen():
	if not is_instance_valid(_screen): return
	_canvas.remove_child(_screen)
	_screen.queue_free()
	_screen = null

func ShowScreen(scene):
	ClearScreen()
	_screen = scene.instantiate()
	_canvas.add_child(_screen)

static func Roman(tier): return ["I", "II", "III", "IV"][clampi(tier, 1, 4) - 1]
