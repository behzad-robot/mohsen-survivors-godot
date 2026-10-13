@tool
class_name Arena
extends Node2D

@export var Size: Vector2 = Vector2(1800, 1100)
@export var FloorTexture: Texture2D = null
@export var WallAtlas: Texture2D = null
@export var WallCellSize: int = 96
@export var WallThickness: float = 36

func _draw():
	DrawFloor()
	DrawWalls()

func DrawFloor():
	if FloorTexture == null:
		draw_rect(Rect2(Vector2.ZERO, Size), Color("#cbc9bb"))
		return
	var tileWidth = FloorTexture.get_width()
	var tileHeight = FloorTexture.get_height()
	if tileWidth <= 0 or tileHeight <= 0: return
	var y = 0.0
	while y < Size.y:
		var x = 0.0
		while x < Size.x:
			var width = minf(tileWidth, Size.x - x)
			var height = minf(tileHeight, Size.y - y)
			draw_texture_rect_region(FloorTexture, Rect2(x, y, width, height), Rect2(0, 0, width, height))
			x += tileWidth
		y += tileHeight

func DrawWalls():
	if WallAtlas == null or WallCellSize <= 0 or WallAtlas.get_width() < WallCellSize * 3 or WallAtlas.get_height() < WallCellSize * 3:
		DrawFallbackWalls()
		return
	var cell = float(WallCellSize)
	var right = Size.x - cell
	var bottom = Size.y - cell
	if right < cell or bottom < cell: return
	DrawWallCell(0, 0, 0.0, 0.0, cell, cell)
	DrawWallCell(2, 0, right, 0.0, cell, cell)
	DrawWallCell(0, 2, 0.0, bottom, cell, cell)
	DrawWallCell(2, 2, right, bottom, cell, cell)
	var x = cell
	while x < right:
		var width = minf(cell, right - x)
		DrawWallCell(1, 0, x, 0.0, width, cell)
		DrawWallCell(1, 2, x, bottom, width, cell)
		x += cell
	var y = cell
	while y < bottom:
		var height = minf(cell, bottom - y)
		DrawWallCell(0, 1, 0.0, y, cell, height)
		DrawWallCell(2, 1, right, y, cell, height)
		y += cell

func DrawWallCell(column, row, x, y, width, height):
	draw_texture_rect_region(WallAtlas, Rect2(x, y, width, height), Rect2(column * WallCellSize, row * WallCellSize, width, height))

func DrawFallbackWalls():
	var wall = Color("#475363")
	draw_rect(Rect2(0, 0, Size.x, WallThickness), wall)
	draw_rect(Rect2(0, Size.y - WallThickness, Size.x, WallThickness), wall)
	draw_rect(Rect2(0, 0, WallThickness, Size.y), wall)
	draw_rect(Rect2(Size.x - WallThickness, 0, WallThickness, Size.y), wall)
