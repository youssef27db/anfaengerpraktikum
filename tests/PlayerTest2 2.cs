using GdUnit4;
using Godot;
using System.Threading;
using System.Threading.Tasks;

// Testklasse für den Player

[TestSuite]
public class PlayerTest2
{
    private ISceneRunner _runner;
    private Player _player;
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    [BeforeTest]
    public async Task Setup()
    {
        _runner = ISceneRunner.Load("res://scenes/intro.tscn");
        await _runner.AwaitIdleFrame();

        Assertions.AssertThat(_runner).IsNotNull();

        _player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        _player.Name = "Player";
        _runner.Scene().AddChild(_player);
        _player.Position = new Vector2(-34, 10);

        Assertions.AssertThat(_player).IsNotNull();
        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestWMovement()
    {
        // Test for W
        await TestMovement(Key.W, Vector2.Up);

    }

    private async Task TestMovement(Key key, Vector2 expectedDirection) //KeyboardInput
    {
        await _semaphore.WaitAsync(); // wait for semaphore
    
        try{
        Vector2 initialPosition = _player.Position;   

        _runner.SimulateKeyPress(key);
        await _runner.SimulateFrames(5, 100);
        _runner.SimulateKeyRelease(key);
        await _runner.AwaitIdleFrame();

        Vector2 newPosition = _player.Position;

        Vector2 movementDirection = (initialPosition - newPosition).Normalized();

        Assertions.AssertVector(movementDirection).IsEqual(expectedDirection);
        }
        finally
        {
        _semaphore.Release(); // release the semaphore when task is done
        }
    }
}