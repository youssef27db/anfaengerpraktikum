using Godot;
using System;
using GdUnit4;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;


[TestSuite]
public class BaseEnemyTest
{

    private BaseEnemy Enemy;
    private ISceneRunner Runner;

    [BeforeTest]
    public async Task Setup()
    {

        Runner = ISceneRunner.Load("res://scenes/level1.tscn");
        await Runner.AwaitIdleFrame();

        Enemy = GD.Load<PackedScene>("res://scenes/base_enemy.tscn").Instantiate<BaseEnemy>();
        Runner.Scene().AddChild(Enemy);

        Enemy.Position = new Vector2(300, 288);
        Enemy.StartPosition = new Vector2(300, 288);
        await Runner.AwaitIdleFrame();


    }

    public async Task Reset()
    {

        Enemy.Position = new Vector2(300, 288);
        Enemy.StartPosition = new Vector2(300, 288);
        Enemy.TargetPosition = Vector2.Inf;
        Enemy.Pursuing = false;
        Enemy.Dead = false;
        await Runner.AwaitIdleFrame();

    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementIfDead()
    {
        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        Enemy.TargetPosition = new Vector2(130, 288);
        Enemy.Pursuing = true;
        Enemy.Dead = true;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqual(Vector2.Zero);


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementIfNotPursuing()
    {
        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        Enemy.TargetPosition = new Vector2(130, 288);
        Enemy.Pursuing = false;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqual(Vector2.Zero);


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementIfNoTargetPosition()
    {
        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        Enemy.TargetPosition = Vector2.Inf;
        Enemy.Pursuing = true;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));


    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementToTargetPositionLeft()
    {
        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        Enemy.TargetPosition = new Vector2(130, 288);
        Enemy.StartPosition = new Vector2(130, 288);
        Enemy.Pursuing = false;
        Enemy.ReturnToStart = -1;
        await Runner.SimulateFrames(5, 100);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Left, new Vector2(0.1f, 0.1f));


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementToTargetPositionRight()
    {
        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        Enemy.TargetPosition = new Vector2(450, 288);
        Enemy.StartPosition = new Vector2(450, 288);
        Enemy.Pursuing = false;
        Enemy.ReturnToStart = -1;
        await Runner.SimulateFrames(5, 100);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Right, new Vector2(0.1f, 0.1f));


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageIfDead()
    {

        await Reset();

        Enemy.Dead = true;
        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(10, 10, Vector2.Zero, null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(InitialHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageOnlyPhysical()
    {

        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(30, 0, Vector2.Zero, null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Enemy.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));

    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageOnlyTrue()
    {

        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(0, 30, Vector2.Zero, null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Enemy.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageOnlyPushRight()
    {

        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(0, 0, new Vector2(10f, 0f), null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(InitialHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Right, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageOnlyPushLeft()
    {

        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(0, 0, new Vector2(-10f, 0f), null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(InitialHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Left, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageCombined()
    {

        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(30, 10, new Vector2(-10f, 0f), null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Enemy.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Left, new Vector2(0.1f, 0.1f));

    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkTakeDamageTillDead()
    {

        await Reset();

        Vector2 InitialPosition = Enemy.Position;
        float InitialHealth = Enemy.CurrentHealthPoints;
        Damage Damage = new Damage(0, 200, Vector2.Zero, null);
        Enemy.TakeDamage(Damage);
        await Runner.AwaitIdleFrame();
        float CalculatedHealth = InitialHealth - Damage.GetPhysicalDMG() * (1 - Enemy.Armor / 100.0f) - Damage.GetTrueDMG();
        Assertions.AssertThat(Enemy.CurrentHealthPoints).IsEqual(CalculatedHealth);
        Vector2 MovementDirection = (Enemy.Position - InitialPosition).Normalized();
        Assertions.AssertThat(MovementDirection).IsEqualApprox(Vector2.Zero, new Vector2(0.1f, 0.1f));
        Assertions.AssertThat(Enemy.Dead).IsTrue();

    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task checkDie()
    {

        await Reset();

        Enemy.Die();
        await Runner.AwaitIdleFrame();

        Assertions.AssertThat(Enemy.Dead).IsTrue();
        Assertions.AssertThat(Enemy.Velocity).IsEqual(Vector2.Zero);
        Assertions.AssertThat(Enemy.MainCollision.Get(CollisionShape2D.PropertyName.Disabled)).IsTrue();

    }

}