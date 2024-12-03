using Godot;
using System;

/** 
 * @brief Klasse für den Spielercharakter.
 * Verwaltet Bewegung, Sprünge, Angriffe und Animationen.
 */
public partial class Player : CharacterBody2D
{
    // Variablen für Bewegung, Sprünge und Dash
    private const float SPEED = 100f;
    private const float JUMP_VELOCITY = -300f;
    private int JumpMax = 2;
    private int JumpCount = 0;

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

    private Vector2 HauptHitbox;

    private Vector2 SpawnPoint;
    private int lastAttack = 0;

    [Export]
    private float MaxHealthPoints = 100f;
    private float CurrentHealth;

    [Export]
    private float MaxStamina = 100f; 
    private float Stamina;  

    /** 
     * @brief Initialisierung der Referenzen.
     * Findet die relevanten Knoten in der Szene und weist sie zu.
     */
    public override void _Ready() {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = GetNode<Sprite2D>("Sprite2D");
        DashEffect = GetNode<Timer>("DashEffect");
        DashTimer = GetNode<Timer>("DashTimer");
        SwordCollision = GetNode<CollisionShape2D>("Sprite2D/SwordHit/SwordCollision");
        PlayerHitbox = GetNode<CollisionShape2D>("PlayerHitbox");
        HauptHitbox = PlayerHitbox.Position;
        CurrentHealth = 50f;
    }

    /** 
     * @brief Physikalische Prozesse werden in jedem Frame ausgeführt.
     * Berechnet Gravitation, Bewegung, Sprünge und Dashes.
     * @param DeltaTime Zeit seit dem letzten Frame.
     */
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

    /** 
     * @brief Verarbeitet die Sprunglogik.
     * Setzt den Sprungzähler zurück und ermöglicht einen Doppelsprung.
     */
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

    /** 
     * @brief Verarbeitet die Bewegung des Spielers.
     * Regelt normale Bewegungen, Dashes und Kollisionen.
     * @param DeltaTime Zeit seit dem letzten Frame.
     */
    private void HandleMovement(double DeltaTime) {
        Vector2 direction = new Vector2(Input.GetAxis("ui_left", "ui_right"), Input.GetAxis("ui_up", "ui_down")).Normalized();
        float currentSpeed = SPEED;

        // Sprite umdrehen basierend auf der Bewegungsrichtung und Kollision umdrehen
        if (direction.X < 0) {
            Sprite.FlipH = true;
            SwordCollision.Position = new Vector2(-Mathf.Abs(SwordCollision.Position.X), SwordCollision.Position.Y);
            PlayerHitbox.Position = new Vector2(Sprite.Position.X * 1.8f, PlayerHitbox.Position.Y);
        } else if (direction.X > 0) {
            Sprite.FlipH = false;
            SwordCollision.Position = new Vector2(Mathf.Abs(SwordCollision.Position.X), SwordCollision.Position.Y);
            PlayerHitbox.Position = HauptHitbox;
        }

        // Geschwindigkeit reduzieren, wenn der Spieler angreift
        if (AnimationPlayer.CurrentAnimation == "light_attack") {
            currentSpeed *= 0.5f;
        } else if (AnimationPlayer.CurrentAnimation == "heavy_attack") {
            currentSpeed *= 0.15f;
        }

        // Blockieren stoppt die Bewegung
        if (IsBlocking()) {
            currentSpeed = 0;
        }

        if (IsDashing) {
            DashInProgress(DeltaTime);
        } else {
            // Normale Bewegung verarbeiten, wenn kein Dash aktiv ist
            if (direction != Vector2.Zero) {
                Velocity = new Vector2(direction.X * currentSpeed, Velocity.Y);
            } else {
                Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
            }

            // Überprüfen, ob der Dash-Button gedrückt wurde
            if (Input.IsActionJustPressed("dash") && direction != Vector2.Zero && CanDash && !IsAttacking()) {
                DashDirection = direction;
                StartDash();
            }
        }
    }

