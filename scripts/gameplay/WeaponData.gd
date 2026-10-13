class_name WeaponData
extends RefCounted

enum Kind { Knife, Pistol, Smg, Shotgun }


static var _shotgunProfiles = [null, null, null, null]
static var _shotgunLoadAttempted = [false, false, false, false]

static func ConfigureShotgunProfiles(profiles):
	for i in range(4):
		_shotgunProfiles[i] = profiles[i] if profiles != null and i < profiles.size() else null
		_shotgunLoadAttempted[i] = _shotgunProfiles[i] != null

static func Shotgun(tier):
	var index = clampi(tier, 1, 4) - 1
	if _shotgunProfiles[index] != null: return _shotgunProfiles[index]
	if not _shotgunLoadAttempted[index]:
		_shotgunLoadAttempted[index] = true
		var path = "res://resources/weapons/shotgun_tier_%02d.tres" % (index + 1)
		_shotgunProfiles[index] = load(path)
		if _shotgunProfiles[index] == null: push_error("Could not load shotgun tier %d from %s; using provisional defaults." % [index + 1, path])
	if _shotgunProfiles[index] == null: _shotgunProfiles[index] = DefaultShotgunProfile(index)
	return _shotgunProfiles[index]

static func DefaultShotgunProfile(index):
	var values = [
		[12.0, 1.5, 210.0, 5, 36.0, 520.0, 0.34, 0.08, 1.5, 8.0],
		[16.0, 1.4, 215.0, 6, 38.0, 535.0, 0.35, 0.10, 1.6, 9.0],
		[21.0, 1.3, 220.0, 7, 40.0, 550.0, 0.36, 0.12, 1.7, 10.0],
		[28.0, 1.2, 230.0, 8, 42.0, 565.0, 0.38, 0.15, 1.8, 11.0]][index]
	var profile = ShotgunTierDefinition.new()
	var properties = ["DamagePerPellet", "AttackInterval", "TargetRange", "PelletCount", "SpreadDegrees", "PelletSpeed", "PelletLifetimeSeconds", "CritChance", "CritMultiplier", "KnockbackPerPellet"]
	for i in range(properties.size()): profile.set(properties[i], values[i])
	profile.RangedDamageScaling = 1.0
	return profile

static func Get(kind, tier):
	if kind == Kind.Shotgun: return Shotgun(tier).Stats
	var table = [
		[[9, 1.01, 280, 0.20, 2.5, 22, 0.80], [13, 0.93, 280, 0.30, 3, 22, 0.80], [18, 0.86, 280, 0.40, 3.5, 22, 0.80], [25, 0.78, 280, 0.50, 4, 22, 0.80]],
		[[12, 1.20, 400, 0.10, 2, 6, 1], [20, 1.12, 400, 0.15, 2, 6, 1], [30, 1.03, 400, 0.20, 2, 6, 1], [50, 0.87, 400, 0.25, 2, 6, 1]],
		[[3, 0.17, 400, 0.01, 1.5, 3, 0.50], [4, 0.17, 400, 0.01, 1.5, 3, 0.60], [5, 0.17, 400, 0.01, 1.5, 3, 0.70], [8, 0.15, 400, 0.01, 1.5, 3, 0.80]]]
	var values = table[kind][clampi(tier, 1, 4) - 1]
	return {"Damage": values[0], "Interval": values[1], "Range": values[2], "CritChance": values[3], "CritMultiplier": values[4], "Knockback": values[5], "Scaling": values[6]}

static func Name(kind): return ["Knife", "Pistol", "SMG", "Shotgun"][kind]
static func Icon(kind): return "res://assets/art/approved/weapons/" + Name(kind).to_lower() + ".png"
