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

        Enemy.Position = new Vector2(150, 288);
        Enemy.StartPosition = new Vector2(150, 288);
        await Runner.AwaitIdleFrame();


    }

    public async Task Reset()
    {

        Enemy.Position = new Vector2(150, 288);
        Enemy.StartPosition = new Vector2(150, 288);
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

        Enemy.TargetPosition = new Vector2(130, 288);
        Enemy.Pursuing = true;
        Enemy.Dead = true;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Enemy.Position).IsEqual(new Vector2(150, 288));


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementIfNotPursuing()
    {
        await Reset();

        Enemy.TargetPosition = new Vector2(130, 288);
        Enemy.Pursuing = false;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Enemy.Position).IsEqual(new Vector2(150, 288));


    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementIfNoTargetPosition()
    {
        await Reset();

        Enemy.TargetPosition = Vector2.Inf;
        Enemy.Pursuing = true;
        await Runner.SimulateFrames(5, 100);
        await Runner.AwaitIdleFrame();
        Assertions.AssertThat(Enemy.Position).IsEqualApprox(new Vector2(150, 288), new Vector2(0.1f,0.1f));


    }


    [TestCase]
    [RequireGodotRuntime]
    public async Task checkMovementToTargetPosition()
    {
        await Reset();

        Enemy.TargetPosition = new Vector2(130, 288);
        Enemy.StartPosition = new Vector2(130, 288);
        Enemy.Pursuing = false;
        Enemy.ReturnToStart = -1;
        await Runner.SimulateFrames(10, 100);
        Assertions.AssertThat(Enemy.TargetPosition).IsEqual(new Vector2(130, 288));
        Assertions.AssertThat(Enemy.Position).IsEqualApprox(new Vector2(138, 288), new Vector2(1f,1f));


    }


}