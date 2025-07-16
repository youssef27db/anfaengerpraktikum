using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;


[TestSuite]
public class SpikeTestGdUnit
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
    public async Task TestWhenPlayerEntersSpikePlayerHealthShouldDecrease()
    {
        // Test if player has maximum health
        Runner.Scene().AddChild(Spike);
        float InitialHealth = PlayerStats.Instance.GetCurrentHealth();

        await Runner.SimulateFrames(1, 1);
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(InitialHealth - Spike.GetDamage().GetTrueDMG());
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestPlayerTakesRepeatedDamageWhileOnSpike()
    {
        // Setze die Startgesundheit des Spielers
        PlayerStats.Instance.SetCurrentHealth(100.0f);
        var InitialHealth = PlayerStats.Instance.GetCurrentHealth();

        // Positioniere den Spieler auf dem Spike
        Player.GlobalPosition = Spike.GlobalPosition;

        // Simuliere das Betreten des Spikes
        var Area2D = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        Runner.Scene().AddChild(Area2D);
        Area2D.EmitSignal(Area2D.SignalName.BodyEntered, Player);
        
        
        // Warte auf den ersten Schaden
        await Runner.AwaitIdleFrame();
        
        // Simuliere eine Verzoegerung, um wiederholten Schaden zu ermoeglichen
        await Runner.SimulateFrames(1100);

        // Ueberpruefe, ob die Gesundheit des Spielers weiter reduziert wurde
        var NewHealth = PlayerStats.Instance.GetCurrentHealth();
        AssertThat(InitialHealth - Spike.GetDamage().GetTrueDMG()).IsGreater(NewHealth);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTimerStopsWhenPlayerLeavesSpike()
    {
        // Setze die Startgesundheit und positioniere den Spieler
        PlayerStats.Instance.SetCurrentHealth(100.0f);
        Player.Position = Spike.Position;

        // Simuliere das Betreten und sofortige Verlassen des Spikes
        var Area2D = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        Runner.Scene().AddChild(Area2D);

        Area2D.EmitSignal(Area2D.SignalName.BodyEntered, Player);
        await Runner.AwaitIdleFrame();
        
        Player.Position = new Vector2(1000, 1000); // Bewege den Spieler weg vom Spike

        Area2D.EmitSignal(Area2D.SignalName.BodyExited, Player);
        await Runner.AwaitIdleFrame();

        // Speichere die Gesundheit nach dem Verlassen
        var HealthAfterExit = PlayerStats.Instance.GetCurrentHealth();

        // Warte eine Weile, um sicherzustellen, dass kein weiterer Schaden auftritt
        await Runner.SimulateFrames(1, 1);

        // Ueberpruefe, ob die Gesundheit unveraendert geblieben ist
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(HealthAfterExit);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTimerNotStartedForNonPlayer()
    {
        // Erstelle ein anderes Objekt, das kein Spieler ist
        var OtherObject = new CharacterBody2D();
        Runner.Scene().AddChild(OtherObject);

        // Hole den Timer aus dem Spike
        var Timer = Spike.GetNode<Timer>("StaticBody2D/Area2D/Timer");
        
        // Simuliere, dass das andere Objekt den Spike betritt
        var Area2D = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        Area2D.EmitSignal(Area2D.SignalName.BodyEntered, OtherObject);
        await Runner.AwaitIdleFrame();

        // Ueberpruefe, ob der Timer nicht gestartet wurde
        AssertThat(Timer.IsStopped()).IsTrue();
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestWhenBaseEnemyEntersSpikeBaseEnemyHealthShouldNotChange()
    {
        // Erstelle einen Gegner und setze seine Gesundheit
        var BaseEnemy = new BaseEnemy();
        BaseEnemy.CurrentHealthPoints = 50.0f;
        Runner.Scene().AddChild(BaseEnemy);
        var InitialHealth = BaseEnemy.CurrentHealthPoints;

        // Simuliere, dass der Gegner den Spike betritt
        var Area2D = Spike.GetNode<Area2D>("StaticBody2D/Area2D");
        Area2D.EmitSignal(Area2D.SignalName.BodyEntered, BaseEnemy);
        await Runner.AwaitIdleFrame();

        // Ueberpruefe, ob die Gesundheit des Gegners unveraendert ist
        AssertThat(BaseEnemy.CurrentHealthPoints).IsEqual(InitialHealth);
        await Setup();
    }
}
