using Godot;

namespace GdMUT
{
    public class PlayerTests
    {
#if TOOLS
        private static Player SetupPlayer()
        {
            Player player = new Player();

            // Sprite2D hinzufügen
            var sprite = new Sprite2D();
            sprite.Name = "Sprite2D";
            player.AddChild(sprite);

            // SwordHit und SwordCollision hinzufügen
            var swordHit = new Node2D();
            swordHit.Name = "SwordHit";
            sprite.AddChild(swordHit);

            var swordCollision = new CollisionShape2D();
            swordCollision.Name = "SwordCollision";
            swordHit.AddChild(swordCollision);

            // AnimationPlayer hinzufügen
            var animationPlayer = new AnimationPlayer();
            animationPlayer.Name = "AnimationPlayer";
            player.AddChild(animationPlayer);

            // PlayerHitbox hinzufügen
            var playerHitbox = new CollisionShape2D();
            playerHitbox.Name = "PlayerHitbox";
            player.AddChild(playerHitbox);

            // DashTimer hinzufügen
            var dashTimer = new Timer();
            dashTimer.Name = "DashTimer";
            player.AddChild(dashTimer);

            // DashEffect hinzufügen
            var dashEffect = new Timer();
            dashEffect.Name = "DashEffect";
            player.AddChild(dashEffect);

            // _Ready() aufrufen, um den Spieler zu initialisieren
            player._Ready();

            return player;
        }

        // Test, ob sich der Spieler nach rechts bewegen kann
        [CSTestFunction]
        public static Result PlayerCanMoveRight()
        {
            // Spieler mit Setup-Methode initialisieren
            Player player = SetupPlayer();
            player.Velocity = Vector2.Zero;

            // Bewegung nach links simulieren
            Input.ActionPress("ui_left");
            player._PhysicsProcess(0.016f); // Simuliere einen Frame
            Input.ActionRelease("ui_left");
            // Test: Velocity X sollte kleiner als 0 sein
            return player.Velocity.X < 0 ? Result.Success : Result.Failure;
        }

        [CSTestFunction]
        public static Result PlayerCanMoveLeft()
        {
            // Spieler initialisieren
            Player player = SetupPlayer();
            player.Velocity = Vector2.Zero;

            // Bewegung nach links simulieren
            Input.ActionPress("ui_left");
            player._PhysicsProcess(0.016f); // Simuliere einen Frame bei 60 FPS
            Input.ActionRelease("ui_left");

            // Test: Velocity X sollte kleiner als 0 sein
            return player.Velocity.X < 0 ? Result.Success : Result.Failure;
        }

        [CSTestFunction]
        public static Result PlayerCanJump()
        {
            // Spieler initialisieren
            Player player = SetupPlayer();
            player.Velocity = Vector2.Zero;

            // Springen simulieren
            Input.ActionPress("ui_up");
            player._PhysicsProcess(0.016f); // Simuliere einen Frame bei 60 FPS
            Input.ActionRelease("ui_up");

            // Test: Velocity Y sollte kleiner als 0 sein (nach oben gerichtet)
            return player.Velocity.Y < 0 ? Result.Success : Result.Failure;
        }
#endif
    }
}
