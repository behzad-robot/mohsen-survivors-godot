using Godot;

[GlobalClass]
public partial class ShopItemDefinition : Resource
{
    [Export] public string DisplayName { get; set; } = "Passive item";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public int Price { get; set; } = 9;
    [Export(PropertyHint.Range, "1,3,1")] public int Rarity { get; set; } = 1;
    [Export] public bool Unique { get; set; }
    [Export] public Godot.Collections.Array<StatModifier> Modifiers { get; set; } = new();
}
