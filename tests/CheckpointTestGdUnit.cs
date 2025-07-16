using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;

[TestSuite]
public class CheckpointTestGdUnit
{
    private ISceneRunner Runner;
    private Checkpoint Checkpoint;
    private Player Player;

    [BeforeTest]
    public async Task Setup()
    {
        Runner = ISceneRunner.Load("res://scenes/level_one.tscn", true);
        Checkpoint = new Checkpoint();
        Runner.Scene().AddChild(Checkpoint);
        Player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        Runner.Scene().AddChild(Player);
        var BloodVial = new BloodVial();
        Player.AddChild(BloodVial);
        var BloodVialsField = Player.GetType().GetField("BloodVials", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (BloodVialsField != null)
            BloodVialsField.SetValue(Player, BloodVial);
        await Runner.AwaitIdleFrame();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestPlayerSetsSpawnpointOnCheckpointEnter()
    {
        // Test if player spawnpoint is set correctly when entering checkpoint
        Checkpoint.GlobalPosition = new Vector2(100, 200);
        PlayerStats.Instance.SetSpawnPoint(Vector2.Zero);
        Checkpoint.Call("OnPlayerBodyEntered", Player);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(new Vector2(100, 200));
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestPlayerIsHealedAndStaminaRestoredOnCheckpoint()
    {
        // Test if player health and stamina are restored when entering checkpoint
        PlayerStats.Instance.SetCurrentHealth(10);
        PlayerStats.Instance.SetStamina(5);
        Checkpoint.Call("OnPlayerBodyEntered", Player);
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(PlayerStats.Instance.GetMaxHealthPoints());
        AssertThat(PlayerStats.Instance.GetStamina()).IsEqual(PlayerStats.Instance.GetMaxStamina());
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestPlayerBloodVialsAreResetOnCheckpoint()
    {
        // Test if player blood vials are reset when entering checkpoint
        var BloodVials = Player.GetBloodVials();
        BloodVials.ResetUses();
        Checkpoint.Call("OnPlayerBodyEntered", Player);
        AssertThat(PlayerStats.Instance.GetBVCurrentUses()).IsEqual(PlayerStats.Instance.GetBVMaxUses());
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestNoEffectWhenNonPlayerEntersCheckpoint()
    {
        // Test if entering checkpoint with non-player object has no effect
        var dummy = new Node2D();
        PlayerStats.Instance.SetSpawnPoint(Vector2.Zero);
        PlayerStats.Instance.SetCurrentHealth(10);
        PlayerStats.Instance.SetStamina(5);
        Runner.Scene().AddChild(dummy);
        Checkpoint.Call("OnPlayerBodyEntered", dummy);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(Vector2.Zero);
        AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(10);
        AssertThat(PlayerStats.Instance.GetStamina()).IsEqual(5);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMultipleCheckpointsOverwriteSpawnpoint()
    {
        // Test if multiple checkpoints overwrite player spawnpoint correctly
        var Checkpoint2 = new Checkpoint();
        Runner.Scene().AddChild(Checkpoint2);
        Checkpoint.GlobalPosition = new Vector2(100, 200);
        Checkpoint2.GlobalPosition = new Vector2(300, 400);
        Checkpoint.Call("OnPlayerBodyEntered", Player);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(new Vector2(100, 200));
        Checkpoint2.Call("OnPlayerBodyEntered", Player);
        AssertThat(PlayerStats.Instance.GetSpawnPoint()).IsEqual(new Vector2(300, 400));
        await Setup();
    }
} 