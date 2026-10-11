using Godot;

public partial class HudDamageEdges : Control
{
    [Export(PropertyHint.Range, "0,0.6,0.01")] public float DurationSeconds { get; set; } = 0.24f;
    [Export(PropertyHint.Range, "0,0.4,0.01")] public float PeakAlpha { get; set; } = 0.18f;
    [Export(PropertyHint.Range, "8,160,1")] public float EdgeWidth { get; set; } = 80f;

    private float _remaining;

    public void Pulse()
    {
        if (DurationSeconds <= 0f || PeakAlpha <= 0f) return;
        _remaining = DurationSeconds;
        QueueRedraw();
    }

    public override void _Process(double delta)
    {
        if (_remaining <= 0f) return;
        _remaining = Mathf.Max(0f, _remaining - (float)delta);
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_remaining <= 0f || DurationSeconds <= 0f || Size.X <= 0f || Size.Y <= 0f) return;
        float intensity = Mathf.Pow(_remaining / DurationSeconds, 2f);
        const int bands = 8;
        float step = Mathf.Min(EdgeWidth, Mathf.Min(Size.X, Size.Y) * 0.35f) / bands;
        for (int i = 0; i < bands; i++)
        {
            float fade = 1f - i / (float)bands;
            Color tint = new(0.9f, 0.05f, 0.05f, PeakAlpha * intensity * fade * fade);
            float inset = i * step;
            DrawRect(new Rect2(inset, inset, Size.X - 2f * inset, step), tint);
            DrawRect(new Rect2(inset, Size.Y - inset - step, Size.X - 2f * inset, step), tint);
            DrawRect(new Rect2(inset, inset + step, step, Size.Y - 2f * inset - 2f * step), tint);
            DrawRect(new Rect2(Size.X - inset - step, inset + step, step, Size.Y - 2f * inset - 2f * step), tint);
        }
    }
}
