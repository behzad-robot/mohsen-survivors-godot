using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameController : Node2D
{
    private const string ShopGreeting = "welcome to VX the Greatest function of Snappfood";
    private static readonly string[] DeathMessages = { "CLOSED", "RESOLVED", "PENDING", "CANCELLED", "NOT VALID", "NOT ENOUGH INFO" };
    public Vector2 ArenaSize => _arena.Size;
    public float WallThickness => _arena.WallThickness;
    [Export] public PackedScene PlayerScene { get; set; } = GD.Load<PackedScene>("res://scenes/player/player.tscn");
    [Export] public PackedScene KnifeScene { get; set; } = GD.Load<PackedScene>("res://scenes/weapons/knife.tscn");
    [Export] public PackedScene PistolScene { get; set; } = GD.Load<PackedScene>("res://scenes/weapons/pistol.tscn");
    [Export] public PackedScene SmgScene { get; set; } = GD.Load<PackedScene>("res://scenes/weapons/smg.tscn");
    [Export] public PackedScene ShotgunScene { get; set; } = GD.Load<PackedScene>("res://scenes/weapons/shotgun.tscn");
    [Export] public Godot.Collections.Array<WaveDefinition> Waves { get; set; } = new();
    [Export] public PackedScene WarningScene { get; set; } = GD.Load<PackedScene>("res://scenes/effects/spawn_warning.tscn");
    [Export] public PackedScene CoinScene { get; set; } = GD.Load<PackedScene>("res://scenes/pickups/coin.tscn");
    [Export] public PackedScene WaterDispenserScene { get; set; } = GD.Load<PackedScene>("res://scenes/pickups/water_dispenser.tscn");
    [Export] public PackedScene WaterPickupScene { get; set; } = GD.Load<PackedScene>("res://scenes/pickups/glass_of_water.tscn");
    [Export] public PackedScene CoffeePickupScene { get; set; } = GD.Load<PackedScene>("res://scenes/pickups/coffee_cup.tscn");
    [Export(PropertyHint.Range, "0,1,0.01")] public float CoffeeDropChance { get; set; } = 0.1f;
    [Export(PropertyHint.Range, "1,10,1")] public int MinimumWaterDispensers { get; set; } = 1;
    [Export(PropertyHint.Range, "1,10,1")] public int MaximumWaterDispensers { get; set; } = 3;
    [Export(PropertyHint.Range, "0,300,1")] public float WaterDropMinimumDistance { get; set; } = 45f;
    [Export(PropertyHint.Range, "0,300,1")] public float WaterDropMaximumDistance { get; set; } = 105f;
    [Export(PropertyHint.Range, "0,100,0.5")] public float WaterHealing { get; set; } = 3f;
    [Export] public PackedScene DamageNumberScene { get; set; } = GD.Load<PackedScene>("res://scenes/effects/damage_number.tscn");
    [Export] public PackedScene EnemyDeathTextScene { get; set; } = GD.Load<PackedScene>("res://scenes/effects/enemy_death_text.tscn");
    [Export] public PackedScene PlayerBulletHitScene { get; set; } = GD.Load<PackedScene>("res://scenes/effects/player_bullet_hit.tscn");
    [Export] public PackedScene EnemyExplosionScene { get; set; } = GD.Load<PackedScene>("res://scenes/effects/enemy_explosion.tscn");
    [Export] public PackedScene PistolBulletScene { get; set; } = GD.Load<PackedScene>("res://scenes/projectiles/pistol_bullet.tscn");
    [Export] public PackedScene SmgBulletScene { get; set; } = GD.Load<PackedScene>("res://scenes/projectiles/smg_bullet.tscn");
    [Export] public PackedScene ShotgunPelletScene { get; set; } = GD.Load<PackedScene>("res://scenes/projectiles/shotgun_pellet.tscn");
    [Export] public Godot.Collections.Array<ShotgunTierDefinition> ShotgunTiers { get; set; } = new();
    [Export] public PackedScene EnemyBulletScene { get; set; } = GD.Load<PackedScene>("res://scenes/projectiles/enemy_bullet.tscn");
    [Export] public PackedScene KnifeAttackScene { get; set; } = GD.Load<PackedScene>("res://scenes/projectiles/knife_attack.tscn");
    [Export] public PackedScene BossScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/orange_boss.tscn");
    [Export] public PackedScene MeleeTankScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/melee_tank.tscn");
    [Export] public PackedScene FemaleMeleeTankScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/melee_female_tank.tscn");
    [Export] public PackedScene RangedTankScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/ranged_tank.tscn");
    [Export] public PackedScene ManyEyesTankScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/many_eyes_tank.tscn");
    [Export] public PackedScene OneEyeMinionTankScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/one_eye_minion_tank.tscn");
    [Export] public PackedScene BossTankScene { get; set; } = GD.Load<PackedScene>("res://scenes/enemies/orange_boss_tank.tscn");
    [Export] public Vector2 WaveOneBossOffset { get; set; } = new(0f, -240f);
    [Export] public Godot.Collections.Array<ShopItemDefinition> ItemPool { get; set; } = new();
    [Export] public float MinimumSpawnDistance { get; set; } = 390f;
    [Export(PropertyHint.Range, "0,32,1")] public float CoinDropScatterRadius { get; set; } = 12f;
    [Export] public int TierOneWeaponPrice { get; set; } = 18;
    [Export] public int WeaponTierPriceStep { get; set; } = 12;
    [Export(PropertyHint.Range, "0,0.2,0.005")] public float KnifeCritPauseSeconds { get; set; } = 0.055f;
    [Export(PropertyHint.Range, "0,0.2,0.005")] public float BossHitPauseSeconds { get; set; } = 0.045f;
    [Export(PropertyHint.Range, "0,0.2,0.005")] public float DefeatPauseSeconds { get; set; } = 0.04f;
    [Export(PropertyHint.Range, "0,0.5,0.01")] public float ImpactPauseMinGapSeconds { get; set; } = 0.11f;
    [Export(PropertyHint.Range, "0,0.5,0.01")] public float HudDamagePunchSeconds { get; set; } = 0.22f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float HudDamagePunchScale { get; set; } = 0.12f;
    [Export(PropertyHint.Range, "0,2,0.05")] public float WaveOpeningBeatSeconds { get; set; } = 0.75f;
    [Export(PropertyHint.Range, "0,2,0.05")] public float WaveClearBeatSeconds { get; set; } = 0.65f;
    [Export(PropertyHint.Range, "0,0.5,0.01")] public float HudCoinBounceSeconds { get; set; } = 0.18f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float HudCoinBounceScale { get; set; } = 0.16f;
    [Export] public PackedScene MenuScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/menu.tscn");
    [Export] public PackedScene ChoiceScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/weapon_choice.tscn");
    [Export] public PackedScene ShopScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/shop.tscn");
    [Export] public PackedScene ShopOfferScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/shop_offer.tscn");
    [Export] public PackedScene ShopInventoryEntryScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/shop_inventory_entry.tscn");
    [Export] public PackedScene FinishScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/finish.tscn");
    [Export] public PackedScene DefeatScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/defeat.tscn");
    [Export] public PackedScene HudScene { get; set; } = GD.Load<PackedScene>("res://scenes/ui/hud.tscn");
    [Export] public PackedScene GameAudioScene { get; set; } = GD.Load<PackedScene>("res://scenes/effects/game_audio.tscn");

    public Player Mohsen { get; private set; }
    public bool WaveActive { get; private set; }
    public bool IsImpactPaused => WaveActive && (_impactPauseRemaining > 0f || _impactPauseFrame);
    public readonly List<Enemy> Enemies = new();
    private readonly List<Coin> _coins = new();
    private readonly List<WaterDispenser> _waterDispensers = new();
    private readonly List<ConsumablePickup> _consumables = new();
    private readonly List<Projectile> _projectiles = new();
    private readonly List<Weapon> _weapons = new();
    private readonly Dictionary<ShopItemDefinition, int> _purchasedItems = new();
    private readonly List<PendingSpawn> _pending = new();
    private readonly Offer[] _offers = new Offer[4];
    private readonly bool[] _locks = new bool[4];
    private Arena _arena;
    private CanvasLayer _canvas;
    private Control _screen;
    private Control _hudScreen;
    private GameAudio _audio;
    private ProgressBar _hudHpBar;
    private Label _hudHpText;
    private HudDamageEdges _hudDamageEdges;
    private Label _hudCoinText;
    private TextureRect _hudCoinIcon;
    private Label _hudWaveName;
    private Label _hudTimer;
    private Label _hudWaveBeat;
    private Label _hudEquipment;
    private int _waveIndex;
    private int _coinsHeld;
    private int _pendingCheatCoins;
    private int _rerollCount;
    private float _waveClock;
    private readonly RandomNumberGenerator _rng = new();
    private int _currentBurst;
    private bool _bossOpportunityPending;
    private string _message = "";
    private int _messageVersion;
    private int _lastShownMessageVersion = -1;
    private bool _hasEnteredShop;
    private bool _productManagerDebugOffers;
    private float _finishDelay;
    private float _waveBeatRemaining;
    private float _waveBeatDuration;
    private float _waveClearRemaining;
    private bool _waveClearForceShop;
    private float _impactPauseRemaining;
    private float _hudDamagePunchRemaining;
    private float _hudCoinBounceRemaining;
    private float _impactPauseCooldown;
    private bool _impactPauseFrame;

    private sealed class PendingSpawn
    {
        public float Due;
        public float WarningDuration;
        public readonly List<(PackedScene scene, Vector2 position, float hp, float speed, float resistance, float damage, SpawnWarning warning)> Entries = new();
    }
    private sealed class Offer
    {
        public ShopItemDefinition Item;
        public WeaponKind Kind;
        public int Tier;
        public int Price;
        public string Title => Item != null ? Item.DisplayName : $"{WeaponData.Name(Kind)} {Roman(Tier)}";
    }

    public override void _Ready()
    {
        _arena = GetNode<Arena>("Arena");
        WeaponData.ConfigureShotgunProfiles(ShotgunTiers);
        _rng.Randomize();
        if (GameAudioScene != null)
        {
            _audio = GameAudioScene.Instantiate<GameAudio>();
            AddChild(_audio);
        }
        if (Waves.Count == 0)
            for (int i = 1; i <= 10; i++)
            {
                var wave = GD.Load<WaveDefinition>($"res://resources/waves/wave_{i:00}.tres");
                if (wave != null) Waves.Add(wave);
            }
        if (ItemPool.Count == 0)
        {
            string[] itemPaths = { "pooria", "narges", "behzad", "sara", "milad", "saman", "arsalan", "mohammad-hossein", "amir-hossein", "hojat", "omid", "hossein", "saeed", "arezoo", "mania", "sahar", "sahel", "sepideh", "sahar-2", "maryam", "devops-buddy-1", "devops-buddy-2", "devops-buddy-3", "hookah", "propranolol_pills", "heavy_smoker", "strawberry_milkshake", "bluetooth_headset" };
            foreach (string path in itemPaths)
            {
                var item = GD.Load<ShopItemDefinition>($"res://resources/items/{path}.tres");
                if (item != null) ItemPool.Add(item);
            }
        }
        _canvas = new CanvasLayer();
        AddChild(_canvas);
        _hudScreen = HudScene.Instantiate<Control>();
        _canvas.AddChild(_hudScreen);
        _hudHpBar = _hudScreen.GetNode<ProgressBar>("HpBar");
        _hudHpText = _hudScreen.GetNode<Label>("HpText");
        _hudDamageEdges = _hudScreen.GetNode<HudDamageEdges>("DamageEdges");
        _hudCoinText = _hudScreen.GetNode<Label>("CoinText");
        _hudCoinIcon = _hudScreen.GetNode<TextureRect>("CoinIcon");
        _hudWaveName = _hudScreen.GetNode<Label>("WaveName");
        _hudTimer = _hudScreen.GetNode<Label>("Timer");
        _hudWaveBeat = _hudScreen.GetNode<Label>("WaveBeat");
        _hudEquipment = _hudScreen.GetNode<Label>("Equipment");
        _hudScreen.Visible = false;
        ShowMenu();
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        UpdateHudDamagePunch(dt);
        UpdateHudCoinBounce(dt);
        UpdateWaveBeat(dt);
        if (_waveClearRemaining > 0f)
        {
            _waveClearRemaining = Mathf.Max(0f, _waveClearRemaining - dt);
            if (_waveClearRemaining <= 0f) CompleteWaveTransition();
            return;
        }
        if (_finishDelay > 0f)
        {
            _finishDelay -= dt;
            if (_finishDelay <= 0f) ShowFinish(false);
        }
        _impactPauseCooldown = Mathf.Max(0f, _impactPauseCooldown - dt);
        if (_impactPauseRemaining > 0f)
        {
            _impactPauseRemaining = Mathf.Max(0f, _impactPauseRemaining - dt);
            _impactPauseFrame = true;
            return;
        }
        _impactPauseFrame = false;
        if (!WaveActive) return;
        _waveClock += dt;
        WaveDefinition wave = Waves[_waveIndex];
        while (_currentBurst < wave.Bursts.Count && wave.Bursts[_currentBurst].TriggerTime <= _waveClock)
            BeginBurst(wave.Bursts[_currentBurst++]);
        if (_bossOpportunityPending && _waveClock >= wave.BossSpawnTime)
        {
            _bossOpportunityPending = false;
            BeginBossOpportunity(wave);
        }
        for (int i = _pending.Count - 1; i >= 0; i--)
        {
            foreach (var entry in _pending[i].Entries)
                if (GodotObject.IsInstanceValid(entry.warning))
                    entry.warning.SetTiming(_pending[i].WarningDuration, _pending[i].Due - _waveClock);
            if (_pending[i].Due <= _waveClock)
            {
                SpawnPending(_pending[i]);
                _pending.RemoveAt(i);
            }
        }
        Enemies.RemoveAll(e => !GodotObject.IsInstanceValid(e));
        foreach (var weapon in _weapons) weapon.Tick(dt);
        UpdateHud();
        if (_waveClock >= wave.Duration) EndWave();
    }

    public override void _Input(InputEvent inputEvent)
    {
        // The editor feature tag is present when playing from Godot's editor, but absent from exports.
        if (!OS.HasFeature("editor") || inputEvent is not InputEventKey key || !key.Pressed || key.Echo) return;
        bool inShop = !WaveActive && GodotObject.IsInstanceValid(_screen) && _screen.HasNode("Offers");

        switch (key.Keycode)
        {
            case Key.C:
                _coinsHeld += 100;
                if (!WaveActive && !inShop) _pendingCheatCoins += 100;
                _hudCoinText.Text = $"Coins: {_coinsHeld}";
                if (inShop)
                    _screen.GetNode<Label>("Coins").Text = ShopCoinsText();
                GetViewport().SetInputAsHandled();
                break;
            case Key.P when inShop:
                DebugProductManagerReroll();
                GetViewport().SetInputAsHandled();
                break;
            case Key.O when WaveActive:
                EndWave(forceShop: true);
                GetViewport().SetInputAsHandled();
                break;
            case Key.R:
                GetViewport().SetInputAsHandled();
                GetTree().ReloadCurrentScene();
                break;
        }
    }

    private void ShowMenu()
    {
        WaveActive = false;
        _hudScreen.Visible = false;
        ShowScreen(MenuScene);
        _screen.GetNode<Button>("Play").Pressed += ShowStartingChoice;
    }

    private void ShowStartingChoice()
    {
        ShowScreen(ChoiceScene);
        _screen.GetNode<Button>("Panel/Box/Knife").Pressed += () => StartRun(WeaponKind.Knife);
        _screen.GetNode<Button>("Panel/Box/Pistol").Pressed += () => StartRun(WeaponKind.Pistol);
        _screen.GetNode<Button>("Panel/Box/Smg").Pressed += () => StartRun(WeaponKind.Smg);
        _screen.GetNode<Button>("Panel/Box/Shotgun").Pressed += () => StartRun(WeaponKind.Shotgun);
    }

    private void StartRun(WeaponKind startingWeapon)
    {
        ClearWorld();
        ResetImpactPause();
        Array.Clear(_offers, 0, _offers.Length);
        Array.Clear(_locks, 0, _locks.Length);
        _purchasedItems.Clear();
        _waveIndex = 0;
        _coinsHeld = _pendingCheatCoins;
        _pendingCheatCoins = 0;
        _rerollCount = 0;
        _message = "";
        _messageVersion = 0;
        _lastShownMessageVersion = -1;
        _hasEnteredShop = false;
        _productManagerDebugOffers = false;
        Mohsen = PlayerScene.Instantiate<Player>();
        Mohsen.Position = ArenaSize / 2f;
        Mohsen.Game = this;
        AddChild(Mohsen);
        Mohsen.Died += OnPlayerDied;
        Mohsen.Healed += OnPlayerHealed;
        Mohsen.Damaged += OnPlayerDamaged;
        AddWeapon(startingWeapon, 1);
        StartWave();
    }

    private void StartWave()
    {
        if (_waveIndex >= Waves.Count) { ShowFinish(true); return; }
        _waveClearRemaining = 0f;
        ClearScreen();
        foreach (var coin in _coins) if (GodotObject.IsInstanceValid(coin)) coin.QueueFree();
        _coins.Clear();
        ClearWaterAndCoffee();
        Mohsen.ClearTemporaryEffects();
        _waveClock = 0f;
        _currentBurst = 0;
        WaveDefinition wave = Waves[_waveIndex];
        _bossOpportunityPending = wave.WaveNumber >= 3 && _rng.Randf() < Mathf.Clamp(wave.BossSpawnChance, 0f, 1f);
        _pending.Clear();
        _finishDelay = 0f;
        ResetImpactPause();
        Mohsen.Refill();
        Mohsen.Active = true;
        foreach (var weapon in _weapons) weapon.ResetTransientEffects();
        WaveActive = true;
        if (wave.WaveNumber > 1) SpawnWaterDispensers();
        PlaySound(GameSound.WaveStart);
        if (_waveIndex == 0) SpawnWaveOneBoss();
        _hudScreen.Visible = true;
        UpdateHud();
        BeginWaveBeat($"WAVE {wave.WaveNumber} START", WaveOpeningBeatSeconds);
    }

    private void SpawnWaveOneBoss()
    {
        if (BossScene == null) return;
        WaveDefinition wave = Waves[_waveIndex];
        var boss = BossScene.Instantiate<BossEnemy>();
        boss.Position = Mohsen.Position + WaveOneBossOffset;
        AddChild(boss);
        boss.Configure(this, wave.WaveNumber, wave.WaveDangerLevelHealthMultiplier,
            wave.WaveDangerLevelDamageMultiplier, 1f, wave.EnemySpeedMultiplier, 0f);
        boss.SetWaveName(wave.WaveName, wave.WaveNumber);
        Enemies.Add(boss);
    }

    private void BeginBossOpportunity(WaveDefinition wave)
    {
        if (BossScene == null || WarningScene == null) return;
        float warningSeconds = Mathf.Max(0f, wave.BossSpawnWarningSeconds);
        if (_waveClock + warningSeconds > wave.Duration) return;
        float margin = WallThickness + 135f;
        Vector2 desired = Mohsen.Position + wave.BossSpawnOffset;
        Vector2 position = new(
            Mathf.Clamp(desired.X, margin, ArenaSize.X - margin),
            Mathf.Clamp(desired.Y, margin, ArenaSize.Y - margin));
        PackedScene scene = SelectTankScene(BossScene, wave.BossTankChance);
        var warning = WarningScene.Instantiate<SpawnWarning>();
        warning.Position = position;
        AddChild(warning);
        warning.SetTiming(warningSeconds, warningSeconds);
        var pending = new PendingSpawn { Due = _waveClock + warningSeconds, WarningDuration = warningSeconds };
        pending.Entries.Add((scene, position, 1f, 1f, 0f, 1f, warning));
        _pending.Add(pending);
    }

    private void EndWave(bool forceShop = false)
    {
        ResetImpactPause();
        _bossOpportunityPending = false;
        WaveActive = false;
        Mohsen.Active = false;
        Mohsen.ClearTemporaryEffects();
        foreach (var weapon in _weapons) weapon.ResetTransientEffects();
        ClearWaveObjects();
        _waveClearForceShop = forceShop;
        BeginWaveBeat("WAVE CLEARED", WaveClearBeatSeconds);
        _waveClearRemaining = WaveClearBeatSeconds;
        if (_waveClearRemaining <= 0f) CompleteWaveTransition();
    }

    private void CompleteWaveTransition()
    {
        _waveClearRemaining = 0f;
        _waveBeatRemaining = 0f;
        _hudWaveBeat.Visible = false;
        _hudScreen.Visible = false;
        bool forceShop = _waveClearForceShop;
        _waveClearForceShop = false;
        if (_waveIndex == Waves.Count - 1 && !forceShop) { ShowFinish(true); return; }
        _productManagerDebugOffers = false;
        _rerollCount = 0;
        SetShopMessage(_hasEnteredShop ? "" : ShopGreeting);
        _hasEnteredShop = true;
        GenerateOffers();
        ShowShop();
    }

    private void OnPlayerDied()
    {
        ResetImpactPause();
        _bossOpportunityPending = false;
        WaveActive = false;
        Mohsen.ClearTemporaryEffects();
        foreach (var weapon in _weapons) weapon.ResetTransientEffects();
        ClearWaveObjects();
        _waveBeatRemaining = 0f;
        _waveClearRemaining = 0f;
        _hudWaveBeat.Visible = false;
        UpdateHud();
        _finishDelay = 0.7f;
    }

    private void BeginBurst(SpawnBurstDefinition burst)
    {
        if (burst == null || burst.WarningDuration < 0f || burst.TriggerTime + burst.WarningDuration > Waves[_waveIndex].Duration) return;
        var pending = new PendingSpawn { Due = _waveClock + burst.WarningDuration, WarningDuration = burst.WarningDuration };
        foreach (var group in burst.Groups)
        {
            if (group == null || group.EnemyScene == null || group.Count < 0) continue;
            int alternatePhase = _rng.RandiRange(0, 1);
            for (int n = 0; n < Mathf.Min(group.Count, 100); n++)
            {
                Vector2 position = ChooseSpawnPosition();
                var warning = WarningScene.Instantiate<SpawnWarning>();
                warning.Position = position;
                AddChild(warning);
                warning.SetTiming(burst.WarningDuration, burst.WarningDuration);
                PackedScene scene = group.AlternateEnemyScene != null && (n + alternatePhase) % 2 == 0
                    ? group.AlternateEnemyScene : group.EnemyScene;
                scene = SelectTankScene(scene, Waves[_waveIndex].TankSpawnChance);
                pending.Entries.Add((scene, position, group.HealthMultiplier, group.SpeedMultiplier,
                    group.KnockbackResistanceAdjustment, burst.DamageMultiplier, warning));
            }
        }
        _pending.Add(pending);
    }

    private PackedScene SelectTankScene(PackedScene baseScene, float chance)
    {
        if (baseScene == null || Waves[_waveIndex].WaveNumber < 3 || _rng.Randf() >= Mathf.Clamp(chance, 0f, 1f))
            return baseScene;
        PackedScene tankScene = baseScene.ResourcePath switch
        {
            "res://scenes/enemies/melee.tscn" => MeleeTankScene,
            "res://scenes/enemies/melee_female.tscn" => FemaleMeleeTankScene,
            "res://scenes/enemies/ranged.tscn" => RangedTankScene,
            "res://scenes/enemies/many_eyes.tscn" => ManyEyesTankScene,
            "res://scenes/enemies/one_eye_minion.tscn" => OneEyeMinionTankScene,
            "res://scenes/enemies/orange_boss.tscn" => BossTankScene,
            _ => null
        };
        return tankScene ?? baseScene;
    }

    private Vector2 ChooseSpawnPosition()
    {
        Vector2 chosen = ArenaSize / 2f;
        float spawnMargin = WallThickness + 50f;
        for (int attempt = 0; attempt < 40; attempt++)
        {
            chosen = new Vector2(_rng.RandfRange(spawnMargin, ArenaSize.X - spawnMargin), _rng.RandfRange(spawnMargin, ArenaSize.Y - spawnMargin));
            if (chosen.DistanceTo(Mohsen.Position) < MinimumSpawnDistance) continue;
            bool nearby = _pending.Any(p => p.Entries.Any(e => e.position.DistanceTo(chosen) < 42f));
            if (!nearby) return chosen;
        }
        return chosen;
    }

    private void SpawnPending(PendingSpawn pending)
    {
        foreach (var entry in pending.Entries)
        {
            if (GodotObject.IsInstanceValid(entry.warning)) entry.warning.QueueFree();
            if (entry.scene.Instantiate() is not Enemy enemy) continue;
            enemy.Position = entry.position;
            AddChild(enemy);
            WaveDefinition wave = Waves[_waveIndex];
            enemy.Configure(this, wave.WaveNumber, wave.WaveDangerLevelHealthMultiplier,
                wave.WaveDangerLevelDamageMultiplier, entry.hp,
                Mathf.Max(.1f, entry.speed * wave.EnemySpeedMultiplier), entry.resistance, entry.damage);
            if (enemy is BossEnemy boss) boss.SetWaveName(Waves[_waveIndex].WaveName, Waves[_waveIndex].WaveNumber);
            Enemies.Add(enemy);
        }
    }

    public void EnemyDefeated(Enemy enemy)
    {
        Enemies.Remove(enemy);
        if (!WaveActive) return;
        RequestImpactPause(DefeatPauseSeconds);
        PlaySound(GameSound.EnemyDefeat);
        if (EnemyExplosionScene != null)
        {
            var explosion = EnemyExplosionScene.Instantiate<EnemyExplosion>();
            AddChild(explosion);
            explosion.GlobalPosition = enemy.GlobalPosition;
        }
        if (EnemyDeathTextScene != null)
        {
            var textEffect = EnemyDeathTextScene.Instantiate<EnemyDeathText>();
            textEffect.Position = enemy.Position + new Vector2(0f, -32f);
            AddChild(textEffect);
            textEffect.SetMessage(DeathMessages[_rng.RandiRange(0, DeathMessages.Length - 1)]);
        }
        int roll = _rng.RandiRange(0, 99);
        int dropCount = roll < 10 ? 0 : roll < 40 ? 1 : roll < 75 ? 2 : 3;
        for (int i = 0; i < dropCount && CoinScene != null; i++)
        {
            var coin = CoinScene.Instantiate<Coin>();
            coin.Game = this;
            float angle = _rng.RandfRange(0f, Mathf.Tau);
            float distance = _rng.RandfRange(0f, CoinDropScatterRadius);
            coin.Position = enemy.Position + Vector2.Right.Rotated(angle) * distance;
            AddChild(coin);
            _coins.Add(coin);
        }
        if (CoffeePickupScene != null && _rng.Randf() < Mathf.Clamp(CoffeeDropChance, 0f, 1f))
        {
            var coffee = CoffeePickupScene.Instantiate<ConsumablePickup>();
            coffee.Game = this;
            coffee.Position = enemy.Position + Vector2.Right.Rotated(_rng.RandfRange(0f, Mathf.Tau)) * _rng.RandfRange(0f, CoinDropScatterRadius);
            AddChild(coffee);
            _consumables.Add(coffee);
        }
    }

    private void SpawnWaterDispensers()
    {
        if (WaterDispenserScene == null) return;
        int minimum = Mathf.Max(1, MinimumWaterDispensers);
        int count = _rng.RandiRange(minimum, Mathf.Max(minimum, MaximumWaterDispensers));
        float margin = WallThickness + 72f;
        for (int i = 0; i < count; i++)
        {
            Vector2 position = Vector2.Zero;
            for (int attempt = 0; attempt < 20; attempt++)
            {
                position = new Vector2(_rng.RandfRange(margin, ArenaSize.X - margin), _rng.RandfRange(margin, ArenaSize.Y - margin));
                if (position.DistanceTo(Mohsen.Position) < 180f) continue;
                if (_waterDispensers.Any(dispenser => dispenser.Position.DistanceTo(position) < 130f)) continue;
                break;
            }
            var dispenser = WaterDispenserScene.Instantiate<WaterDispenser>();
            dispenser.Game = this;
            dispenser.Position = position;
            AddChild(dispenser);
            _waterDispensers.Add(dispenser);
        }
    }

    public void SpawnWater(WaterDispenser dispenser)
    {
        if (!WaveActive || WaterPickupScene == null || !GodotObject.IsInstanceValid(dispenser)) return;
        float angle = _rng.RandfRange(0f, Mathf.Tau);
        float distance = _rng.RandfRange(Mathf.Min(WaterDropMinimumDistance, WaterDropMaximumDistance), Mathf.Max(WaterDropMinimumDistance, WaterDropMaximumDistance));
        float margin = WallThickness + 25f;
        Vector2 destination = dispenser.Position + Vector2.Right.Rotated(angle) * distance;
        destination = new Vector2(Mathf.Clamp(destination.X, margin, ArenaSize.X - margin), Mathf.Clamp(destination.Y, margin, ArenaSize.Y - margin));
        var water = WaterPickupScene.Instantiate<ConsumablePickup>();
        water.Game = this;
        AddChild(water);
        water.Launch(dispenser.Position, destination);
        _consumables.Add(water);
    }

    public void CollectConsumable(ConsumablePickup pickup)
    {
        _consumables.Remove(pickup);
        if (!WaveActive || Mohsen == null) return;
        if (pickup.Kind == ConsumablePickup.PickupKind.Water) Mohsen.Heal(WaterHealing);
        else Mohsen.ApplyCoffeeBoost(pickup.CoffeeSpeedBonusPercent, pickup.CoffeeDurationSeconds);
        PlaySound(GameSound.CoinPickup);
        UpdateHud();
    }

    private void OnPlayerHealed(float amount)
    {
        if (amount <= 0f || DamageNumberScene == null) return;
        var number = DamageNumberScene.Instantiate<DamageNumber>();
        number.Position = Mohsen.Position + new Vector2(0f, -42f);
        AddChild(number);
        number.SetHealing(amount);
    }

    private void OnPlayerDamaged(float amount)
    {
        if (amount <= 0f) return;
        if (DamageNumberScene != null)
        {
            var number = DamageNumberScene.Instantiate<DamageNumber>();
            number.Position = Mohsen.Position + new Vector2(0f, -42f);
            AddChild(number);
            number.SetPlayerDamage(amount);
        }
        if (!_hudScreen.Visible) return;
        _hudDamagePunchRemaining = HudDamagePunchSeconds;
        _hudDamageEdges.Pulse();
    }

    private void UpdateHudDamagePunch(float delta)
    {
        _hudDamagePunchRemaining = Mathf.Max(0f, _hudDamagePunchRemaining - delta);
        float progress = HudDamagePunchSeconds <= 0f ? 0f : _hudDamagePunchRemaining / HudDamagePunchSeconds;
        Vector2 scale = Vector2.One * (1f + HudDamagePunchScale * progress * progress);
        if (_hudHpBar != null) _hudHpBar.Scale = scale;
        if (_hudHpText != null) _hudHpText.Scale = scale;
    }

    private void UpdateHudCoinBounce(float delta)
    {
        _hudCoinBounceRemaining = Mathf.Max(0f, _hudCoinBounceRemaining - delta);
        float progress = HudCoinBounceSeconds <= 0f ? 0f : _hudCoinBounceRemaining / HudCoinBounceSeconds;
        Vector2 scale = Vector2.One * (1f + HudCoinBounceScale * progress * progress);
        if (_hudCoinText != null) _hudCoinText.Scale = scale;
        if (_hudCoinIcon != null) _hudCoinIcon.Scale = scale;
    }

    private void BeginWaveBeat(string text, float seconds)
    {
        _waveBeatDuration = Mathf.Max(0f, seconds);
        _waveBeatRemaining = _waveBeatDuration;
        _hudWaveBeat.Text = text;
        _hudWaveBeat.Visible = seconds > 0f;
        _hudWaveBeat.PivotOffset = new Vector2(450f, 34f);
        _hudWaveBeat.Scale = Vector2.One * 1.12f;
        _hudWaveBeat.Modulate = Colors.White;
    }

    private void UpdateWaveBeat(float delta)
    {
        if (_waveBeatRemaining <= 0f) return;
        _waveBeatRemaining = Mathf.Max(0f, _waveBeatRemaining - delta);
        float progress = _waveBeatDuration <= 0f ? 0f : _waveBeatRemaining / _waveBeatDuration;
        _hudWaveBeat.Scale = Vector2.One * (1f + 0.12f * progress);
        _hudWaveBeat.Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, progress * 3f));
        if (_waveBeatRemaining <= 0f) _hudWaveBeat.Visible = false;
    }

    private void ClearWaterAndCoffee()
    {
        foreach (var dispenser in _waterDispensers) if (GodotObject.IsInstanceValid(dispenser)) dispenser.QueueFree();
        _waterDispensers.Clear();
        foreach (var pickup in _consumables) if (GodotObject.IsInstanceValid(pickup)) pickup.QueueFree();
        _consumables.Clear();
    }

    public void SpawnEnemyOffspring(PackedScene scene, Vector2 at, float healthMultiplier, float speedMultiplier, float damageMultiplier)
    {
        if (!WaveActive || scene == null) return;
        PackedScene chosenScene = SelectTankScene(scene, Waves[_waveIndex].TankSpawnChance);
        if (chosenScene.Instantiate() is not Enemy offspring) return;
        float boundary = WallThickness + 48f;
        offspring.Position = new Vector2(
            Mathf.Clamp(at.X, boundary, ArenaSize.X - boundary),
            Mathf.Clamp(at.Y, boundary, ArenaSize.Y - boundary));
        AddChild(offspring);
        WaveDefinition wave = Waves[_waveIndex];
        offspring.Configure(this, wave.WaveNumber, wave.WaveDangerLevelHealthMultiplier,
            wave.WaveDangerLevelDamageMultiplier, healthMultiplier,
            Mathf.Max(0.1f, speedMultiplier), 0f, damageMultiplier);
        Enemies.Add(offspring);
    }

    public void CollectCoin(Coin coin)
    {
        _coins.Remove(coin);
        _coinsHeld++;
        _hudCoinBounceRemaining = HudCoinBounceSeconds;
        _audio?.PlayCoinCollected();
        UpdateHud();
    }

    public void PlaySound(GameSound cue) => _audio?.Play(cue);

    public void RequestKnifeCritPause() => RequestImpactPause(KnifeCritPauseSeconds);
    public void RequestBossHitPause() => RequestImpactPause(BossHitPauseSeconds);

    private void RequestImpactPause(float seconds)
    {
        if (!WaveActive || seconds <= 0f) return;
        if (_impactPauseRemaining <= 0f && _impactPauseCooldown > 0f) return;
        _impactPauseRemaining = Mathf.Max(_impactPauseRemaining, Mathf.Min(seconds, 0.2f));
        _impactPauseCooldown = Mathf.Max(_impactPauseCooldown, _impactPauseRemaining + ImpactPauseMinGapSeconds);
    }

    private void ResetImpactPause()
    {
        _impactPauseRemaining = 0f;
        _impactPauseCooldown = 0f;
        _impactPauseFrame = false;
    }

    public void SpawnDamageNumber(float amount, Vector2 at, bool criticalHit)
    {
        if (DamageNumberScene == null) return;
        var number = DamageNumberScene.Instantiate<DamageNumber>();
        number.Position = at;
        AddChild(number);
        number.SetDamage(amount, criticalHit);
    }

    public void SpawnPlayerBulletHit(Vector2 at)
    {
        if (PlayerBulletHitScene == null) return;
        var effect = PlayerBulletHitScene.Instantiate<PlayerBulletHit>();
        effect.Position = at;
        AddChild(effect);
    }

    public void SpawnEnemyProjectile(Vector2 from, Vector2 direction, float speed, float damage)
    {
        SpawnHostileProjectile(EnemyBulletScene, from, direction, speed, damage);
    }

    public void SpawnBossProjectile(Vector2 from, Vector2 direction, float speed, float damage)
    {
        SpawnHostileProjectile(EnemyBulletScene, from, direction, speed, damage);
    }

    public void SpawnManyEyesProjectile(PackedScene scene, Vector2 from, Vector2 direction, float speed, float damage)
    {
        SpawnHostileProjectile(scene, from, direction, speed, damage);
    }

    private void SpawnHostileProjectile(PackedScene scene, Vector2 from, Vector2 direction, float speed, float damage)
    {
        if (scene == null) return;
        var projectile = scene.Instantiate<Projectile>();
        projectile.Game = this;
        projectile.Position = from;
        projectile.Direction = direction;
        projectile.Speed = speed;
        projectile.RemainingDistance = 3000f;
        projectile.Damage = Mathf.Max(0f, damage);
        AddChild(projectile);
        _projectiles.Add(projectile);
    }

    public void FireWeapon(Weapon weapon, Vector2 direction, WeaponStats stats)
    {
        PlaySound(weapon.Kind switch
        {
            WeaponKind.Knife => GameSound.KnifeShot,
            WeaponKind.Pistol => GameSound.PistolShot,
            WeaponKind.Smg => GameSound.SmgShot,
            WeaponKind.Shotgun => GameSound.ShotgunShot,
            _ => GameSound.PistolShot
        });
        if (weapon.Kind == WeaponKind.Shotgun)
        {
            FireShotgun(weapon, direction, stats);
            Mohsen.ShakeOnShot(weapon.ShotShakePixels);
            return;
        }

        float damage = EffectiveWeaponDamage(weapon.Kind, stats);
        bool critical = _rng.Randf() < EffectiveCritChance(stats);
        if (critical) damage *= stats.CritMultiplier;
        bool knife = weapon.Kind == WeaponKind.Knife;
        PackedScene scene = weapon.Kind switch
        {
            WeaponKind.Knife => KnifeAttackScene,
            WeaponKind.Pistol => PistolBulletScene,
            _ => SmgBulletScene
        };
        var shot = scene.Instantiate<Projectile>();
        shot.Game = this;
        shot.SourceScene = scene;
        shot.Position = weapon.ShotOrigin(direction);
        shot.Direction = direction;
        if (!knife) shot.Speed = weapon.Kind == WeaponKind.Smg ? 650f : 580f;
        shot.RemainingDistance = Mathf.Max(20f, stats.Range - weapon.Position.Length());
        shot.RangeAtSpawn = shot.RemainingDistance;
        shot.Damage = damage;
        shot.SizeMultiplier = Mohsen.ProjectileSizeMultiplier;
        shot.CriticalHit = critical;
        shot.Knockback = stats.Knockback + Mohsen.Knockback;
        if (!knife) shot.PierceCapacity = shot.RemainingPierces = Mohsen.BulletPierce;
        AddChild(shot);
        _projectiles.Add(shot);
        Mohsen.ShakeOnShot(weapon.ShotShakePixels);
    }

    private void FireShotgun(Weapon weapon, Vector2 direction, WeaponStats stats)
    {
        ShotgunTierDefinition profile = WeaponData.Shotgun(weapon.Tier);
        int pelletCount = Mathf.Max(1, profile.PelletCount);
        Vector2 origin = weapon.ShotOrigin(direction);
        float travelRange = Mathf.Max(1f, stats.Range - origin.DistanceTo(Mohsen.GlobalPosition));
        for (int i = 0; i < pelletCount; i++)
        {
            float angle = pelletCount == 1 ? 0f : Mathf.DegToRad(profile.SpreadDegrees) * (i / (float)(pelletCount - 1) - 0.5f);
            Vector2 pelletDirection = direction.Rotated(angle);
            var pellet = ShotgunPelletScene.Instantiate<Projectile>();
            pellet.Game = this;
            pellet.SourceScene = ShotgunPelletScene;
            pellet.Position = origin;
            pellet.Direction = pelletDirection;
            pellet.Speed = Mathf.Max(1f, profile.PelletSpeed);
            pellet.LifetimeSeconds = Mathf.Max(0.01f, profile.PelletLifetimeSeconds);
            pellet.RemainingDistance = travelRange;
            pellet.RangeAtSpawn = travelRange;
            pellet.Damage = EffectiveWeaponDamage(WeaponKind.Shotgun, stats);
            pellet.SizeMultiplier = Mohsen.ProjectileSizeMultiplier;
            pellet.CriticalHit = _rng.Randf() < EffectiveCritChance(stats);
            if (pellet.CriticalHit) pellet.Damage *= stats.CritMultiplier;
            pellet.Knockback = stats.Knockback + Mohsen.Knockback;
            pellet.PierceCapacity = pellet.RemainingPierces = Mohsen.BulletPierce;
            AddChild(pellet);
            _projectiles.Add(pellet);
        }
    }

    public void SpawnSpreadBullets(Projectile source, Vector2 at, ulong originalTargetId)
    {
        if (!WaveActive || source.SourceScene == null || source.Hostile || source.Knife || !source.CanSpread) return;
        int count = Mathf.Max(0, Mohsen.Spread);
        for (int i = 0; i < count; i++)
        {
            var child = source.SourceScene.Instantiate<Projectile>();
            child.Game = this;
            child.SourceScene = source.SourceScene;
            child.CanSpread = false;
            child.Position = at;
            child.Direction = source.Direction.Rotated(Mathf.Tau * i / count);
            child.Hostile = source.Hostile;
            child.Knife = source.Knife;
            child.Speed = source.Speed;
            child.LifetimeSeconds = source.LifetimeSeconds;
            child.RemainingDistance = source.RangeAtSpawn;
            child.RangeAtSpawn = source.RangeAtSpawn;
            child.HitRadius = source.HitRadius;
            child.SizeMultiplier = source.SizeMultiplier;
            child.EaseSpeed = source.EaseSpeed;
            child.EndSpeedMultiplier = source.EndSpeedMultiplier;
            child.EaseExponent = source.EaseExponent;
            child.TrailLifetimeSeconds = source.TrailLifetimeSeconds;
            child.Damage = source.Damage;
            child.CriticalHit = source.CriticalHit;
            child.Knockback = source.Knockback;
            child.PierceCapacity = source.PierceCapacity;
            child.RemainingPierces = source.PierceCapacity;
            child.IgnoreEnemyForLife(originalTargetId);
            AddChild(child);
            _projectiles.Add(child);
        }
    }

    private Weapon AddWeapon(WeaponKind kind, int tier)
    {
        PackedScene scene = kind switch
        {
            WeaponKind.Knife => KnifeScene,
            WeaponKind.Pistol => PistolScene,
            WeaponKind.Smg => SmgScene,
            WeaponKind.Shotgun => ShotgunScene,
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        var weapon = scene.Instantiate<Weapon>();
        weapon.Tier = tier;
        Mohsen.WeaponMount.AddChild(weapon);
        _weapons.Add(weapon);
        RefreshWeaponSlots();
        return weapon;
    }

    private void RefreshWeaponSlots()
    {
        for (int i = 0; i < _weapons.Count; i++)
            _weapons[i].Equip(this, Mohsen, i);
    }

    private bool CanBuyWeapon(WeaponKind kind, int tier) => _weapons.Any(w => w.Kind == kind && w.Tier == tier && tier < 4) || _weapons.Count < 4;

    private Weapon GiveWeapon(WeaponKind kind, int tier, out bool merged)
    {
        var matching = _weapons.FirstOrDefault(w => w.Kind == kind && w.Tier == tier && tier < 4);
        merged = matching != null;
        if (matching == null) return AddWeapon(kind, tier);
        matching.Tier++;
        // Chain merges can occur after a purchase upgrades an existing weapon.
        while (matching.Tier < 4)
        {
            var next = _weapons.FirstOrDefault(w => w != matching && w.Kind == kind && w.Tier == matching.Tier);
            if (next == null) break;
            _weapons.Remove(next);
            if (GodotObject.IsInstanceValid(next)) next.QueueFree();
            matching.Tier++;
        }
        RefreshWeaponSlots();
        return matching;
    }

    private void GenerateOffers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_locks[i]) continue;
            _offers[i] = RandomOffer();
        }
    }

    private void DebugProductManagerReroll()
    {
        _productManagerDebugOffers = true;
        Array.Clear(_locks, 0, _locks.Length);
        for (int i = 0; i < _offers.Length; i++)
            _offers[i] = RandomProductManagerOffer();
        SetShopMessage(_offers.All(offer => offer == null)
            ? "No eligible Product Manager items."
            : "Debug: Product Manager items only.");
        ShowShop();
    }

    private Offer RandomProductManagerOffer()
    {
        ShopItemDefinition[] eligible = ItemPool.Where(item => IsEligibleItem(item) &&
            string.Equals(item.Description?.Trim(), "Product manager.", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (eligible.Length == 0) return null;
        ShopItemDefinition item = WeightedItem(eligible);
        return new Offer { Item = item, Price = item.Price };
    }

    private Offer RandomOfferForCurrentShop() => _productManagerDebugOffers
        ? RandomProductManagerOffer() : RandomOffer();

    private Offer RandomOffer()
    {
        ShopItemDefinition[] eligibleItems = ItemPool.Where(IsEligibleItem).ToArray();
        int maxTier = Mathf.Min(4, 1 + _waveIndex / 2);
        var eligibleWeapons = new List<(WeaponKind kind, int tier)>();
        foreach (WeaponKind kind in Enum.GetValues<WeaponKind>())
            for (int tier = 1; tier <= maxTier; tier++)
                if (CanBuyWeapon(kind, tier)) eligibleWeapons.Add((kind, tier));
        bool chooseItem = eligibleItems.Length > 0 && (eligibleWeapons.Count == 0 || _rng.Randf() >= 0.4f);
        if (chooseItem)
        {
            ShopItemDefinition item = WeightedItem(eligibleItems);
            return new Offer { Item = item, Price = item.Price };
        }
        if (eligibleWeapons.Count == 0) return null;
        var weapon = eligibleWeapons[_rng.RandiRange(0, eligibleWeapons.Count - 1)];
        return new Offer { Kind = weapon.kind, Tier = weapon.tier, Price = TierOneWeaponPrice + (weapon.tier - 1) * WeaponTierPriceStep };
    }

    private ShopItemDefinition WeightedItem(ShopItemDefinition[] items)
    {
        int completedWave = Mathf.Clamp(_waveIndex + 1, 1, 20);
        float Weight(ShopItemDefinition item) => Mathf.Clamp(item.Rarity, 1, 3) switch
        {
            1 => 12f,
            2 => 4f + 0.25f * (completedWave - 1),
            _ => 1f + 0.25f * (completedWave - 1)
        };
        float total = items.Sum(Weight);
        float roll = _rng.Randf() * total;
        foreach (ShopItemDefinition item in items)
        {
            roll -= Weight(item);
            if (roll < 0f) return item;
        }
        return items[^1];
    }

    private bool IsEligibleItem(ShopItemDefinition item) =>
        item != null && (!item.Unique || !_purchasedItems.ContainsKey(item));

    private bool IsSafeHealthOffer(ShopItemDefinition item)
    {
        if (item == null) return false;
        float healthChange = item.Modifiers.Where(modifier => modifier != null && modifier.Stat == PlayerStat.MaxHealth)
            .Sum(modifier => modifier.Amount);
        return Mohsen.MaxHealth + healthChange > 0f;
    }

    private void ReplaceIneligibleOffers()
    {
        for (int i = 0; i < _offers.Length; i++)
        {
            if (_offers[i]?.Item == null || IsEligibleItem(_offers[i].Item)) continue;
            _locks[i] = false;
            _offers[i] = RandomOfferForCurrentShop();
        }
    }

    private void ShowShop()
    {
        ReplaceIneligibleOffers();
        ShowScreen(ShopScene);
        _screen.GetNode<Label>("Title").Text = ShopTitleText();
        _screen.GetNode<Label>("Coins").Text = ShopCoinsText();
        _screen.GetNode<Label>("StatsPanel/Box/Stats").Text = ShopStatsText();
        var bubble = _screen.GetNode<ShopSpeechBubble>("SpeechBubble");
        bubble.ShowMessage(_message, _lastShownMessageVersion != _messageVersion);
        _lastShownMessageVersion = _messageVersion;
        var rows = _screen.GetNode<HBoxContainer>("Offers");
        for (int i = 0; i < 4; i++)
        {
            int slot = i;
            Offer offer = _offers[i];
            var row = ShopOfferScene.Instantiate<PanelContainer>();
            rows.AddChild(row);
            var icon = row.GetNode<TextureRect>("Content/Header/Icon");
            var name = row.GetNode<Label>("Content/Header/Heading/Name");
            var kind = row.GetNode<Label>("Content/Header/Heading/Kind");
            var description = row.GetNode<RichTextLabel>("Content/Description");
            var buy = row.GetNode<Button>("Content/BuyRow/Buy");
            var buyCoin = row.GetNode<TextureRect>("Content/BuyRow/CoinIcon");
            var lockButton = row.GetNode<Button>("Content/Lock");
            if (offer == null)
            {
                icon.Visible = false;
                name.Text = "Empty";
                kind.Text = "Slot " + (i + 1);
                description.Text = "Empty. Reroll or continue to refill this slot.";
                buy.Visible = false;
                buyCoin.Visible = false;
                lockButton.Visible = false;
            }
            else
            {
                if (offer.Item != null) ApplyItemRarityFrame(row, offer.Item.Rarity);
                icon.Texture = offer.Item != null ? offer.Item.Icon : GD.Load<Texture2D>(WeaponData.Icon(offer.Kind));
                name.Text = offer.Title;
                kind.Text = offer.Item != null
                    ? $"{offer.Item.Description} · Rarity {offer.Item.Rarity}{(offer.Item.Unique ? " · Unique" : "")}"
                    : $"Weapon · Tier {Roman(offer.Tier)}";
                description.Text = OfferDescription(offer);
                if (offer.Item != null)
                {
                    string tooltip = ItemTooltip(offer.Item, 1);
                    row.TooltipText = tooltip;
                    icon.TooltipText = tooltip;
                    description.TooltipText = tooltip;
                }
                buy.Text = $"Buy · {offer.Price}";
                buy.Pressed += () => Buy(slot);
                lockButton.Text = _locks[i] ? "Unlock" : "Lock";
                lockButton.Pressed += () => { _locks[slot] = !_locks[slot]; ShowShop(); };
            }
        }
        _screen.GetNode<Button>("Reroll").Text = $"Reroll · {5 * (_rerollCount + 1)}";
        _screen.GetNode<Button>("Reroll").Pressed += Reroll;
        _screen.GetNode<Button>("RerollCoinButton").Pressed += Reroll;
        _screen.GetNode<Button>("Next").Text = _waveIndex == Waves.Count - 1 ? "Finish" : $"Start Wave {_waveIndex + 2}";
        _screen.GetNode<Button>("Next").Pressed += () => { _waveIndex++; StartWave(); };
        PopulateShopInventory();
    }

    private string ShopCoinsText() => $"Coins: {_coinsHeld}";

    private string ShopTitleText()
    {
        WaveDefinition next = _waveIndex + 1 < Waves.Count ? Waves[_waveIndex + 1] : null;
        if (next == null) return "Shop — Last Wave";
        string name = next.WaveName?.Trim() ?? "";
        return string.IsNullOrEmpty(name)
            ? $"Shop — Wave-{next.WaveNumber}"
            : $"Shop — Wave-{next.WaveNumber}-{name}";
    }

    private string ShopStatsText() =>
        $"HP: {Mathf.CeilToInt(Mohsen.Health)}/{Mathf.CeilToInt(Mohsen.MaxHealth)}\n" +
        $"Max HP: {Mohsen.MaxHealth:0.#}\n" +
        $"HP regeneration: {Mohsen.HealthRegenPerSecond:0.##}/s\n" +
        $"Movement speed: {Mohsen.EffectiveMovementSpeed:0.#}\n" +
        $"Knockback: {Mohsen.Knockback:0.#}\n" +
        $"Bonus damage: {Mohsen.BonusDamagePercent:0.#}%\n" +
        $"Critical chance bonus: {Mohsen.CriticalChancePercent:+0.#;-0.#;0}%\n" +
        $"Size: {Mohsen.ProjectileSizeMultiplier * 100f:0}% · {(Mohsen.CannotShootWhileMoving ? "No moving shots" : "Moving shots OK")}\n" +
        $"Ranged damage: {Mohsen.RangedDamagePercent:0.#}%\n" +
        $"Melee damage: {Mohsen.MeleeDamagePercent:0.#}%\n" +
        $"Pickup radius: {Mohsen.CoinPickupRadius:0.#}\n" +
        $"Spread: +{Mohsen.Spread}\n" +
        $"Bullet pierce: +{Mohsen.BulletPierce}\n" +
        $"Weapons: {_weapons.Count}/4\n" +
        $"Bought items: {_purchasedItems.Values.Sum()}";

    private string OfferDescription(Offer offer)
    {
        if (offer.Item != null)
            return string.Join("\n", offer.Item.Modifiers.Where(modifier => modifier != null).Select(ColoredModifierSummary));

        WeaponStats stats = WeaponData.Get(offer.Kind, offer.Tier);
        if (offer.Kind == WeaponKind.Shotgun)
        {
            ShotgunTierDefinition profile = WeaponData.Shotgun(offer.Tier);
            return $"Damage: {EffectiveWeaponDamage(offer.Kind, stats):0.#} per pellet\n" +
                   $"Pellets: {profile.PelletCount} · Spread: {profile.SpreadDegrees:0}°\n" +
                   $"Attack speed: {1f / stats.Interval:0.##}/s\n" +
                   $"Range: {stats.Range:0} px\n" +
                   $"Speed: {profile.PelletSpeed:0} px/s · Life: {profile.PelletLifetimeSeconds:0.##}s\n" +
                   $"Knockback: {stats.Knockback + Mohsen.Knockback:0.#} · Crit: {EffectiveCritChance(stats) * 100f:0}%\n" +
                   (Mohsen.BulletPierce > 0 ? $"Each pellet can hit {Mohsen.BulletPierce + 1} enemies" : "Each pellet stops on hit");
        }
        string behavior = offer.Kind == WeaponKind.Knife ? "Piercing throw"
            : Mohsen.BulletPierce > 0 ? $"Bullet can hit {Mohsen.BulletPierce + 1} enemies" : "Bullet stops on hit";
        return $"Damage: {EffectiveWeaponDamage(offer.Kind, stats):0.#}\n" +
               $"Attack speed: {1f / stats.Interval:0.##}/s\n" +
               $"Range: {stats.Range:0} px\n" +
               $"Knockback: {stats.Knockback + Mohsen.Knockback:0.#}\n" +
               $"Crit: {EffectiveCritChance(stats) * 100f:0}% · ×{stats.CritMultiplier:0.#}\n" +
               behavior;
    }

    private static string ModifierSummary(StatModifier modifier)
    {
        string name = modifier.Stat switch
        {
            PlayerStat.MaxHealth => "Max HP",
            PlayerStat.MovementSpeed => "movement speed",
            PlayerStat.Knockback => "knockback",
            PlayerStat.BonusDamagePercent => "all damage",
            PlayerStat.RangedDamagePercent => "ranged damage",
            PlayerStat.MeleeDamagePercent => "melee damage",
            PlayerStat.HealthRegenPerSecond => "HP regeneration/s",
            PlayerStat.Spread => "Spread bullets on hit",
            PlayerStat.BulletPierce => "bullet pierce",
            PlayerStat.CriticalChancePercent => "critical chance",
            PlayerStat.ProjectileSizePercent => "bigger projectiles",
            _ => modifier.Stat.ToString()
        };
        bool percent = modifier.Stat == PlayerStat.BonusDamagePercent || modifier.Stat == PlayerStat.RangedDamagePercent || modifier.Stat == PlayerStat.MeleeDamagePercent || modifier.Stat == PlayerStat.CriticalChancePercent || modifier.Stat == PlayerStat.ProjectileSizePercent;
        return $"{modifier.Amount:+0.##;-0.##;0}{(percent ? "%" : "")} {name}";
    }

    private static string ColoredModifierSummary(StatModifier modifier)
    {
        if (modifier.Stat == PlayerStat.BlockFiringWhileMoving)
            return "[color=#ff7676]You cant shoot while moving.[/color]";
        string color = modifier.Amount > 0f ? "#76df80" : modifier.Amount < 0f ? "#ff7676" : "#dddddd";
        return $"[color={color}]{ModifierSummary(modifier)}[/color]";
    }

    private void PopulateShopInventory()
    {
        var equipped = _screen.GetNode<GridContainer>("WeaponsScroll/WeaponsGrid");
        foreach (Weapon weapon in _weapons)
        {
            var entry = ShopInventoryEntryScene.Instantiate<Panel>();
            equipped.AddChild(entry);
            entry.GetNode<TextureRect>("Icon").Texture = GD.Load<Texture2D>(WeaponData.Icon(weapon.Kind));
            entry.GetNode<Label>("Count").Text = Roman(weapon.Tier);
            entry.TooltipText = WeaponTooltip(weapon);
        }

        var bought = _screen.GetNode<GridContainer>("ItemsScroll/ItemsGrid");
        foreach (var pair in _purchasedItems)
        {
            var entry = ShopInventoryEntryScene.Instantiate<Panel>();
            bought.AddChild(entry);
            ApplyItemRarityFrame(entry, pair.Key.Rarity);
            entry.GetNode<TextureRect>("Icon").Texture = pair.Key.Icon;
            entry.GetNode<Label>("Count").Text = pair.Value > 1 ? $"×{pair.Value}" : "";
            entry.TooltipText = ItemTooltip(pair.Key, pair.Value);
        }
        if (_purchasedItems.Count == 0)
        {
            var empty = new Label { Text = "No items bought yet" };
            bought.AddChild(empty);
        }
        _screen.GetNode<Label>("WeaponsTitle").Text = $"Weapons ({_weapons.Count}/4)";
        _screen.GetNode<Label>("ItemsTitle").Text = $"Items ({_purchasedItems.Values.Sum()})";
    }

    private static void ApplyItemRarityFrame(Control frame, int rarity)
    {
        if (rarity < 2 || rarity > 3 || frame.GetThemeStylebox("panel") is not StyleBoxFlat original) return;
        Color background = rarity == 2 ? new Color("#173b55") : new Color("#38244f");
        Color border = rarity == 2 ? new Color("#5c9fc4") : new Color("#aa83d2");
        var tinted = (StyleBoxFlat)original.Duplicate();
        tinted.BgColor = background;
        tinted.BorderColor = border;
        tinted.BorderWidthLeft = 3;
        tinted.BorderWidthTop = 3;
        tinted.BorderWidthRight = 3;
        tinted.BorderWidthBottom = 3;
        frame.AddThemeStyleboxOverride("panel", tinted);
    }

    private float EffectiveWeaponDamage(WeaponKind kind, WeaponStats stats)
    {
        float typedBonus = kind == WeaponKind.Knife ? Mohsen.MeleeDamagePercent : Mohsen.RangedDamagePercent;
        return stats.Damage * (1f + (Mohsen.BonusDamagePercent + stats.Scaling * typedBonus) / 100f);
    }

    private float EffectiveCritChance(WeaponStats stats) =>
        Mathf.Clamp(stats.CritChance + Mohsen.CriticalChancePercent / 100f, 0f, 1f);

    private string WeaponTooltip(Weapon weapon)
    {
        WeaponStats stats = WeaponData.Get(weapon.Kind, weapon.Tier);
        if (weapon.Kind == WeaponKind.Shotgun)
        {
            ShotgunTierDefinition profile = WeaponData.Shotgun(weapon.Tier);
            return $"Shotgun {Roman(weapon.Tier)}\n" +
                   $"Damage: {EffectiveWeaponDamage(weapon.Kind, stats):0.#} per pellet (base {stats.Damage:0.#})\n" +
                   $"Pellets: {profile.PelletCount} in {profile.SpreadDegrees:0}° spread\n" +
                   $"Attack speed: {1f / stats.Interval:0.##}/s ({stats.Interval:0.##}s between blasts)\n" +
                   $"Range: {stats.Range:0} px · Pellet speed: {profile.PelletSpeed:0} px/s · Life: {profile.PelletLifetimeSeconds:0.##}s\n" +
                   $"Knockback: {stats.Knockback + Mohsen.Knockback:0.#} per pellet · Crit: {EffectiveCritChance(stats) * 100f:0}% ×{stats.CritMultiplier:0.#}\n" +
                   (Mohsen.BulletPierce > 0 ? $"Each pellet can hit {Mohsen.BulletPierce + 1} enemies" : "Each pellet stops on its first enemy hit");
        }
        string behavior = weapon.Kind == WeaponKind.Knife
            ? "Thrown knife: hits each enemy once during flight"
            : Mohsen.BulletPierce > 0 ? $"Bullet: can hit {Mohsen.BulletPierce + 1} enemies"
            : "Bullet: disappears on the first enemy hit";
        return $"{WeaponData.Name(weapon.Kind)} {Roman(weapon.Tier)}\n" +
               $"Damage: {EffectiveWeaponDamage(weapon.Kind, stats):0.#} (base {stats.Damage:0.#})\n" +
               $"Attack speed: {1f / stats.Interval:0.##}/s ({stats.Interval:0.##}s between attacks)\n" +
               $"Range: {stats.Range:0} px  •  Knockback: {stats.Knockback + Mohsen.Knockback:0.#}\n" +
               $"Critical hit: {EffectiveCritChance(stats) * 100f:0}% chance, ×{stats.CritMultiplier:0.#} damage\n" +
               $"{behavior}";
    }

    private string ItemTooltip(ShopItemDefinition item, int count)
    {
        var lines = new List<string> { $"{item.DisplayName} ×{count}", string.IsNullOrWhiteSpace(item.Description) ? "Passive item" : item.Description, $"Rarity {item.Rarity}{(item.Unique ? " · Unique" : " · Repeatable")}" };
        foreach (StatModifier modifier in item.Modifiers)
        {
            if (modifier == null) continue;
            if (modifier.Stat == PlayerStat.BlockFiringWhileMoving)
            {
                lines.Add("You cant shoot while moving.");
                continue;
            }
            string name = modifier.Stat switch
            {
                PlayerStat.MaxHealth => "Max HP",
                PlayerStat.MovementSpeed => "Movement speed",
                PlayerStat.Knockback => "Knockback",
                PlayerStat.BonusDamagePercent => "All weapon damage",
                PlayerStat.RangedDamagePercent => "Ranged damage",
                PlayerStat.MeleeDamagePercent => "Melee damage",
                PlayerStat.HealthRegenPerSecond => "HP regeneration / s",
                PlayerStat.Spread => "Spread bullets on hit",
                PlayerStat.BulletPierce => "Bullet pierce",
                PlayerStat.CriticalChancePercent => "Critical chance",
                PlayerStat.ProjectileSizePercent => "Projectile size",
                _ => modifier.Stat.ToString()
            };
            bool percent = modifier.Stat == PlayerStat.BonusDamagePercent || modifier.Stat == PlayerStat.RangedDamagePercent || modifier.Stat == PlayerStat.MeleeDamagePercent || modifier.Stat == PlayerStat.CriticalChancePercent || modifier.Stat == PlayerStat.ProjectileSizePercent;
            string suffix = percent ? "%" : "";
            lines.Add($"{name}: {modifier.Amount:+0.##;-0.##;0}{suffix} each, {modifier.Amount * count:+0.##;-0.##;0}{suffix} total");
        }
        lines.Add($"Current all-damage bonus: {Mohsen.BonusDamagePercent:0.#}%");
        lines.Add($"Current critical chance bonus: {Mohsen.CriticalChancePercent:+0.#;-0.#;0}%");
        lines.Add($"Current projectile size: {Mohsen.ProjectileSizeMultiplier * 100f:0}%");
        foreach (Weapon weapon in _weapons)
        {
            WeaponStats stats = WeaponData.Get(weapon.Kind, weapon.Tier);
            string damageUnit = weapon.Kind == WeaponKind.Shotgun ? " damage/pellet" : " damage";
            lines.Add($"{WeaponData.Name(weapon.Kind)} {Roman(weapon.Tier)}: {EffectiveWeaponDamage(weapon.Kind, stats):0.#}{damageUnit}, {1f / stats.Interval:0.##} attacks/s");
        }
        return string.Join("\n", lines);
    }

    private void Buy(int slot)
    {
        Offer offer = _offers[slot];
        if (offer == null) return;
        if (offer.Item != null && !IsEligibleItem(offer.Item))
        {
            _locks[slot] = false;
            _offers[slot] = RandomOfferForCurrentShop();
            SetShopMessage("That item is no longer eligible.");
            ShowShop();
            return;
        }
        if (offer.Item != null && !IsSafeHealthOffer(offer.Item))
        {
            SetShopMessage("You can't buy that item. It would drop your max HP to 0 or below.");
            ShowShop();
            return;
        }
        if (_coinsHeld < offer.Price) { SetShopMessage("Not enough coins."); ShowShop(); return; }
        if (offer.Item == null && !CanBuyWeapon(offer.Kind, offer.Tier)) { SetShopMessage("Four weapons equipped; this offer cannot merge."); ShowShop(); return; }
        _coinsHeld -= offer.Price;
        PlaySound(GameSound.ShopPurchase);
        Weapon upgradedWeapon = null;
        bool merged = false;
        if (offer.Item != null)
        {
            foreach (var modifier in offer.Item.Modifiers) Mohsen.ApplyModifier(modifier);
            _purchasedItems.TryGetValue(offer.Item, out int copies);
            _purchasedItems[offer.Item] = copies + 1;
        }
        else upgradedWeapon = GiveWeapon(offer.Kind, offer.Tier, out merged);
        _offers[slot] = null;
        _locks[slot] = false;
        SetShopMessage(offer.Item != null ? $"Bought {offer.Item.DisplayName}."
            : merged ? $"{WeaponData.Name(offer.Kind)} upgraded to tier {Roman(upgradedWeapon.Tier)}!"
            : "Weapon equipped.");
        ShowShop();
        if (merged) ShowMergeFeedback(slot, upgradedWeapon);
    }

    private void ShowMergeFeedback(int offerSlot, Weapon upgradedWeapon)
    {
        var offers = _screen.GetNode<HBoxContainer>("Offers");
        if (offerSlot < 0 || offerSlot >= offers.GetChildCount()) return;
        var card = (PanelContainer)offers.GetChild(offerSlot);
        var icon = card.GetNode<TextureRect>("Content/Header/Icon");
        icon.Texture = GD.Load<Texture2D>(WeaponData.Icon(upgradedWeapon.Kind));
        icon.Visible = true;
        card.GetNode<Label>("Content/Header/Heading/Name").Text = "TIER UP!";
        card.GetNode<Label>("Content/Header/Heading/Kind").Text = $"{WeaponData.Name(upgradedWeapon.Kind)} · Tier {Roman(upgradedWeapon.Tier)}";
        card.GetNode<RichTextLabel>("Content/Description").Text = "Weapon merged successfully.";

        var grid = _screen.GetNode<GridContainer>("WeaponsScroll/WeaponsGrid");
        int weaponSlot = _weapons.IndexOf(upgradedWeapon);
        if (weaponSlot < 0 || weaponSlot >= grid.GetChildCount()) return;
        var entry = (Panel)grid.GetChild(weaponSlot);
        entry.GetNode<Label>("Count").Text = Roman(upgradedWeapon.Tier);

        AnimateTierUp(card, new Vector2(112f, 195f), 1.04f);
        AnimateTierUp(entry, new Vector2(36f, 36f), 1.28f);
    }

    private static void AnimateTierUp(Control control, Vector2 pivot, float peakScale)
    {
        control.PivotOffset = pivot;
        control.Scale = Vector2.One * peakScale;
        control.Modulate = new Color(1f, 0.83f, 0.36f);
        var tween = control.CreateTween();
        tween.TweenProperty(control, "scale", Vector2.One, 0.36f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(control, "modulate", Colors.White, 0.48f);
    }

    private void Reroll()
    {
        int cost = 5 * (_rerollCount + 1);
        if (_coinsHeld < cost) { SetShopMessage("Not enough coins to reroll."); ShowShop(); return; }
        _coinsHeld -= cost;
        _rerollCount++;
        _productManagerDebugOffers = false;
        GenerateOffers();
        SetShopMessage("Unlocked slots rerolled.");
        ShowShop();
    }

    private void SetShopMessage(string message)
    {
        _message = message;
        _messageVersion++;
    }

    private void ShowFinish(bool victory)
    {
        WaveActive = false;
        _hudScreen.Visible = false;
        if (!victory)
        {
            ShowScreen(DefeatScene);
            _screen.GetNode<Button>("PlayAgain").Pressed += ShowMenu;
            return;
        }
        ShowScreen(FinishScene);
        _screen.GetNode<Label>("Panel/Box/Title").Text = $"FIRST {Waves.Count} WAVES CLEARED";
        _screen.GetNode<Label>("Panel/Box/Subtitle").Text = "Current wave set cleared.";
        _screen.GetNode<TextureRect>("Panel/Box/Illustration").Texture = GD.Load<Texture2D>("res://assets/placeholders/passive.svg");
        _screen.GetNode<Button>("Panel/Box/PlayAgain").Pressed += ShowMenu;
    }

    private void ClearWaveObjects()
    {
        foreach (var enemy in Enemies) if (GodotObject.IsInstanceValid(enemy)) enemy.QueueFree();
        Enemies.Clear();
        foreach (var coin in _coins) if (GodotObject.IsInstanceValid(coin)) coin.QueueFree();
        _coins.Clear();
        ClearWaterAndCoffee();
        foreach (var shot in _projectiles) if (GodotObject.IsInstanceValid(shot)) shot.QueueFree();
        _projectiles.Clear();
        foreach (var pending in _pending)
            foreach (var entry in pending.Entries)
                if (GodotObject.IsInstanceValid(entry.warning)) entry.warning.QueueFree();
        _pending.Clear();
    }

    private void ClearWorld()
    {
        WaveActive = false;
        ClearWaveObjects();
        foreach (var weapon in _weapons) if (GodotObject.IsInstanceValid(weapon)) weapon.QueueFree();
        _weapons.Clear();
        if (GodotObject.IsInstanceValid(Mohsen)) Mohsen.QueueFree();
        Mohsen = null;
    }

    private void UpdateHud()
    {
        if (Mohsen == null || _waveIndex >= Waves.Count) return;
        WaveDefinition wave = Waves[_waveIndex];
        string waveName = wave.WaveName?.Trim() ?? "";
        _hudWaveName.Text = string.IsNullOrEmpty(waveName)
            ? $"Wave-{wave.WaveNumber}"
            : $"Wave-{wave.WaveNumber}-{waveName}";

        int seconds = Mathf.CeilToInt(Mathf.Max(0f, wave.Duration - _waveClock));
        _hudTimer.Text = $"{seconds}s";
        _hudTimer.AddThemeColorOverride("font_color", seconds < 10 ? Colors.Red : seconds < 20 ? Colors.Yellow : Colors.White);

        _hudHpBar.MaxValue = Mathf.Max(1f, Mohsen.MaxHealth);
        _hudHpBar.Value = Mathf.Clamp(Mohsen.Health, 0f, Mohsen.MaxHealth);
        _hudHpText.Text = $"{Mathf.CeilToInt(Mohsen.Health)}/{Mathf.CeilToInt(Mohsen.MaxHealth)}";
        _hudCoinText.Text = $"Coins: {_coinsHeld}";
        _hudEquipment.Text = $"Weapons: {string.Join(", ", _weapons.Select(w => WeaponData.Name(w.Kind) + " " + Roman(w.Tier)))}\n" +
                             $"Damage bonus: {Mohsen.BonusDamagePercent:0}%";
    }

    private void ClearScreen()
    {
        if (!GodotObject.IsInstanceValid(_screen)) return;
        _canvas.RemoveChild(_screen);
        _screen.QueueFree();
        _screen = null;
    }

    private void ShowScreen(PackedScene scene)
    {
        ClearScreen();
        _screen = scene.Instantiate<Control>();
        _canvas.AddChild(_screen);
    }

    private static string Roman(int tier) => tier switch { 1 => "I", 2 => "II", 3 => "III", _ => "IV" };
}
