using Godot;
using System;

public partial class Player : CharacterBody2D
{
    private const float SPEED = 100f;
    private const float JUMP_VELOCITY = -300f;

    // Variablen für den Doppelsprung
    private int JumpMax = 2;
    private int JumpCount = 0;

    // Variablen für den Dash
    private Vector2 DashDirection = Vector2.Zero;
    private float DashSpeed = 300f;
    private bool IsDashing = false;
    private bool CanDash = true;
    private float DashTrailInterval = 0.05f;
    private float DashTrailTimer = 0f;

    // Referenzen zu den Knoten
    private AnimationPlayer AnimationPlayer;
    private Sprite2D Sprite;
    private Timer DashEffect;
    private Timer DashTimer;
    private CollisionShape2D SwordCollision;
    private CollisionShape2D PlayerHitbox;

    //Variable um Hitbox des Players zu platzieren
    private Vector2 HauptHitbox;

    public override void _Ready() {
        // Knoten in der Szene finden und zuweisen
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = GetNode<Sprite2D>("Sprite2D");
        DashEffect = GetNode<Timer>("DashEffect");
        DashTimer = GetNode<Timer>("DashTimer");
        SwordCollision = GetNode<CollisionShape2D>("Sprite2D/SwordHit/SwordCollision");
        PlayerHitbox = GetNode<CollisionShape2D>("PlayerHitbox");
        HauptHitbox = PlayerHitbox.Position;
    }

    public override void _PhysicsProcess(double DeltaTime) {
        // Gravitation hinzufügen, wenn der Charakter nicht am Boden ist
        if (!IsOnFloor()) {
            Velocity += GetGravity() * (float)DeltaTime;
        } else {
            CanDash = true; // Dash wird zurückgesetzt, wenn der Charakter am Boden ist
        }

        HandleJump();
        HandleMovement(DeltaTime);
        MoveAndSlide();
        UpdateAnimations();
    }

