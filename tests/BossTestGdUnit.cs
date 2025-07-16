using Godot;
using System;
using GdUnit4;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;


[TestSuite]
public class BossTestGdUnit
{

    private Boss1 Boss;
    private ISceneRunner Runner;

    [BeforeTest]
    public async Task Setup()
    {

        Runner = ISceneRunner.Load("res://scenes/bossRoom.tscn");
        await Runner.AwaitIdleFrame();

        Boss = Runner.Scene().GetNode<Boss1>("enemies/Boss");
        // Runner.Scene().AddChild(Boss);

        Boss.Position = new Vector2(300, 288);
        Boss.StartPosition = new Vector2(300, 288);
        await Runner.AwaitIdleFrame();


    }

    public async Task Reset()
    {

        Boss.Position = new Vector2(300, 288);
        Boss.StartPosition = new Vector2(300, 288);
        Boss.TargetPosition = Vector2.Inf;
        Boss.Pursuing = false;
        Boss.Dead = false;
        await Runner.AwaitIdleFrame();

    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMovementIfDead()
    {
        await Reset();

        Vector2 InitialPosition = Boss.Position;
        Boss.TargetPosition = new Vector2(130, 288);
        Boss.Pursuing = true;
        Boss.Dead = true;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqual(Vector2.Zero);


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMovementIfNotPursuing()
    {
        await Reset();

        Vector2 InitialPosition = Boss.Position;
        Boss.TargetPosition = new Vector2(130, 288);
        Boss.Pursuing = false;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqual(Vector2.Zero);


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMovementIfNoTargetPosition()
    {
        await Reset();

        Vector2 InitialPosition = Boss.Position;
        Boss.TargetPosition = Vector2.Inf;
        Boss.Pursuing = true;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));


    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMovementToTargetPositionLeft()
    {
        await Reset();

        Vector2 InitialPosition = Boss.Position;
        Boss.TargetPosition = new Vector2(130, 288);
        Boss.StartPosition = new Vector2(130, 288);
        Boss.Pursuing = false;
        Boss.ReturnToStart = -1;
        await Runner.SimulateFrames(5, 100);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Left, new Vector2(0.1f, 0.1f));


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestMovementToTargetPositionRight()
    {
        await Reset();

        Vector2 InitialPosition = Boss.Position;
        Boss.TargetPosition = new Vector2(450, 288);
        Boss.StartPosition = new Vector2(450, 288);
        Boss.Pursuing = false;
        Boss.ReturnToStart = -1;
        await Runner.SimulateFrames(5, 100);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Right, new Vector2(0.1f, 0.1f));


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageIfDead()
    {

        await Reset();

        Boss.Dead = true;
        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(10, 10, Vector2.Zero, null);
        Boss.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(InitialHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageOnlyPhysical()
    {

        await Reset();

        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(30, 0, Vector2.Zero, null);
        Boss.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Boss.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));

    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageOnlyTrue()
    {

        await Reset();

        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(0, 30, Vector2.Zero, null);
        Boss.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Boss.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageOnlyPushRight()
    {

        await Reset();

        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(0, 0, new Vector2(10f, 0f), null);
        Boss.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(InitialHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Right, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageOnlyPushLeft()
    {

        await Reset();

        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(0, 0, new Vector2(-10f, 0f), null);
        Boss.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(InitialHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Left, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageCombined()
    {

        await Reset();

        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(30, 10, new Vector2(-10f, 0f), null);
        Boss.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Boss.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Left, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestTakeDamageTillDead()
    {

        await Reset();

        Vector2 InitialPosition = Boss.Position;
        float InitialHealth = Boss.CurrentHealthPoints;
        Damage Damage = new Damage(0, 400.0f, Vector2.Zero, null);
        Boss.TakeDamage(Damage);
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Boss.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Boss.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));
        Assertions.AssertThat(Boss.Dead).IsTrue();
    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task TestDie()
    {

        await Reset();

        Boss.Die();
        await Runner.AwaitIdleFrame();

        Assertions.AssertThat(Boss.Dead).IsTrue();
        Assertions.AssertThat(Boss.Velocity).IsEqual(Vector2.Zero);
        Assertions.AssertThat(Boss.MainCollision.Get(CollisionShape2D.PropertyName.Disabled)).IsTrue();

    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task TestStartGlowing()
    {
        await Reset();

        Boss.StartGlowing();
        await Runner.AwaitIdleFrame();

        Assertions.AssertThat(Boss.Sprite.Modulate).IsEqual(new Color(1.0f, 0.84f, 0.0f, 1.0f));
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestHPUnder50Percent()
    {
        await Reset();

        Boss.CurrentHealthPoints *= 0.49f;
        await Runner.AwaitIdleFrame();

        Assertions.AssertThat(Boss.Sprite.Modulate).IsEqual(new Color(1.0f, 0.84f, 0.0f, 1.0f));
        Assertions.AssertThat(Boss.EnemiesRevived).IsTrue();
        Assertions.AssertThat(Boss.Armor).IsEqual(60f);
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task TestRegenerateHealth()
    {
        Boss.CurrentHealthPoints = 350.0f;
        float Delta = 20f; 

        for (int i = 0; i < 5; i++)
        {
            Boss.HandleRegeneration(Delta);
        }
        await Runner.SimulateFrames(10, 100);
        
        Assertions.AssertThat(Boss.CurrentHealthPoints).IsEqual(400.0f);

        await Setup();
    }

}