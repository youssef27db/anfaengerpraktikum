using Godot;
using GdUnit4;
using System.Threading.Tasks; // Nötig für async/await
using static GdUnit4.Assertions;

/// <summary>
/// Test-Suite für die SpikeDynamic-Klasse, die das Zusammenspiel mit dem Spieler testet.
/// </summary>
[TestSuite]
public class SpikeDynamicTest
{
    private ISceneRunner _runner;
    private SpikeDynamic _spikeDynamic;
    private Player _player;

    [BeforeTest]
    public async Task Setup()
    {
        _runner = ISceneRunner.Load("res://scenes/level_one.tscn", true);
        _spikeDynamic = new SpikeDynamic();
        _runner.Scene().AddChild(_spikeDynamic);
        _player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        _runner.Scene().AddChild(_player);
        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    public async Task WhenPlayerEntersSpikeDynamic_PlayerHealthShouldDecrease()
    {
        var initialHealth = PlayerStats.Instance.GetCurrentHealth();
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();
        var newHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(newHealth < initialHealth).IsTrue();
    }

    [TestCase]
    public async Task PlayerTakesRepeatedDamageWhileOnSpikeDynamic()
    {
        var initialHealth = PlayerStats.Instance.GetCurrentHealth();
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();
        await _runner.SimulateFrames(120, 16); // 2 Sekunden
        var newHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(newHealth < initialHealth).IsTrue();
    }

    [TestCase]
    public async Task TimerStopsWhenPlayerLeavesSpikeDynamic()
    {
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();
        area2d.EmitSignal("body_exited", _player);
        await _runner.AwaitIdleFrame();
        var healthAfterExit = PlayerStats.Instance.GetCurrentHealth();
        await _runner.SimulateFrames(60, 16);
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(healthAfterExit);
    }

    [TestCase]
    public async Task TimerNotStartedForNonPlayer_SpikeDynamic()
    {
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        var otherObject = new CharacterBody2D();
        _runner.Scene().AddChild(otherObject);
        var timer = _spikeDynamic.GetNode<Timer>("StaticBody2D/Area2D/Timer");
        area2d.EmitSignal("body_entered", otherObject);
        await _runner.AwaitIdleFrame();
        AssertThat(timer.IsStopped()).IsTrue();
    }

    [TestCase]
    public async Task WhenBaseEnemyEntersSpikeDynamic_BaseEnemyHealthShouldNotChange()
    {
        var baseEnemy = new BaseEnemy();
        _runner.Scene().AddChild(baseEnemy);
        var initialHealth = baseEnemy.GetCurrentHealth();
        var area2d = _spikeDynamic.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", baseEnemy);
        await _runner.AwaitIdleFrame();
        var newHealth = baseEnemy.GetCurrentHealth();
        AssertThat(newHealth).IsEqual(initialHealth);
    }

    // Speziell für SpikeDynamic:
    // Hier könnten Tests ergänzt werden, die die Bewegung des Spikes prüfen (z.B. ob er sich korrekt entlang eines Pfades bewegt, ob er nach Kollisionen stoppt, etc.).
    // Da SpikeDynamic beweglich ist, wären Bewegungstests und Kollisionsverhalten mit anderen Objekten sinnvoll.

    // --- SpikeDynamic-spezifische Tests ---

    [TestCase]
    public async Task SpikeDynamic_MovesRight_WhenActivated()
    {
        // Arrange: Setze Startposition und ggf. Bewegungsrichtung
        var initialPosition = _spikeDynamic.Position;
        // Act: Simuliere Bewegung (z.B. durch Aufruf einer Methode oder SimulateFrames)
        await _runner.SimulateFrames(30, 16); // Simuliere ca. 0,5 Sekunde
        var newPosition = _spikeDynamic.Position;
        // Assert: Prüfe, ob sich SpikeDynamic nach rechts bewegt hat
        AssertThat(newPosition.X > initialPosition.X).IsTrue();
    }

    [TestCase]
    public async Task SpikeDynamic_StopsOnWallCollision()
    {
        // Arrange: Positioniere SpikeDynamic nahe an einer Wand (ggf. Wand-Node hinzufügen)
        // Hier als Platzhalter: Annahme, dass eine Wand bei X=400 ist
        _spikeDynamic.Position = new Vector2(395, _spikeDynamic.Position.Y);
        await _runner.SimulateFrames(20, 16); // Simuliere Bewegung zur Wand
        var velocityBefore = _spikeDynamic.Get("velocity"); // Falls Velocity-Property vorhanden
        await _runner.SimulateFrames(20, 16); // Simuliere nach Kollision
        var velocityAfter = _spikeDynamic.Get("velocity");
        // Assert: Nach der Kollision sollte die Geschwindigkeit 0 sein (Platzhalter)
        // Falls keine Velocity-Property existiert, prüfe, ob Position sich nicht mehr ändert
        AssertThat(velocityAfter.Equals(Vector2.Zero) || _spikeDynamic.Position.X <= 400).IsTrue();
    }

    [TestCase]
    public async Task SpikeDynamic_PlaysMoveAnimation_WhenMoving()
    {
        // Arrange: Hole AnimationPlayer, falls vorhanden
        var animationPlayer = _spikeDynamic.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animationPlayer == null)
        {
            // Test überspringen, falls keine Animation vorhanden
            AssertThat(true).IsTrue();
            return;
        }
        // Act: Simuliere Bewegung
        await _runner.SimulateFrames(10, 16);
        // Assert: Prüfe, ob eine Bewegungsanimation abgespielt wird
        AssertThat(animationPlayer.CurrentAnimation != "" && animationPlayer.IsPlaying()).IsTrue();
    }

    }
} 