    /** 
     * @brief Startet den Dash-Prozess.
     */
    private void StartDash() {
        IsDashing = true;
        CanDash = false;
        DashTimer.Timeout += StopDash;
        DashTimer.Start();
        DashEffect.Start();
        DashTrailTimer = 0f;
    }

    /** 
     * @brief Führt die Logik während eines Dashes aus.
     * @param DeltaTime Zeit seit dem letzten Frame.
     */
    private void DashInProgress(double DeltaTime) {
        // Charakter bewegt sich in die Dash-Richtung mit Dash-Geschwindigkeit
        if (DashDirection == Vector2.Up) {
            Velocity = DashDirection / 1.5f * DashSpeed;
        } else {
            Velocity = DashDirection * DashSpeed;
        }

        // Dash-Trail bei Intervallen erstellen
        DashTrailTimer -= (float)DeltaTime;
        if (DashTrailTimer <= 0f) {
            CreateDashEffect();
            DashTrailTimer = DashTrailInterval;
        }
    }

    /** 
     * @brief Erstellt einen visuellen Dash-Trail.
     * Der Spieler hinterlässt eine Spur während des Dashes.
     */
    private void CreateDashEffect() {
        Sprite2D PlayerCopyNode = (Sprite2D)Sprite.Duplicate();
        GetParent().AddChild(PlayerCopyNode);

        CollisionShape2D SwordCollisionCopy = PlayerCopyNode.GetNode<CollisionShape2D>("SwordHit/SwordCollision");
        if (SwordCollisionCopy != null) {
            SwordCollisionCopy.Disabled = true; // Deaktiviere die Kollision der Kopie
        }

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
                PlayerCopyNode.QueueFree();
            }
        };
        FadeTimer3.Start(AnimationTime * 3);
    }

    /** 
     * @brief Stoppt den Dash.
     */
    private void StopDash() {
        IsDashing = false;
        DashEffect.Stop();
        DashTimer.Stop();
        DashTimer.Timeout -= StopDash;
    }

    /** 
     * @brief Überprüft, ob der Spieler gerade angreift.
     * @return true, wenn der Spieler angreift.
     */
    private bool IsAttacking() {
        return AnimationPlayer.CurrentAnimation == "heavy_attack" || AnimationPlayer.CurrentAnimation == "light_attack";
    }

    /** 
     * @brief Überprüft, ob der Spieler blockiert.
     * @return true, wenn der Spieler blockiert.
     */
    private bool IsBlocking() {
        return AnimationPlayer.CurrentAnimation == "block";
    }

    /** 
     * @brief Gibt den SpawnPoint des Spielers zurück.
     * @return Der SpawnPoint des Spielers.
     */
    public void SetSpawnPoint(Vector2 spawnPoint) {
        SpawnPoint = spawnPoint;
    }

    /** 
    * @brief Setzt die aktuellen Lebenspunkte des Spielers.
    * @param Health Neue Lebenspunkte, die gesetzt werden sollen.
    */
    public void SetCurrentHealth(float Health){
        CurrentHealth = Health;
    }

    /** 
    * @brief Gibt die aktuellen Lebenspunkte des Spielers zurück.
    * @return Die aktuellen Lebenspunkte.
    */
    public float GetCurrentHealth(){
        return CurrentHealth;
    }

    /** 
    * @brief Setzt die maximalen Lebenspunkte des Spielers.
    * @param maxHealthPoints Die neuen maximalen Lebenspunkte (muss positiv sein).
     */
    public void SetMaxHealthPoints(float maxHealthPoints){
        // MaxHealthPoints muss immer positiv sein
        MaxHealthPoints = Mathf.Max(maxHealthPoints, 1); // Verhindert, dass MaxHealthPoints <= 0 wird
    }

    /** 
    * @brief Gibt die maximalen Lebenspunkte des Spielers zurück.
    * @return Die maximalen Lebenspunkte.
    */
    public float GetMaxHealthPoints(){
        return MaxHealthPoints;
    }

    /** 
    * @brief Heilt den Spieler vollständig, indem die aktuellen Lebenspunkte auf das Maximum gesetzt werden.
    */
    public void MaxHeal(){
        SetCurrentHealth(MaxHealthPoints);
    }

    /** 
    * @brief Wendet Schaden auf den Spieler an.
    * Reduziert die aktuellen Lebenspunkte basierend auf dem übergebenen Schaden und wendet einen Rückstoßeffekt an.
    * @param Damage Instanz der Klasse `Damage`, die den physischen und wahren Schaden sowie den Rückstoß enthält.
    */
    public void TakeDamage(Damage Damage){
        float totalDamage = Damage.GetPhysicalDMG() + Damage.GetTrueDMG();

        SetCurrentHealth(GetCurrentHealth() - totalDamage);
        Position += Damage.GetPushAmount();

        // Überprüfe, ob der Spieler gestorben ist
        if (GetCurrentHealth() <= 0){
            GD.Print("Spieler ist gestorben!");
            Respawn();
        }
    }

    /** 
    * @brief Gibt den Schaden zurück, den der Spieler mit seinem aktuellen Angriff verursacht.
    * Der Schaden basiert auf der letzten Angriffsmethode (`light_attack` oder `heavy_attack`).
    * @return Eine Instanz der Klasse `Damage`, die den physischen Schaden, wahren Schaden und Rückstoß enthält.
    */
    public Damage GetDamage(){
        if(lastAttack == 1){
            return new Damage(10, 0, Vector2.Zero);
        }
        if(lastAttack == 2){
            Vector2 Push = new Vector2(20,0);
            if(Sprite.FlipH){
                Push = -Push;
            }
            return new Damage(20, 0, Push);       
        }
        return new Damage(0,0,Vector2.Zero);
    }
    
    /** 
     * @brief Setzt die Position des Spielers auf den SpawnPoint zurück.
     */
    public void Respawn(){
        Position = SpawnPoint;
    }
    
    /** 
     * @brief Verarbeitung, wenn ein Körper das Schwert trifft.
     * @param body Der getroffene Körper.
     */
    public void OnEnemyHitBoxEntered(Area2D area){
        // Überprüfen, ob der Collider ein `BaseEnemy` ist
        if (area.GetParent() is BaseEnemy enemy){
            // Hole den Schaden vom Gegner und wende ihn auf den Spieler an
            Damage damage = enemy.GetDamage();
            TakeDamage(damage);
        }
    }

    /** 
     * @brief Aktualisiert die Animationen des Spielers.
     */
    private void UpdateAnimations() {
        if (Input.IsActionJustPressed("light_attack") && !IsDashing && !IsAttacking()) {
            lastAttack = 1;
            AnimationPlayer.Play("light_attack");
        } else if (Input.IsActionJustPressed("heavy_attack") && !IsDashing && !IsAttacking()) {
            lastAttack = 2;
            AnimationPlayer.Play("heavy_attack");
        }
        if (Input.IsActionPressed("block") && !IsDashing && !IsAttacking() && IsOnFloor()) {
            AnimationPlayer.Play("block");
            lastAttack = 0;
        }

        if (IsOnFloor() && !IsAttacking() && !IsBlocking()) {
            lastAttack = 0;
            if (Velocity.X == 0) {
                AnimationPlayer.Play("idle");
            } else {
                AnimationPlayer.Play("run");
            }
        } else if (!IsOnFloor() && !IsAttacking() && !IsBlocking()) {
            lastAttack = 0;
            if (Velocity.Y < 0) {
                AnimationPlayer.Play("jump");
            } else if (Velocity.Y > 0) {
                AnimationPlayer.Play("fall");
            }
        }
    }
}
