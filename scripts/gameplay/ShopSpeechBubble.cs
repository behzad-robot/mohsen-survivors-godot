using Godot;

public partial class ShopSpeechBubble : PanelContainer
{
    [Export(PropertyHint.Range, "0.1,0.8,0.01")] public float MessagePopSeconds { get; set; } = 0.32f;

    private Label _text;
    private float _popAge;

    public override void _Ready()
    {
        _text = GetNode<Label>("Text");
        SetProcess(false);
    }

    public void ShowMessage(string message, bool animate)
    {
        Visible = !string.IsNullOrEmpty(message);
        _text.Text = message ?? "";
        Scale = Vector2.One;
        Modulate = Colors.White;
        SetProcess(false);
        if (!Visible || !animate) return;

        PivotOffset = Size * 0.5f;
        _popAge = 0f;
        Scale = Vector2.One * 0.84f;
        Modulate = new Color(1f, 1f, 1f, 0f);
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        _popAge += (float)delta;
        float progress = Mathf.Clamp(_popAge / Mathf.Max(0.01f, MessagePopSeconds), 0f, 1f);
        float size = progress < 0.65f
            ? Mathf.Lerp(0.84f, 1.07f, 1f - Mathf.Pow(1f - progress / 0.65f, 2f))
            : Mathf.Lerp(1.07f, 1f, (progress - 0.65f) / 0.35f);
        Scale = Vector2.One * size;
        Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, progress * 4f));
        if (progress < 1f) return;
        Scale = Vector2.One;
        Modulate = Colors.White;
        SetProcess(false);
    }
}
