using Godot;

public enum WeaponKind { Knife, Pistol, Smg, Shotgun }

public readonly record struct WeaponStats(float Damage, float Interval, float Range, float CritChance, float CritMultiplier, float Knockback, float Scaling);

public static class WeaponData
{
    private static readonly ShotgunTierDefinition[] ShotgunProfiles = new ShotgunTierDefinition[4];
    private static readonly bool[] ShotgunLoadAttempted = new bool[4];

    public static void ConfigureShotgunProfiles(Godot.Collections.Array<ShotgunTierDefinition> profiles)
    {
        for (int i = 0; i < ShotgunProfiles.Length; i++)
        {
            ShotgunProfiles[i] = profiles != null && i < profiles.Count ? profiles[i] : null;
            ShotgunLoadAttempted[i] = ShotgunProfiles[i] != null;
        }
    }

    public static ShotgunTierDefinition Shotgun(int tier)
    {
        int index = Mathf.Clamp(tier, 1, 4) - 1;
        if (ShotgunProfiles[index] != null) return ShotgunProfiles[index];
        if (!ShotgunLoadAttempted[index])
        {
            ShotgunLoadAttempted[index] = true;
            string path = $"res://resources/weapons/shotgun_tier_{index + 1:00}.tres";
            ShotgunProfiles[index] = GD.Load<ShotgunTierDefinition>(path);
            if (ShotgunProfiles[index] == null)
                GD.PushError($"Could not load shotgun tier {index + 1} from {path}; using provisional defaults.");
        }
        ShotgunProfiles[index] ??= DefaultShotgunProfile(index);
        return ShotgunProfiles[index];
    }

    private static ShotgunTierDefinition DefaultShotgunProfile(int index) => index switch
    {
        0 => new() { DamagePerPellet = 12f, AttackInterval = 1.5f, TargetRange = 210f, PelletCount = 5, SpreadDegrees = 36f, PelletSpeed = 520f, PelletLifetimeSeconds = 0.34f, CritChance = 0.08f, CritMultiplier = 1.5f, KnockbackPerPellet = 8f, RangedDamageScaling = 1f },
        1 => new() { DamagePerPellet = 16f, AttackInterval = 1.4f, TargetRange = 215f, PelletCount = 6, SpreadDegrees = 38f, PelletSpeed = 535f, PelletLifetimeSeconds = 0.35f, CritChance = 0.10f, CritMultiplier = 1.6f, KnockbackPerPellet = 9f, RangedDamageScaling = 1f },
        2 => new() { DamagePerPellet = 21f, AttackInterval = 1.3f, TargetRange = 220f, PelletCount = 7, SpreadDegrees = 40f, PelletSpeed = 550f, PelletLifetimeSeconds = 0.36f, CritChance = 0.12f, CritMultiplier = 1.7f, KnockbackPerPellet = 10f, RangedDamageScaling = 1f },
        _ => new() { DamagePerPellet = 28f, AttackInterval = 1.2f, TargetRange = 230f, PelletCount = 8, SpreadDegrees = 42f, PelletSpeed = 565f, PelletLifetimeSeconds = 0.38f, CritChance = 0.15f, CritMultiplier = 1.8f, KnockbackPerPellet = 11f, RangedDamageScaling = 1f }
    };

    public static WeaponStats Get(WeaponKind kind, int tier) => kind == WeaponKind.Shotgun
        ? Shotgun(tier).Stats
        : (kind, Mathf.Clamp(tier, 1, 4)) switch
    {
        (WeaponKind.Knife, 1) => new(9, 1.01f, 280, .20f, 2.5f, 22, .80f),
        (WeaponKind.Knife, 2) => new(13, .93f, 280, .30f, 3f, 22, .80f),
        (WeaponKind.Knife, 3) => new(18, .86f, 280, .40f, 3.5f, 22, .80f),
        (WeaponKind.Knife, _) => new(25, .78f, 280, .50f, 4f, 22, .80f),
        (WeaponKind.Pistol, 1) => new(12, 1.20f, 400, .10f, 2f, 6, 1f),
        (WeaponKind.Pistol, 2) => new(20, 1.12f, 400, .15f, 2f, 6, 1f),
        (WeaponKind.Pistol, 3) => new(30, 1.03f, 400, .20f, 2f, 6, 1f),
        (WeaponKind.Pistol, _) => new(50, .87f, 400, .25f, 2f, 6, 1f),
        (WeaponKind.Smg, 1) => new(3, .17f, 400, .01f, 1.5f, 3, .50f),
        (WeaponKind.Smg, 2) => new(4, .17f, 400, .01f, 1.5f, 3, .60f),
        (WeaponKind.Smg, 3) => new(5, .17f, 400, .01f, 1.5f, 3, .70f),
        (WeaponKind.Smg, _) => new(8, .15f, 400, .01f, 1.5f, 3, .80f),
        _ => throw new System.ArgumentOutOfRangeException(nameof(kind))
    };

    public static string Name(WeaponKind kind) => kind switch { WeaponKind.Smg => "SMG", _ => kind.ToString() };
    public static string Icon(WeaponKind kind) => "res://assets/art/approved/weapons/" + kind.ToString().ToLowerInvariant() + ".png";
}
