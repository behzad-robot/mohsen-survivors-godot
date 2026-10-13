class_name HudDamageEdges
extends Control

@export_range(0, 0.6, 0.01) var DurationSeconds: float = 0.24
@export_range(0, 0.4, 0.01) var PeakAlpha: float = 0.18
@export_range(8, 160, 1) var EdgeWidth: float = 80

var _remaining = 0.0

func Pulse():
	if DurationSeconds <= 0.0 or PeakAlpha <= 0.0: return
	_remaining = DurationSeconds
	queue_redraw()

func _process(dt):
	if _remaining <= 0.0: return
	_remaining = maxf(0.0, _remaining - dt)
	queue_redraw()

func _draw():
	if _remaining <= 0.0 or DurationSeconds <= 0.0 or size.x <= 0.0 or size.y <= 0.0: return
	var intensity = pow(_remaining / DurationSeconds, 2.0)
	var bands = 8
	var step = minf(EdgeWidth, minf(size.x, size.y) * 0.35) / bands
	for i in range(bands):
		var fade = 1.0 - float(i) / bands
		var tint = Color(0.9, 0.05, 0.05, PeakAlpha * intensity * fade * fade)
		var inset = i * step
		draw_rect(Rect2(inset, inset, size.x - 2.0 * inset, step), tint)
		draw_rect(Rect2(inset, size.y - inset - step, size.x - 2.0 * inset, step), tint)
		draw_rect(Rect2(inset, inset + step, step, size.y - 2.0 * inset - 2.0 * step), tint)
		draw_rect(Rect2(size.x - inset - step, inset + step, step, size.y - 2.0 * inset - 2.0 * step), tint)
