using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;

[TestSuite]
public class SpikeTest
{
    private ISceneRunner _runner;
    private Spike _spike;
    private Player _player;

    [BeforeTest]
    public async Task Setup()
    {
        _runner = ISceneRunner.Load("res://scenes/level_one.tscn", true);

        _spike = new Spike();
        _runner.Scene().AddChild(_spike);

        _player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        _runner.Scene().AddChild(_player);

        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    public async Task WhenPlayerEntersSpike_PlayerHealthShouldDecrease()
    {
        var initialHealth = PlayerStats.Instance.GetCurrentHealth();

        var area2d = _spike.GetNode<Area2D>("StaticBody2D/Area2D");
        area2d.EmitSignal("body_entered", _player);
        await _runner.AwaitIdleFrame();

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

        await _runner.SimulateFrames(120, 16);
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
        await _runner.SimulateFrames(60, 16);
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