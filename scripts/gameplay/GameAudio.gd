class_name GameAudio
extends Node

enum Sound { KnifeShot, PistolShot, SmgShot, ShotgunShot, EnemyHit, EnemyDefeat, CoinPickup, ShopPurchase, WaveStart, MohsenHit, BossWarning }

@export var KnifeShot: AudioStream = null
@export var PistolShot: AudioStream = null
@export var SmgShot: AudioStream = null
@export var ShotgunShot: AudioStream = null
@export var EnemyHit: AudioStream = null
@export var EnemyDefeat: AudioStream = null
@export var CoinPickup: AudioStream = null
@export var ShopPurchase: AudioStream = null
@export var WaveStart: AudioStream = null
@export var MohsenHit: AudioStream = null
@export var BossWarning: AudioStream = null
@export_range(1, 24, 1) var VoiceCount: int = 12
@export_range(-40, 0, 1) var ShotVolumeDb: float = -15
@export_range(-40, 0, 1) var HitVolumeDb: float = -16
@export_range(-40, 0, 1) var DefeatVolumeDb: float = -12
@export_range(-40, 0, 1) var CoinVolumeDb: float = -17
@export_range(-40, 0, 1) var UiVolumeDb: float = -13
@export_range(-40, 0, 1) var WarningVolumeDb: float = -12
@export_range(0, 0.2, 0.01) var PitchVariation: float = 0.04
@export_range(0, 0.3, 0.01) var SmgMinimumInterval: float = 0.08
@export_range(0, 0.3, 0.01) var CoinMinimumInterval: float = 0.07
@export_range(0, 0.3, 0.01) var EnemyHitMinimumInterval: float = 0.05
@export_range(0, 0.3, 0.01) var EnemyDefeatMinimumInterval: float = 0.08
@export_range(0.1, 1, 0.05) var CoinRhythmGapSeconds: float = 0.35
@export_range(0, 0.1, 0.005) var CoinPitchStep: float = 0.025
@export_range(0, 0.3, 0.01) var CoinPitchMaxBonus: float = 0.15

var _voices = []
var _lastPlayed = {}
var _random = RandomNumberGenerator.new()
var _lastCoinCollectedAt = -1000.0
var _coinChain = 0

func _ready():
	_random.randomize()
	for i in range(clampi(VoiceCount, 1, 24)):
		var voice = AudioStreamPlayer.new()
		add_child(voice)
		_voices.append(voice)

func PlayCoinCollected():
	var now = Time.get_ticks_msec() / 1000.0
	_coinChain = mini(_coinChain + 1, 12) if now - _lastCoinCollectedAt <= CoinRhythmGapSeconds else 0
	_lastCoinCollectedAt = now
	Play(Sound.CoinPickup, minf(CoinPitchMaxBonus, _coinChain * CoinPitchStep))

func Play(cue, pitchBonus = 0.0):
	var stream = [KnifeShot, PistolShot, SmgShot, ShotgunShot, EnemyHit, EnemyDefeat, CoinPickup, ShopPurchase, WaveStart, MohsenHit, BossWarning][cue]
	if stream == null: return
	var now = Time.get_ticks_msec() / 1000.0
	var minimumInterval = {Sound.SmgShot: SmgMinimumInterval, Sound.CoinPickup: CoinMinimumInterval, Sound.EnemyHit: EnemyHitMinimumInterval, Sound.EnemyDefeat: EnemyDefeatMinimumInterval}.get(cue, 0.0)
	if _lastPlayed.has(cue) and now - _lastPlayed[cue] < minimumInterval: return
	var voice = null
	for candidate in _voices:
		if not candidate.playing:
			voice = candidate
			break
	if voice == null: return
	var volume = UiVolumeDb
	match cue:
		Sound.KnifeShot, Sound.PistolShot, Sound.SmgShot, Sound.ShotgunShot: volume = ShotVolumeDb
		Sound.EnemyHit, Sound.MohsenHit: volume = HitVolumeDb
		Sound.EnemyDefeat: volume = DefeatVolumeDb
		Sound.CoinPickup: volume = CoinVolumeDb
		Sound.BossWarning: volume = WarningVolumeDb
	_lastPlayed[cue] = now
	voice.stream = stream
	voice.volume_db = volume
	voice.pitch_scale = 1.0 + pitchBonus + _random.randf_range(-PitchVariation, PitchVariation)
	voice.play()
