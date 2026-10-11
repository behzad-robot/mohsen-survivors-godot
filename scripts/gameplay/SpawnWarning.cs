using Godot;

public partial class SpawnWarning : Sprite2D
{
    [Export(PropertyHint.Range, "16,64,1")] public float InitialRingRadius { get; set; } = 35f;
    [Export(PropertyHint.Range, "4,32,1")] public float FinalRingRadius { get; set; } = 13f;
    [Export(PropertyHint.Range, "1,6,0.5")] public float RingWidth { get; set; } = 2.5f;
    [Export(PropertyHint.Range, "0,8,1")] public float PulseCount { get; set; } = 3f;

    private float _duration = 1f;
    private float _progress;

    public void SetTiming(float duration, float remaining)
    {
        _duration = Mathf.Max(0.001f, duration);
        _progress = Mathf.Clamp(1f - remaining / _duration, 0f, 1f);
        float pulse = 0.5f + 0.5f * Mathf.Sin(_progress * PulseCount * Mathf.Tau);
        Modulate = new Color(1f, 1f, 1f, 0.68f + 0.22f * pulse);
        QueueRedraw();
    }

    public override void _Draw()
    {
        float radius = Mathf.Lerp(InitialRingRadius, FinalRingRadius, _progress);
        DrawArc(Vector2.Zero, radius, 0f, Mathf.Tau, 32, new Color(1f, 0.14f, 0.14f, 0.85f), RingWidth, true);
    }
}