    private void HandleJump() {
        // Sprungzähler zurücksetzen, wenn der Charakter am Boden ist
        if (JumpCount != 0 && IsOnFloor()) {
            JumpCount = 0;
        }

        // Überprüfen, ob der Sprung-Button gedrückt wurde und der Charakter noch Sprünge übrig hat
        if (Input.IsActionJustPressed("ui_up") && JumpCount < JumpMax) {
            Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);
            JumpCount += 1;
        }
    }

    private void HandleMovement(double DeltaTime) {
        // Bewegungsrichtung bestimmen (links/rechts)
        Vector2 direction = new Vector2(Input.GetAxis("ui_left", "ui_right"), Input.GetAxis("ui_up", "ui_down")).Normalized();

        // Sprite umdrehen basierend auf der Bewegungsrichtung
        if (direction.X < 0) {
            Sprite.FlipH = true;
            SwordCollision.Position = new Vector2(-Mathf.Abs(SwordCollision.Position.X), SwordCollision.Position.Y);    //SwordCollition umdrehen
            PlayerHitbox.Position = new Vector2(Sprite.Position.X * 1.8f, PlayerHitbox.Position.Y);
        } else if (direction.X > 0) {
            //Hauptsprites/Collision wieder in die Ursprungsposition bringen
            Sprite.FlipH = false;
            SwordCollision.Position = new Vector2(Mathf.Abs(SwordCollision.Position.X), SwordCollision.Position.Y);
            PlayerHitbox.Position = HauptHitbox;
        }


        float currentSpeed = SPEED;

        // Überprüfen, ob der Spieler gerade angreift, und Geschwindigkeit reduzieren
        if (AnimationPlayer.CurrentAnimation == "light_attack") {
            currentSpeed *= 0.5f;
        }else if (AnimationPlayer.CurrentAnimation == "heavy_attack"){
            currentSpeed *= 0.15f;
        }

        // Dash-Verarbeitung
        if (IsDashing) {
            DashInProgress(DeltaTime);
        } else {
            // Normale Bewegung verarbeiten, wenn kein Dash aktiv ist
            if (direction != Vector2.Zero) {
                Velocity = new Vector2(direction.X * currentSpeed, Velocity.Y);
            } else {
                Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
            }

            // Überprüfen, ob der Dash-Button gedrückt wurde und Dash möglich ist
            if (Input.IsActionJustPressed("dash") && direction != Vector2.Zero && CanDash && direction != Vector2.Up) {
                DashDirection = direction;
                StartDash();
            }
        }
    }

    // Funktion, die den Dash-Prozess startet
    private void StartDash() {
        IsDashing = true;
        CanDash = false;
        DashTimer.Timeout += StopDash;
        DashTimer.Start();
        DashEffect.Start();
        DashTrailTimer = 0f; // Dash-Trail-Timer zurücksetzen
    }

    // Funktion, die während des Dashes ausgeführt wird
    private void DashInProgress(double DeltaTime) {
        // Charakter bewegt sich in die Dash-Richtung mit Dash-Geschwindigkeit
        Velocity = DashDirection * DashSpeed;

        // Dash-Trail bei Intervallen erstellen
        DashTrailTimer -= (float)DeltaTime;
        if (DashTrailTimer <= 0f) {
            CreateDashEffect();
            DashTrailTimer = DashTrailInterval;
        }
    }

    // Funktion zum Erstellen des Dash-Effekts (Nachbild des Spielers)
    private void CreateDashEffect() {
        Sprite2D PlayerCopyNode = (Sprite2D)Sprite.Duplicate();
        GetParent().AddChild(PlayerCopyNode);

        CollisionShape2D SwordCollisionCopy = PlayerCopyNode.GetNode<CollisionShape2D>("SwordHit/SwordCollision");
        if (SwordCollisionCopy != null) {
            SwordCollisionCopy.Disabled = true;  // Deaktiviere die Kollision der Kopie
        }
        
        // Position der Kopie entsprechend der Position des Spielers festlegen
        PlayerCopyNode.GlobalPosition = GlobalPosition + new Vector2(0, Sprite.Texture.GetHeight() * Sprite.Scale.Y * -0.5f);

        // Verblassen-Effekt für den Dash-Trail hinzufügen
        float AnimationTime = (float)(DashTimer.WaitTime / 3);

        Timer FadeTimer1 = new Timer();
        AddChild(FadeTimer1);
        FadeTimer1.Timeout += () => {
            if (IsInstanceValid(PlayerCopyNode)) {
                PlayerCopyNode.Modulate = new Color(PlayerCopyNode.Modulate, 0.4f);
            }
        };
        FadeTimer1.Start(AnimationTime);

        Timer FadeTimer2 = new Timer();
        AddChild(FadeTimer2);
        FadeTimer2.Timeout += () => {
            if (IsInstanceValid(PlayerCopyNode)) {
                PlayerCopyNode.Modulate = new Color(PlayerCopyNode.Modulate, 0.2f);
            }
        };
        FadeTimer2.Start(AnimationTime * 2);

        Timer FadeTimer3 = new Timer();
        AddChild(FadeTimer3);
        FadeTimer3.Timeout += () => {
            if (IsInstanceValid(PlayerCopyNode)) {
                PlayerCopyNode.QueueFree();  // Knoten sicher entfernen
            }
        };
        FadeTimer3.Start(AnimationTime * 3);
    }

    // Funktion zum Stoppen des Dashes
    private void StopDash() {
        IsDashing = false;
        DashEffect.Stop();
        DashTimer.Stop();
        DashTimer.Timeout -= StopDash;
    }

    public async void OnSwordHitBodyEntered(Node2D body){
        if(body.Name == "Player"){
            return;
        }
        GD.Print(body.Name);
        if(AnimationPlayer.CurrentAnimation.Equals("heavy_attack")){
            if(Sprite.FlipH == false){
                body.Position += new Vector2(20, 0);
            } else {
                body.Position -= new Vector2(20, 0);
            }
        }
        for (int i = 0; i < 3; i++){
            body.Visible = false;                                       //Dmg effect, wenn Gegner getroffen wird
            await ToSignal(GetTree().CreateTimer(0.05f), "timeout"); 
            body.Visible = true;
            await ToSignal(GetTree().CreateTimer(0.05f), "timeout"); 
        }
    }

    // Funktion zum Aktualisieren der Animationen
    private void UpdateAnimations() {

        if (Input.IsActionJustPressed("light_attack") && !IsDashing && AnimationPlayer.CurrentAnimation != "heavy_attack") {
            AnimationPlayer.Play("light_attack");
        } else if(Input.IsActionJustPressed("heavy_attack") && !IsDashing && AnimationPlayer.CurrentAnimation != "light_attack"){
            AnimationPlayer.Play("heavy_attack");
        }

        // Bewegungsanimationen
        if (IsOnFloor() && AnimationPlayer.CurrentAnimation != "light_attack" && AnimationPlayer.CurrentAnimation != "heavy_attack") {
            if (Velocity.X == 0) {
                AnimationPlayer.Play("idle");
            } else {
                AnimationPlayer.Play("run");
            }
        } else if (!IsOnFloor() && AnimationPlayer.CurrentAnimation != "light_attack" && AnimationPlayer.CurrentAnimation != "heavy_attack") {
            if (Velocity.Y < 0) {
                AnimationPlayer.Play("jump");
            } else if (Velocity.Y > 0) {
                AnimationPlayer.Play("fall");
            }
        }
    }
}
