using Godot;

[Tool]
public partial class Arena : Node2D
{
    [Export] public Vector2 Size { get; set; } = new(1800, 1100);
    [Export] public Texture2D FloorTexture { get; set; }
    [Export] public Texture2D WallAtlas { get; set; }
    [Export] public int WallCellSize { get; set; } = 96;
    [Export] public float WallThickness { get; set; } = 36f;

    public override void _Draw()
    {
        DrawFloor();
        DrawWalls();
    }

    private void DrawFloor()
    {
        if (FloorTexture == null)
        {
            DrawRect(new Rect2(Vector2.Zero, Size), new Color("#cbc9bb"));
            return;
        }

        float tileWidth = FloorTexture.GetWidth();
        float tileHeight = FloorTexture.GetHeight();
        if (tileWidth <= 0f || tileHeight <= 0f) return;
        for (float y = 0f; y < Size.Y; y += tileHeight)
        for (float x = 0f; x < Size.X; x += tileWidth)
        {
            float width = Mathf.Min(tileWidth, Size.X - x);
            float height = Mathf.Min(tileHeight, Size.Y - y);
            DrawTextureRectRegion(FloorTexture, new Rect2(x, y, width, height), new Rect2(0f, 0f, width, height));
        }
    }

    private void DrawWalls()
    {
        if (WallAtlas == null || WallCellSize <= 0 || WallAtlas.GetWidth() < WallCellSize * 3 || WallAtlas.GetHeight() < WallCellSize * 3)
        {
            DrawFallbackWalls();
            return;
        }
        float cell = WallCellSize;
        float right = Size.X - cell;
        float bottom = Size.Y - cell;
        if (right < cell || bottom < cell) return;

        DrawWallCell(0, 0, 0f, 0f, cell, cell);
        DrawWallCell(2, 0, right, 0f, cell, cell);
        DrawWallCell(0, 2, 0f, bottom, cell, cell);
        DrawWallCell(2, 2, right, bottom, cell, cell);

        for (float x = cell; x < right; x += cell)
        {
            float width = Mathf.Min(cell, right - x);
            DrawWallCell(1, 0, x, 0f, width, cell);
            DrawWallCell(1, 2, x, bottom, width, cell);
        }
        for (float y = cell; y < bottom; y += cell)
        {
            float height = Mathf.Min(cell, bottom - y);
            DrawWallCell(0, 1, 0f, y, cell, height);
            DrawWallCell(2, 1, right, y, cell, height);
        }
    }

    private void DrawWallCell(int column, int row, float x, float y, float width, float height)
    {
        float cell = WallCellSize;
        DrawTextureRectRegion(WallAtlas,
            new Rect2(x, y, width, height),
            new Rect2(column * cell, row * cell, width, height));
    }

    private void DrawFallbackWalls()
    {
        Color wall = new("#475363");
        DrawRect(new Rect2(0f, 0f, Size.X, WallThickness), wall);
        DrawRect(new Rect2(0f, Size.Y - WallThickness, Size.X, WallThickness), wall);
        DrawRect(new Rect2(0f, 0f, WallThickness, Size.Y), wall);
        DrawRect(new Rect2(Size.X - WallThickness, 0f, WallThickness, Size.Y), wall);
    }
}
