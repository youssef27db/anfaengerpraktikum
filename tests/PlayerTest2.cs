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

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestAMovement()
    {
        // Test for A
        await TestMovementApprox(Key.A, Vector2.Left);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDMovement()
    {
        // Test for D
        await TestMovementApprox(Key.D, Vector2.Right);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithNoDirectionMovement()
    {
        // Test L-Shift for dash if no direction is pressed
        Player.Position = new Vector2(120, 288);

        await TestMovement(Key.Shift, Vector2.Zero);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithDirectionLeftMovement()
    {
        // Test L-Shift for dash if Left direction is pressed

        Runner.SimulateKeyPress(Key.A);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Left);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithDirectionRightMovement()
    {
        // Test L-Shift for dash if Right direction is pressed

        Runner.SimulateKeyPress(Key.D);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Right);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithDirectionUpMovement()
    {
        // Test L-Shift for dash if "W" direction is pressed

        Runner.SimulateKeyPress(Key.W);
        await Runner.SimulateFrames(1, 1);
        await TestMovementApprox(Key.Shift, Vector2.Up);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithLightAttackMovement()
    {
        // Test L-Shift for dash if light attack is pressed
        Player.Position = new Vector2(120, 288); // Reset position to avoid interference 

        Runner.SimulateMouseButtonPress(MouseButton.Left);
        await Runner.SimulateFrames(1, 100);
        await TestMovementApprox(Key.Shift, Vector2.Zero);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDashWithHeavyAttackMovement()
    {
        // Test L-Shift for dash if heavy attack is pressed
        Player.Position = new Vector2(120, 288); // Reset position to avoid interference

        Runner.SimulateMouseButtonPress(MouseButton.Right);
        await Runner.SimulateFrames(1, 100);
        await TestMovementApprox(Key.Shift, Vector2.Zero);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestHeavyAttack()
    {
        Player.Position = new Vector2(120, 288);
        // Test heavy attack with right mouse button

        Runner.SimulateMouseButtonPress(MouseButton.Right);
        await Runner.SimulateFrames(1, 100);

        Assertions.AssertThat(Player.IsAttacking()).IsTrue();

        await Setup(); // Reset player state after attack
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestLightAttack()
    {
        // Test light attack with right mouse button

        Player.Position = new Vector2(120, 288); // Reset position to avoid interference

        Runner.SimulateMouseButtonPress(MouseButton.Left);
        await Runner.SimulateFrames(1, 100);

        Assertions.AssertThat(Player.IsAttacking()).IsTrue();
        
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestBlock()
    {
        // Test block with space Key

        Runner.SimulateKeyPress(Key.Space);
        await Runner.SimulateFrames(5, 100);
        Assertions.AssertThat(Player.IsBlocking()).IsTrue();

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMaxHeal()
    {
        // Test if player has maximum health
        float InitialHealth = PlayerStats.Instance.GetCurrentHealth();

        PlayerStats.Instance.SetCurrentHealth(50.0f);
        Player.MaxHeal();
        await Runner.SimulateFrames(1, 100);
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(InitialHealth);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDMG()
    {
        // Test if player has maximum health
        Spike Spike = GD.Load<PackedScene>("res://scenes/spike.tscn").Instantiate<Spike>();
        Runner.Scene().AddChild(Spike);
        float InitialHealth = PlayerStats.Instance.GetCurrentHealth();

        Damage Damage = Spike.GetDamage();  // Get 10 damage from Spike
        Player.TakeDamage(Damage);

        await Runner.SimulateFrames(1, 1);
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(InitialHealth - 10.0f);
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDMGWhileBlocking()
    {
        // Test if player has maximum health
        BaseEnemy BaseEnemy = GD.Load<PackedScene>("res://scenes/base_enemy.tscn").Instantiate<BaseEnemy>();
        Runner.Scene().AddChild(BaseEnemy);
        PlayerStats.Instance.SetCurrentHealth(100.0f); // Set initial health

        Runner.SimulateKeyPress(Key.Space); // Simulate blocking
        await Runner.SimulateFrames(5, 100);
        Damage DMG = new Damage(50.0f, 0, Vector2.Zero, BaseEnemy);
        Player.TakeDamage(DMG);

        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(100.0f); // Health should remain the same due to blocking
        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestRegenerateStamina()
    {
        // Test stamina regeneration
        PlayerStats.Instance.SetStamina(50.0f); // Set initial stamina
        float StaminaRegenRate = 10f;
        float Delta = 0.5f; // Simulate half a second

        Player.RegenerateStamina(StaminaRegenRate, Delta);
        await Runner.SimulateFrames(5, 100);
        
        Assertions.AssertThat(PlayerStats.Instance.GetStamina()).IsEqual(100.0f); // Assuming max stamina is 100

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestUseBloodVial()
    {
        // Test using a blood vial
        PlayerStats.Instance.SetCurrentHealth(50.0f); // Set initial health

        await Runner.AwaitIdleFrame();
        Runner.SimulateKeyPress(Key.Q); // Simulate using blood vial
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(50.0f);

        await Setup();
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestUseBloodVialWithMaxHealth()
    {
        // Test using a blood vial when health is already at maximum
        PlayerStats.Instance.SetCurrentHealth(100.0f); // Set initial health

        await Runner.AwaitIdleFrame();
        Runner.SimulateKeyPress(Key.Q); // Simulate using blood vial
        Assertions.AssertThat(PlayerStats.Instance.GetCurrentHealth()).IsEqual(100.0f);

        await Setup();
    }

    private async Task TestMovement(Key Key, Vector2 EcpectedDirection) //KeyboardInput
    {
        await Semaphore.WaitAsync(); // wait for semaphore

        try
        {
            Vector2 InitialPosition = Player.Position;

            Runner.SimulateKeyPress(Key);
            await Runner.SimulateFrames(1, 100);
            Runner.SimulateKeyRelease(Key);
            await Runner.AwaitIdleFrame();

            Vector2 NewPosition = Player.Position;

            Vector2 MovementDirection = (NewPosition - InitialPosition).Normalized();

            Assertions.AssertThat(MovementDirection).IsEqual(EcpectedDirection);
        }
        finally
        {
            Semaphore.Release(); // release the semaphore when task is done
        }
    }

    private async Task TestMovementApprox(Key Key, Vector2 EcpectedDirection) //KeyboardInput
    {
        await Semaphore.WaitAsync(); // wait for semaphore

        try
        {
            Vector2 initialPosition = Player.Position;

            Runner.SimulateKeyPress(Key);
            await Runner.SimulateFrames(1, 100);
            Runner.SimulateKeyRelease(Key);
            await Runner.AwaitIdleFrame();

            Vector2 NewPosition = Player.Position;

            Vector2 MovementDirection = (NewPosition - initialPosition).Normalized();

            Assertions.AssertThat(MovementDirection).IsEqualApprox(EcpectedDirection, new Vector2(0.1f, 0.1f));
        }
        finally
        {
            Semaphore.Release(); // release the semaphore when task is done
        }
    }
}