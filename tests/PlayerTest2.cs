using GdUnit4;
using Godot;
using NUnit.Framework.Internal;
using System.Threading;
using System.Threading.Tasks;

// Testklasse für den Player

[TestSuite]
public class PlayerTest2
{
    private ISceneRunner Runner;
    private Player Player;
    private SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

    [BeforeTest]
    public async Task Setup()
    {
        Runner = ISceneRunner.Load("res://scenes/level1.tscn");
        await Runner.AwaitIdleFrame();

        Assertions.AssertThat(Runner).IsNotNull();

        Player = GD.Load<PackedScene>("res://scenes/player.tscn").Instantiate<Player>();
        Player.Name = "Player";
        Runner.Scene().AddChild(Player);
        Player.Position = new Vector2(150, 288);

        Assertions.AssertThat(Player).IsNotNull();
        await Runner.AwaitIdleFrame();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestWMovement()
    {
        // Test for W
        await TestMovement(Key.W, Vector2.Up);

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestAMovement()
    {
        // Test for A
        await TestMovementApprox(Key.A, Vector2.Left);

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDMovement()
    {
        // Test for D
        await TestMovementApprox(Key.D, Vector2.Right);

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithNoDirectionMovement()
    {
        // Test L-Shift for dash if no direction is pressed
        await TestMovementApprox(Key.Shift, Vector2.Zero);
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithDirectionLeftMovement()
    {
        // Test L-Shift for dash if Left direction is pressed

        Runner.SimulateKeyPress(Key.A);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Left);


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithDirectionRightMovement()
    {
        // Test L-Shift for dash if Right direction is pressed

        Runner.SimulateKeyPress(Key.D);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Right);
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithDirectionUpMovement()
    {
        // Test L-Shift for dash if "W" direction is pressed

        Runner.SimulateKeyPress(Key.W);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Up);
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithLightAttackMovement()
    {
        // Test L-Shift for dash if light attack is pressed
        Player.Position = new Vector2(150, 288); // Reset position to avoid interference 

        Runner.SimulateMouseButtonPress(MouseButton.Left);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Zero);
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithHeavyAttackMovement()
    {
        // Test L-Shift for dash if heavy attack is pressed
        Player.Position = new Vector2(120, 288); // Reset position to avoid interference

        Runner.SimulateMouseButtonPress(MouseButton.Right);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Zero);
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestHeavyAttack()
    {
        // Test heavy attack with right mouse button

        Runner.SimulateMouseButtonPress(MouseButton.Right);
        await Runner.SimulateFrames(1, 1);
        
        Assertions.AssertThat(Player.IsAttacking()).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestLightAttack()
    {
        // Test light attack with right mouse button

        Runner.SimulateMouseButtonPress(MouseButton.Left);
        await Runner.SimulateFrames(1, 1);
        
        Assertions.AssertThat(Player.IsAttacking()).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestBlock()
    {
        // Test block with space key

        Runner.SimulateKeyPress(Key.Space);
        await Runner.SimulateFrames(1, 1);
        
        Assertions.AssertThat(Player.IsBlocking()).IsTrue();
    }

    private async Task TestMovement(Key key, Vector2 expectedDirection) //KeyboardInput
    {
        await Semaphore.WaitAsync(); // wait for semaphore

        try
        {
            Vector2 initialPosition = Player.Position;

            Runner.SimulateKeyPress(key);
            await Runner.SimulateFrames(1, 100);
            Runner.SimulateKeyRelease(key);
            await Runner.AwaitIdleFrame();

            Vector2 newPosition = Player.Position;

            Vector2 movementDirection = (newPosition - initialPosition).Normalized();

            Assertions.AssertThat(movementDirection).IsEqual(expectedDirection);
        }
        finally
        {
            Semaphore.Release(); // release the semaphore when task is done
        }
    }
    
    private async Task TestMovementApprox(Key key, Vector2 expectedDirection) //KeyboardInput
    {
        await Semaphore.WaitAsync(); // wait for semaphore

        try
        {
            Vector2 initialPosition = Player.Position;

            Runner.SimulateKeyPress(key);
            await Runner.SimulateFrames(1, 100);
            Runner.SimulateKeyRelease(key);
            await Runner.AwaitIdleFrame();

            Vector2 newPosition = Player.Position;

            Vector2 movementDirection = (newPosition - initialPosition).Normalized();

            Assertions.AssertThat(movementDirection).IsEqualApprox(expectedDirection, new Vector2(0.1f, 0.1f));
        }
        finally
        {
            Semaphore.Release(); // release the semaphore when task is done
        }
    }
}