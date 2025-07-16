using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;


[TestSuite]
public class SpikeTest
{
    private ISceneRunner Runner;
    private Spike Spike;
    private Player Player;

    [BeforeTest]
    public async Task Setup()
    {
        Runner = ISceneRunner.Load("res://scenes/level1.tscn");
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Runner).IsNotNull();
        // Laedt die Spike-Szene und instanziiert sie
        Spike = GD.Load<PackedScene>("res://scenes/spike.tscn").Instantiate<Spike>();
        
        // Laedt die Spieler-Szene und instanziiert sie
        Player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        Player.Name = "Player"; // Wichtig fuer die Erkennung im Spiel
        Runner.Scene().AddChild(Player);

        Runner.Scene().AddChild(Spike);

        // Initialisiert den ISceneRunner mit der neuen Szene
    // Runner = ISceneRunner.Load(scene, true);
        
        // Warte einen Frame, damit die Szene vollstaendig geladen ist
  
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task WhenPlayerEntersSpikePlayerHealthShouldDecrease()
    {
        // Test if player has maximum health
        Runner.Scene().AddChild(Spike);
        float initialHealth = PlayerStats.Instance.GetCurrentHealth();

        Damage Damage = Spike.GetDamage();  // Get 10 damage from Spike
        Player.TakeDamage(Damage);

        await Runner.SimulateFrames(1, 1);
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(initialHealth - 10.0f);
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
        Player.GlobalPosition = Spike.GlobalPosition;

        // Simuliere das Betreten des Spikes
        var area2d = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, Player);
        
        // Warte auf den ersten Schaden
        await Runner.AwaitIdleFrame();
        
        // Simuliere eine Verzoegerung, um wiederholten Schaden zu ermoeglichen
        await Runner.SimulateFrames(1100);

        // Ueberpruefe, ob die Gesundheit des Spielers weiter reduziert wurde
        var newHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(initialHealth - Spike.GetDamage().GetTrueDMG()).IsGreater(newHealth);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TimerStopsWhenPlayerLeavesSpike()
    {
        // Setze die Startgesundheit und positioniere den Spieler
        PlayerStats.Instance.SetCurrentHealth(100.0f);
        Player.GlobalPosition = Spike.GlobalPosition;

        // Simuliere das Betreten und sofortige Verlassen des Spikes
        var area2d = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, Player);
        await Runner.AwaitIdleFrame();
        
        area2d.EmitSignal(Area2D.SignalName.BodyExited, Player);
        await Runner.AwaitIdleFrame();

        // Speichere die Gesundheit nach dem Verlassen
        var healthAfterExit = PlayerStats.Instance.GetCurrentHealth();

        // Warte eine Weile, um sicherzustellen, dass kein weiterer Schaden auftritt
        await Runner.SimulateFrames(2000);

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
        Runner.Scene().AddChild(otherObject);

        // Hole den Timer aus dem Spike
        var timer = Spike.GetNode<Timer>("StaticBody2D/Area2D/Timer");
        
        // Simuliere, dass das andere Objekt den Spike betritt
        var area2d = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, otherObject);
        await Runner.AwaitIdleFrame();

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
        Runner.Scene().AddChild(baseEnemy);
        var initialHealth = baseEnemy.CurrentHealthPoints;

        // Simuliere, dass der Gegner den Spike betritt
        var area2d = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal(Area2D.SignalName.BodyEntered, baseEnemy);
        await Runner.AwaitIdleFrame();

        // Ueberpruefe, ob die Gesundheit des Gegners unveraendert ist
        AssertThat(baseEnemy.CurrentHealthPoints).IsEqual(initialHealth);
        await Setup();
    }
}
