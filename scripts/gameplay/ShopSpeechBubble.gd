class_name ShopSpeechBubble
extends PanelContainer

@export_range(0.1, 0.8, 0.01) var MessagePopSeconds: float = 0.32

var _text
var _popAge = 0.0

func _ready():
	_text = get_node("Text")
	set_process(false)

func ShowMessage(message, animate):
	visible = not message.is_empty()
	_text.text = message
	scale = Vector2.ONE
	modulate = Color.WHITE
	set_process(false)
	if not visible or not animate: return
	pivot_offset = size * 0.5
	_popAge = 0.0
	scale = Vector2.ONE * 0.84
	modulate = Color(1, 1, 1, 0)
	set_process(true)

func _process(dt):
	_popAge += dt
	var progress = clampf(_popAge / maxf(0.01, MessagePopSeconds), 0.0, 1.0)
	var pop = lerpf(0.84, 1.07, 1.0 - pow(1.0 - progress / 0.65, 2.0)) if progress < 0.65 else lerpf(1.07, 1.0, (progress - 0.65) / 0.35)
	scale = Vector2.ONE * pop
	modulate = Color(1, 1, 1, minf(1.0, progress * 4.0))
	if progress < 1.0: return
	scale = Vector2.ONE
	modulate = Color.WHITE
	set_process(false)
