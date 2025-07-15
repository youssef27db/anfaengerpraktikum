using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;

[TestSuite]
public class CheckpointTest
{
    private ISceneRunner _runner;
    private Checkpoint _checkpoint;
    private Player _player;

    [BeforeTest]
    public async Task Setup()
    {
        _runner = ISceneRunner.Load("res://scenes/level_one.tscn", true);
        _checkpoint = new Checkpoint();
        _runner.Scene().AddChild(_checkpoint);
        _player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        _runner.Scene().AddChild(_player);
        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    public void PlayerSetsSpawnpointOnCheckpointEnter()
    {
        _checkpoint.GlobalPosition = new Vector2(100, 200);
        PlayerStats.Instance.SetSpawnPoint(Vector2.Zero); // Reset vorher
        _checkpoint.Call("OnPlayerBodyEntered", _player);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(new Vector2(100, 200));
    }

    [TestCase]
    public void PlayerIsHealedAndStaminaRestoredOnCheckpoint()
    {
        PlayerStats.Instance.SetCurrentHealth(10);
        PlayerStats.Instance.SetStamina(5);
        _checkpoint.Call("OnPlayerBodyEntered", _player);
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(PlayerStats.Instance.GetMaxHealthPoints());
        AssertThat(PlayerStats.Instance.GetStamina()).IsEqual(PlayerStats.Instance.GetMaxStamina());
    }

    [TestCase]
    public void PlayerBloodVialsAreResetOnCheckpoint()
    {
        var bloodVials = _player.GetBloodVials();
        bloodVials.ResetUses(); // Setze auf 0
        _checkpoint.Call("OnPlayerBodyEntered", _player);
        AssertThat(bloodVials.GetCurrentUses()).IsEqual(bloodVials.GetMaxUses());
    }

    [TestCase]
    public void RespawnLevelTagIsSetOnCheckpoint()
    {
        var parent = new Node();
        parent.Name = "TestLevel";
        parent.AddChild(_checkpoint);
        _checkpoint.Call("OnPlayerBodyEntered", _player);
        AssertThat(PlayerStats.Instance.GetRespawnLevelTag()).IsEqual("TestLevel");
    }

    [TestCase]
    public void NoEffectWhenNonPlayerEntersCheckpoint()
    {
        var dummy = new Node2D();
        PlayerStats.Instance.SetSpawnPoint(Vector2.Zero);
        PlayerStats.Instance.SetCurrentHealth(10);
        PlayerStats.Instance.SetStamina(5);
        _checkpoint.Call("OnPlayerBodyEntered", dummy);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(Vector2.Zero);
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(10);
        AssertThat(PlayerStats.Instance.GetStamina()).IsEqual(5);
    }

    [TestCase]
    public void MultipleCheckpointsOverwriteSpawnpoint()
    {
        var checkpoint2 = new Checkpoint();
        _runner.Scene().AddChild(checkpoint2);
        _checkpoint.GlobalPosition = new Vector2(100, 200);
        checkpoint2.GlobalPosition = new Vector2(300, 400);
        _checkpoint.Call("OnPlayerBodyEntered", _player);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(new Vector2(100, 200));
        checkpoint2.Call("OnPlayerBodyEntered", _player);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(new Vector2(300, 400));
    }
} 