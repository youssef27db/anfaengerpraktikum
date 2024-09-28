using Godot;
using System;

public partial class Player : CharacterBody2D
{
    private const float SPEED = 100f;
    private const float JUMP_VELOCITY = -300f;

    // Variablen für den Doppelsprung
    private int jumpMax = 2;
    private int jumpCount = 0;

    // Variablen für den Dash
    private Vector2 dashDirection = Vector2.Zero;
    private float dashSpeed = 300f;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTrailInterval = 0.05f;
    private float dashTrailTimer = 0f;

    // Referenzen zu den Knoten
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Timer dashEffect;
    private Timer dashTimer;

    public override void _Ready() {
        // Knoten in der Szene finden und zuweisen
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite2D>("Sprite2D");
        dashEffect = GetNode<Timer>("DashEffect");
        dashTimer = GetNode<Timer>("DashTimer");
    }

    public override void _PhysicsProcess(double delta) {
        // Gravitation hinzufügen, wenn der Charakter nicht am Boden ist
        if (!IsOnFloor()) {
            Velocity += GetGravity() * (float)delta;
        } else {
            canDash = true; // Dash wird zurückgesetzt, wenn der Charakter am Boden ist
        }

        HandleJump();
        HandleDash(delta);
        MoveAndSlide();
        UpdateAnimations();
    }

    private void HandleJump() {
        // Sprungzähler zurücksetzen, wenn der Charakter am Boden ist
        if (jumpCount != 0 && IsOnFloor()) {
            jumpCount = 0;
        }

        // Überprüfen, ob der Sprung-Button gedrückt wurde und der Charakter noch Sprünge übrig hat
        if (Input.IsActionJustPressed("ui_up") && jumpCount < jumpMax) {
            Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);
            jumpCount += 1;
        }
    }

    private void HandleDash(double delta) {
        // Bewegungsrichtung bestimmen (links/rechts)
        Vector2 direction = new Vector2(Input.GetAxis("ui_left", "ui_right"), Input.GetAxis("ui_up", "ui_down")).Normalized();

        // Sprite umdrehen basierend auf der Bewegungsrichtung
        if (direction.X < 0) {
            sprite.FlipH = true;
        } else if (direction.X > 0) {
            sprite.FlipH = false;
        }

        // Dash-Verarbeitung
        if (isDashing) {
            DashInProgress(delta);
        } else {
            // Normale Bewegung verarbeiten, wenn kein Dash aktiv ist
            if (direction != Vector2.Zero) {
                Velocity = new Vector2(direction.X * SPEED, Velocity.Y);
            } else {
                Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
            }

            // Überprüfen, ob der Dash-Button gedrückt wurde und Dash möglich ist
            if (Input.IsActionJustPressed("dash") && direction != Vector2.Zero && canDash && direction != Vector2.Up) {
                dashDirection = direction;
                StartDash();
            }
        }
    }

    // Funktion, die den Dash-Prozess startet
    private void StartDash() {
        isDashing = true;
        canDash = false;
        dashTimer.Timeout += StopDash;
        dashTimer.Start();
        dashEffect.Start();
        dashTrailTimer = 0f; // Dash-Trail-Timer zurücksetzen
    }

    // Funktion, die während des Dashes ausgeführt wird
    private void DashInProgress(double delta) {
        // Charakter bewegt sich in die Dash-Richtung mit Dash-Geschwindigkeit
        Velocity = dashDirection * dashSpeed;

        // Dash-Trail bei Intervallen erstellen
        dashTrailTimer -= (float)delta;
        if (dashTrailTimer <= 0f) {
            CreateDashEffect();
            dashTrailTimer = dashTrailInterval;
        }
    }

    // Funktion zum Erstellen des Dash-Effekts (Nachbild des Spielers)
    private void CreateDashEffect() {
        Sprite2D playerCopyNode = (Sprite2D)sprite.Duplicate();
        GetParent().AddChild(playerCopyNode);

        // Position der Kopie entsprechend der Position des Spielers festlegen
        playerCopyNode.GlobalPosition = GlobalPosition + new Vector2(0, sprite.Texture.GetHeight() * sprite.Scale.Y * -0.5f);

        // Verblassen-Effekt für den Dash-Trail hinzufügen
        float animationTime = (float)(dashTimer.WaitTime / 3);

        Timer fadeTimer1 = new Timer();
        AddChild(fadeTimer1);
        fadeTimer1.Timeout += () => {
            if (IsInstanceValid(playerCopyNode)) {
                playerCopyNode.Modulate = new Color(playerCopyNode.Modulate, 0.4f);
            }
        };
        fadeTimer1.Start(animationTime);

        Timer fadeTimer2 = new Timer();
        AddChild(fadeTimer2);
        fadeTimer2.Timeout += () => {
            if (IsInstanceValid(playerCopyNode)) {
                playerCopyNode.Modulate = new Color(playerCopyNode.Modulate, 0.2f);
            }
        };
        fadeTimer2.Start(animationTime * 2);

        Timer fadeTimer3 = new Timer();
        AddChild(fadeTimer3);
        fadeTimer3.Timeout += () => {
            if (IsInstanceValid(playerCopyNode)) {
                playerCopyNode.QueueFree();  // Knoten sicher entfernen
            }
        };
        fadeTimer3.Start(animationTime * 3);
    }

    // Funktion zum Stoppen des Dashes
    private void StopDash() {
        isDashing = false;
        dashEffect.Stop();
        dashTimer.Stop();
        dashTimer.Timeout -= StopDash;
    }

    // Funktion zum Aktualisieren der Animationen
    private void UpdateAnimations() {
        if (IsOnFloor()) {
            if (Velocity.X == 0) {
                animationPlayer.Play("idle");
            } else {
                animationPlayer.Play("run");
            }
        } else {
            if (Velocity.Y < 0) {
                animationPlayer.Play("jump");
            } else if (Velocity.Y > 0) {
                animationPlayer.Play("fall");
            }
        }
    }
}
