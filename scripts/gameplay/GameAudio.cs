using Godot;
using System.Collections.Generic;

public enum GameSound
{
    KnifeShot, PistolShot, SmgShot, ShotgunShot,
    EnemyHit, EnemyDefeat, CoinPickup, ShopPurchase,
    WaveStart, MohsenHit, BossWarning
}

public partial class GameAudio : Node
{
    [Export] public AudioStream KnifeShot { get; set; }
    [Export] public AudioStream PistolShot { get; set; }
    [Export] public AudioStream SmgShot { get; set; }
    [Export] public AudioStream ShotgunShot { get; set; }
    [Export] public AudioStream EnemyHit { get; set; }
    [Export] public AudioStream EnemyDefeat { get; set; }
    [Export] public AudioStream CoinPickup { get; set; }
    [Export] public AudioStream ShopPurchase { get; set; }
    [Export] public AudioStream WaveStart { get; set; }
    [Export] public AudioStream MohsenHit { get; set; }
    [Export] public AudioStream BossWarning { get; set; }

    [Export(PropertyHint.Range, "1,24,1")] public int VoiceCount { get; set; } = 12;
    [Export(PropertyHint.Range, "-40,0,1")] public float ShotVolumeDb { get; set; } = -15f;
    [Export(PropertyHint.Range, "-40,0,1")] public float HitVolumeDb { get; set; } = -16f;
    [Export(PropertyHint.Range, "-40,0,1")] public float DefeatVolumeDb { get; set; } = -12f;
    [Export(PropertyHint.Range, "-40,0,1")] public float CoinVolumeDb { get; set; } = -17f;
    [Export(PropertyHint.Range, "-40,0,1")] public float UiVolumeDb { get; set; } = -13f;
    [Export(PropertyHint.Range, "-40,0,1")] public float WarningVolumeDb { get; set; } = -12f;
    [Export(PropertyHint.Range, "0,0.2,0.01")] public float PitchVariation { get; set; } = 0.04f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float SmgMinimumInterval { get; set; } = 0.08f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float CoinMinimumInterval { get; set; } = 0.07f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float EnemyHitMinimumInterval { get; set; } = 0.05f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float EnemyDefeatMinimumInterval { get; set; } = 0.08f;
    [Export(PropertyHint.Range, "0.1,1,0.05")] public float CoinRhythmGapSeconds { get; set; } = 0.35f;
    [Export(PropertyHint.Range, "0,0.1,0.005")] public float CoinPitchStep { get; set; } = 0.025f;
    [Export(PropertyHint.Range, "0,0.3,0.01")] public float CoinPitchMaxBonus { get; set; } = 0.15f;

    private readonly List<AudioStreamPlayer> _voices = new();
    private readonly Dictionary<GameSound, double> _lastPlayed = new();
    private readonly RandomNumberGenerator _random = new();
    private double _lastCoinCollectedAt = -1000.0;
    private int _coinChain;

    public override void _Ready()
    {
        _random.Randomize();
        for (int i = 0; i < Mathf.Clamp(VoiceCount, 1, 24); i++)
        {
            var voice = new AudioStreamPlayer();
            AddChild(voice);
            _voices.Add(voice);
        }
    }

    public void PlayCoinCollected()
    {
        double now = Time.GetTicksMsec() / 1000.0;
        _coinChain = now - _lastCoinCollectedAt <= CoinRhythmGapSeconds ? Mathf.Min(_coinChain + 1, 12) : 0;
        _lastCoinCollectedAt = now;
        Play(GameSound.CoinPickup, Mathf.Min(CoinPitchMaxBonus, _coinChain * CoinPitchStep));
    }

    public void Play(GameSound cue, float pitchBonus = 0f)
    {
        AudioStream stream = cue switch
        {
            GameSound.KnifeShot => KnifeShot,
            GameSound.PistolShot => PistolShot,
            GameSound.SmgShot => SmgShot,
            GameSound.ShotgunShot => ShotgunShot,
            GameSound.EnemyHit => EnemyHit,
            GameSound.EnemyDefeat => EnemyDefeat,
            GameSound.CoinPickup => CoinPickup,
            GameSound.ShopPurchase => ShopPurchase,
            GameSound.WaveStart => WaveStart,
            GameSound.MohsenHit => MohsenHit,
            GameSound.BossWarning => BossWarning,
            _ => null
        };
        if (stream == null) return;

        double now = Time.GetTicksMsec() / 1000.0;
        float minimumInterval = cue switch
        {
            GameSound.SmgShot => SmgMinimumInterval,
            GameSound.CoinPickup => CoinMinimumInterval,
            GameSound.EnemyHit => EnemyHitMinimumInterval,
            GameSound.EnemyDefeat => EnemyDefeatMinimumInterval,
            _ => 0f
        };
        if (_lastPlayed.TryGetValue(cue, out double last) && now - last < minimumInterval) return;
        AudioStreamPlayer voice = null;
        foreach (AudioStreamPlayer candidate in _voices)
            if (!candidate.Playing) { voice = candidate; break; }
        if (voice == null) return;

        float volume = cue switch
        {
            GameSound.KnifeShot or GameSound.PistolShot or GameSound.SmgShot or GameSound.ShotgunShot => ShotVolumeDb,
            GameSound.EnemyHit or GameSound.MohsenHit => HitVolumeDb,
            GameSound.EnemyDefeat => DefeatVolumeDb,
            GameSound.CoinPickup => CoinVolumeDb,
            GameSound.BossWarning => WarningVolumeDb,
            _ => UiVolumeDb
        };
        _lastPlayed[cue] = now;
        voice.Stream = stream;
        voice.VolumeDb = volume;
        voice.PitchScale = 1f + pitchBonus + _random.RandfRange(-PitchVariation, PitchVariation);
        voice.Play();
    }
}
