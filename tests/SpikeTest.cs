using Godot;
using GdUnit4;
using System.Threading.Tasks; // Nötig für async/await
using static GdUnit4.Assertions;

/// <summary>
/// Test-Suite für die Spike-Klasse, die das echte Zusammenspiel mit dem Spieler testet.
/// </summary>
[TestSuite]
public class SpikeTest
{
    private ISceneRunner _runner;
    private Spike _spike;
    private Player _player;

    /// <summary>
    /// Diese Methode wird vor jedem einzelnen Testfall ausgeführt.
    /// Sie sorgt für eine saubere und konsistente Testumgebung.
    /// </summary>
    [BeforeTest]
    public async Task Setup()
    {
        // 1. Lade eine Szene, in der unsere Testobjekte leben können.
        //    Eine leere Szene oder eine einfache Level-Szene ist hier gut geeignet.
        _runner = ISceneRunner.Load("res://scenes/level_one.tscn", true);

        // 2. Erstelle eine echte Instanz der Klasse, die wir testen wollen.
        _spike = new Spike();
        _runner.Scene().AddChild(_spike);

        // 3. Erstelle eine echte Spieler-Instanz.
        //    HINWEIS: Dieser Test setzt voraus, dass deine Player-Klasse eine öffentliche
        //    Eigenschaft 'Health' hat, um die Lebenspunkte abzufragen.
        _player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        _runner.Scene().AddChild(_player);

        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    public async Task WhenPlayerEntersSpike_PlayerHealthShouldDecrease()
    {
        // Arrange: Hole die anfänglichen Lebenspunkte des Spielers.
        var initialHealth = PlayerStats.Instance.GetCurrentHealth();

        // Act: Simuliere, dass der Spieler den Spike berührt, indem wir das Signal am Area2D-Kind aussenden.
        var area2d = _spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();

        // Assert: Überprüfe, ob die Lebenspunkte des Spielers gesunken sind.
        var newHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(newHealth < initialHealth).IsTrue();
    }

    [TestCase]
    public async Task PlayerTakesRepeatedDamageWhileOnSpike()
    {
        var initialHealth = PlayerStats.Instance.GetCurrentHealth();
        var area2d = _spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();

        // Simuliere Zeit für mindestens zwei Timer-Timeouts (z.B. 2 Sekunden)
        await _runner.SimulateFrames(120, 16); // 120 Frames bei 16ms ≈ 2 Sekunden
        var newHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(newHealth < initialHealth).IsTrue();
    }

    [TestCase]
    public async Task TimerStopsWhenPlayerLeavesSpike()
    {
        var area2d = _spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();
        area2d.EmitSignal("body_exited", _player);
        await _runner.AwaitIdleFrame();
        var healthAfterExit = PlayerStats.Instance.GetCurrentHealth();
        await _runner.SimulateFrames(60, 16); // Simuliere 1 Sekunde
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(healthAfterExit);
    }

    [TestCase]
    public async Task TimerNotStartedForNonPlayer()
    {
        var area2d = _spike.GetNode<Area2D>("StaticBody2D/Area2D");
        var otherObject = new CharacterBody2D();
        _runner.Scene().AddChild(otherObject);
        var timer = _spike.GetNode<Timer>("StaticBody2D/Area2D/Timer");
        area2d.EmitSignal("body_entered", otherObject);
        await _runner.AwaitIdleFrame();
        AssertThat(timer.IsStopped()).IsTrue();
    }

    [TestCase]
    public async Task WhenBaseEnemyEntersSpike_BaseEnemyHealthShouldNotChange()
    {
        var baseEnemy = new BaseEnemy();
        _runner.Scene().AddChild(baseEnemy);
        var initialHealth = baseEnemy.GetCurrentHealth();

        var area2d = _spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", baseEnemy);
        await _runner.AwaitIdleFrame();

        var newHealth = baseEnemy.GetCurrentHealth();
        AssertThat(newHealth).IsEqual(initialHealth);
    }
}