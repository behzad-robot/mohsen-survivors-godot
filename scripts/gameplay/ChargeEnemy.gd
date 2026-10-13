class_name ChargeEnemy
extends Enemy

@export_range(40, 800, 5) var ChargeRadius: float = 525
@export_range(50, 1200, 5) var ChargeSpeed: float = 450
@export_range(50, 1600, 5) var MaximumChargeSpeed: float = 900
@export_range(0, 5000, 25) var ChargeAcceleration: float = 1800
@export_range(0.1, 3, 0.05) var WarningSeconds: float = 0.9
@export_range(1, 8, 1) var WarningFlashes: int = 3
@export_range(1, 40, 1) var ArrivalTolerance: float = 6
@export_range(0, 10, 0.1) var StopDuration: float = 3

enum ChargeState { Approach, Warn, Charge, Recover }
var _state = ChargeState.Approach
var _targetDestination = Vector2.ZERO
var _chargeDirection = Vector2.ZERO
var _remainingDistance = 0.0
var _recoveryLeft = 0.0
var _warningElapsed = 0.0
var _currentChargeSpeed = 0.0
var _visual

func HitWallMargin(): return 34.0

func _ready():
	super._ready()
	_visual = get_node("Visual")

func _process(dt):
	super._process(dt)
	if Health <= 0.0 or Game == null or not Game.WaveActive or Game.IsImpactPaused: return
	if _state == ChargeState.Warn: _visual.flip_h = _chargeDirection.x < 0.0
	elif _state == ChargeState.Charge:
		_visual.flip_h = _chargeDirection.x < 0.0
		_visual.scale *= Vector2(1.12, 0.9)
	elif _state == ChargeState.Recover: _visual.scale *= Vector2(0.94, 1.06)

func UpdateBehavior(dt, toward, distance):
	match _state:
		ChargeState.Approach:
			if distance <= ChargeRadius and distance > 0.01:
				_targetDestination = Game.Mohsen.position
				_chargeDirection = toward / distance
				_remainingDistance = distance
				_warningElapsed = 0.0
				_state = ChargeState.Warn
				SetWarningFlash(0.0)
			else: MoveWithinArena(toward.normalized() * MovementSpeed * dt, 34.0)
		ChargeState.Warn:
			_warningElapsed += dt
			var progress = clampf(_warningElapsed / maxf(0.01, WarningSeconds), 0.0, 1.0)
			var flashCycle = progress * maxi(1, WarningFlashes)
			SetWarningFlash(0.0 if progress >= 1.0 else sin(PI * (flashCycle - floor(flashCycle))))
			if progress >= 1.0:
				_currentChargeSpeed = maxf(0.0, ChargeSpeed) * ConfiguredSpeedMultiplier
				_state = ChargeState.Charge
		ChargeState.Charge:
			var distanceAlongCharge = (_targetDestination - position).dot(_chargeDirection)
			if distanceAlongCharge <= ArrivalTolerance or _remainingDistance <= ArrivalTolerance:
				FinishCharge()
				return
			_currentChargeSpeed = minf(maxf(ChargeSpeed, MaximumChargeSpeed) * ConfiguredSpeedMultiplier, _currentChargeSpeed + maxf(0.0, ChargeAcceleration) * ConfiguredSpeedMultiplier * dt)
			var step = minf(minf(_currentChargeSpeed * dt, distanceAlongCharge), _remainingDistance)
			var before = position
			MoveWithinArena(_chargeDirection * step, 34.0)
			_remainingDistance = maxf(0.0, _remainingDistance - maxf(0.0, (position - before).dot(_chargeDirection)))
			if _remainingDistance <= ArrivalTolerance or position.distance_to(_targetDestination) <= ArrivalTolerance or position == before: FinishCharge()
		ChargeState.Recover:
			_recoveryLeft -= dt
			if _recoveryLeft <= 0.0: _state = ChargeState.Approach

func FinishCharge():
	position = _targetDestination
	_state = ChargeState.Recover
	_recoveryLeft = maxf(0.0, StopDuration)
	_remainingDistance = 0.0
	SetWarningFlash(0.0)

func OnDefeated(): SetWarningFlash(0.0)
