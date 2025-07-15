using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;

[TestSuite]
public class SpikeDynamicTest
{
    private ISceneRunner _runner;
    private SpikeDynamic _spikeDynamic;
    private Player _player;

    [BeforeTest]
    public async Task Setup()
    {
        _runner = ISceneRunner.Load("res://scenes/level1.tscn");
        await _runner.AwaitIdleFrame();
        Assertions.AssertThat(_runner).IsNotNull();
        // Laedt die SpikeDynamic-Szene und instanziiert sie
        _spikeDynamic = GD.Load<PackedScene>("res://scenes/spike_dynamic.tscn").Instantiate<SpikeDynamic>();
        
        // Laedt die Spieler-Szene und instanziiert sie
        _player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        _player.Name = "Player"; // Wichtig fuer die Erkennung im Spiel
        _runner.Scene().AddChild(_player);

        _runner.Scene().AddChild(_spikeDynamic);

        // Warte einen Frame, damit die Szene vollstaendig geladen ist
        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task WhenPlayerEntersSpike_PlayerHealthShouldDecrease()
    {
        // Test if player has maximum health
        _runner.Scene().AddChild(_spikeDynamic);
        float initialHealth = PlayerStats.Instance.GetCurrentHealth();

        Damage Damage = _spikeDynamic.GetDamage();  // Get damage from SpikeDynamic
        _player.TakeDamage(Damage);

        await _runner.SimulateFrames(1, 1);
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(initialHealth - Damage.GetTrueDMG());
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task PlayerTakesRepeatedDamageWhileOnSpike()
    {
        // Setze die Startgesundheit des Spielers
        PlayerStats.Instance.SetCurrentHealth(100.0f);
        var initialHealth = PlayerStats.Instance.GetCurrentHealth();

        // Positioniere den Spieler auf dem Spike
        _player.GlobalPosition = _spikeDynamic.GlobalPosition;

        // Simuliere das Betreten des Spikes
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, _player);
        
        // Warte auf den ersten Schaden
        await _runner.AwaitIdleFrame();
        
        // Simuliere eine Verzoegerung, um wiederholten Schaden zu ermoeglichen
        await _runner.SimulateFrames(1100);

        // Ueberpruefe, ob die Gesundheit des Spielers weiter reduziert wurde
        var newHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(initialHealth - _spikeDynamic.GetDamage().GetTrueDMG()).IsGreater(newHealth);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TimerStopsWhenPlayerLeavesSpike()
    {
        // Setze die Startgesundheit und positioniere den Spieler
        PlayerStats.Instance.SetCurrentHealth(100.0f);
        _player.GlobalPosition = _spikeDynamic.GlobalPosition;

        // Simuliere das Betreten und sofortige Verlassen des Spikes
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, _player);
        await _runner.AwaitIdleFrame();
        
        area2d.EmitSignal(Area2D.SignalName.BodyExited, _player);
        await _runner.AwaitIdleFrame();

        // Speichere die Gesundheit nach dem Verlassen
        var healthAfterExit = PlayerStats.Instance.GetCurrentHealth();

        // Warte eine Weile, um sicherzustellen, dass kein weiterer Schaden auftritt
        await _runner.SimulateFrames(2000);

        // Ueberpruefe, ob die Gesundheit unveraendert geblieben ist
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(healthAfterExit);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TimerNotStartedForNonPlayer()
    {
        // Erstelle ein anderes Objekt, das kein Spieler ist
        var otherObject = new CharacterBody2D();
        _runner.Scene().AddChild(otherObject);

        // Hole den Timer aus dem Spike
        var timer = _spikeDynamic.GetNode<Timer>("StaticBody2D/Area2D/Timer");
        
        // Simuliere, dass das andere Objekt den Spike betritt
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, otherObject);
        await _runner.AwaitIdleFrame();

        // Ueberpruefe, ob der Timer nicht gestartet wurde
        AssertThat(timer.IsStopped()).IsTrue();
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task WhenBaseEnemyEntersSpike_BaseEnemyHealthShouldNotChange()
    {
        // Erstelle einen Gegner und setze seine Gesundheit
        var baseEnemy = new BaseEnemy();
        baseEnemy.CurrentHealthPoints = 50.0f;
        _runner.Scene().AddChild(baseEnemy);
        var initialHealth = baseEnemy.CurrentHealthPoints;

        // Simuliere, dass der Gegner den Spike betritt
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, baseEnemy);
        await _runner.AwaitIdleFrame();

        // Ueberpruefe, ob die Gesundheit des Gegners unveraendert ist
        AssertThat(baseEnemy.CurrentHealthPoints).IsEqual(initialHealth);
        await Setup();
    }
}