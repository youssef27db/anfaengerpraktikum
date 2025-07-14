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
    private BloodVial BloodVials;
    private Label SinDisplay;

    private Vector2 HauptHitbox;
    private int LastAttack = 0;

    //Variablen für Stamina
    private float TimeSinceLastStaminaUse = 0f;

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
        BloodVials = GetNode<BloodVial>("../HUD/BloodVial/Counter");
        SinDisplay = GetNode<Label>("../HUD/SinAmount/Counter");

        SinDisplay.Text = PlayerStats.Instance.GetSinAmount() + "";

        NavigationManager navigationManager = GetNode<NavigationManager>("/root/NavigationManager");
        navigationManager.Connect("OnTriggerPlayerSpawn", new Callable(this, nameof(OnSpawn)));

        Position = PlayerStats.Instance.GetPosition();
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

        TimeSinceLastStaminaUse += (float)DeltaTime;
        RegenerateStamina(20f, DeltaTime);

        // Heal
        if(Input.IsActionJustPressed("heal")){
            BloodVials.UseBloodVial();
        }
        
        HandleJump();
        HandleMovement(DeltaTime);
        MoveAndSlide();
        UpdateAnimations();
        PlayerStats.Instance.SetPosition(Position);
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
            if (JumpCount == 0) {
            // Erster Sprung ohne Stamina-Verlust
            Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);
            JumpCount += 1;
            } else if (JumpCount > 0) {
                // Beim Doppelsprung Stamina prüfen und abziehen
                if (UseStamina(15)) {
                    Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);
                    JumpCount += 1;
                }
            } 
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

            // Überprüfen, ob der Dash-Button gedrückt wurde mit eine Bewegungsrichtung und nicht schon am angreifen ist
            if (Input.IsActionJustPressed("dash") && direction != Vector2.Zero && CanDash && !IsAttacking()) {
                // Wenn der Player genug Stamina hat kann er dashen
                if (UseStamina(20)){
                    DashDirection = direction;
                    StartDash(); 
                }
            }
        }
    }

    /** 
     * @brief Startet den Dash-Prozess.
     */
    private void StartDash() {
        SetCollisionLayerValue(1,false);
        SetCollisionMaskValue(1,false);
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
        SetCollisionLayerValue(1,true);
        SetCollisionMaskValue(1,true);
    }

    /** 
     * @brief Überprüft, ob der Spieler gerade angreift.
     * @return true, wenn der Spieler angreift.
     */
    public bool IsAttacking() {
        return AnimationPlayer.CurrentAnimation == "heavy_attack" || AnimationPlayer.CurrentAnimation == "light_attack";
    }

    /** 
     * @brief Überprüft, ob der Spieler blockiert.
     * @return true, wenn der Spieler blockiert.
     */
    public bool IsBlocking() {
        return AnimationPlayer.CurrentAnimation == "block";
    }

    /** 
    * @brief Heilt den Spieler vollständig, indem die aktuellen Lebenspunkte auf das Maximum gesetzt werden.
    */
    public void MaxHeal(){
        PlayerStats.Instance.SetCurrentHealth(PlayerStats.Instance.GetMaxHealthPoints());
    }

    /** 
    * @brief Wendet Schaden auf den Spieler an.
    * Reduziert die aktuellen Lebenspunkte basierend auf dem übergebenen Schaden und wendet einen Rückstoßeffekt an.
    * @param Damage Instanz der Klasse `Damage`, die den physischen und wahren Schaden sowie den Rückstoß enthält.
    */
    public void TakeDamage(Damage Damage){
        float totalDamage = Damage.GetTrueDMG();
        if(!IsBlocking()){
            totalDamage += Damage.GetPhysicalDMG();
        } else {
            float CurrentStamina = PlayerStats.Instance.GetStamina();
            CurrentStamina -= Damage.GetPhysicalDMG();
            if(CurrentStamina < 0){
                totalDamage -= CurrentStamina;
            }
            PlayerStats.Instance.SetStamina(CurrentStamina);
        }

        PlayerStats.Instance.SetCurrentHealth(PlayerStats.Instance.GetCurrentHealth() - totalDamage);
        Position += Damage.GetPushAmount();

        // Überprüfe, ob der Spieler gestorben ist
        if (PlayerStats.Instance.GetCurrentHealth() <= 0){
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
        if(LastAttack == 1){
            return new Damage(50, 0, Vector2.Zero, this);
        }
        if(LastAttack == 2){
            Vector2 Push = new Vector2(20,0);
            if(Sprite.FlipH){
                Push = -Push;
            }
            return new Damage(100, 0, Push, this);       
        }
        return new Damage(0,0,Vector2.Zero, this);
    }

    /** 
    * @brief Regeneriert die Stamina des Spielers, wenn er für eine bestimmte Zeit keine Stamina-verbrauchende Aktion durchgeführt hat.
    * @param Amount Menge der Stamina, die regeneriert werden soll.
    * @param delta Zeit seit dem letzten Frame.
    */
    public void RegenerateStamina(float Amount, double delta) {
        // Wenn die Verzögerungszeit erreicht wurde, regeneriere Stamina
        if (TimeSinceLastStaminaUse >= 1f) {
            PlayerStats.Instance.SetStamina(PlayerStats.Instance.GetStamina() + Amount * (float)delta); // Regeneriere Stamina abhängig von der Zeit
        }
    }

    /** 
    * @brief Verbraucht eine bestimmte Menge an Stamina, falls genügend verfügbar ist.
    * Setzt den Inaktivitäts-Timer zurück, wenn Stamina verbraucht wird.
    * @param Amount Die Menge an Stamina, die verbraucht werden soll.
    * @return `true`, wenn genügend Stamina verfügbar war und die Aktion ausgeführt wurde; andernfalls `false`.
    */
    public bool UseStamina(float Amount) {
        // Versucht, eine bestimmte Menge an Stamina zu verbrauchen.
        // Gibt true zurück, wenn genug Stamina verfügbar war; andernfalls false.
        if (PlayerStats.Instance.GetStamina() >= Amount) {
            PlayerStats.Instance.SetStamina(PlayerStats.Instance.GetStamina() - Amount);
            TimeSinceLastStaminaUse = 0f;
            return true;
        }

        return false;
    }

    /**
    * @brief Verlangsamt den Spieler um einen bestimmten Prozentsatz.
    * @param SlowAmount Der Prozentsatz, um den der Spieler verlangsamt werden soll.
    */
    public void SlowPlayer(float SlowAmount){
        Velocity = new Vector2(Velocity.X * SlowAmount, Velocity.Y);
    }
    
    /**
    * @brief Lässt den Spieler am Checkpoint spawnen.
    */
    public void Respawn(){
        var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");
        NavigationManager.GoToLevel(PlayerStats.Instance.GetRespawnLevelTag(), "spawn");
        BloodVials.ResetUses();

    }
    
    /**
    * @brief Getter für BloodVials.
    * @return BloodVial
    */
    public BloodVial GetBloodVials(){
        return BloodVials;
    }

    /** 
    * @brief Setzt den SinAmount des Spielers.
    * @param Value Der neue Wert für den SinAmount.
    */
    public void SetSinAmount(int Value) {
        // SinAmount muss immer >= 0 sein
        PlayerStats.Instance.SetSinAmount(Value);
        SinDisplay.Text = PlayerStats.Instance.GetSinAmount() + "";
    }

    /**
    * @brief Wird aufgerufen, wenn der Spieler an einer neuen Position spawnen soll.
    * @param position Die Position, an der der Spieler spawnen soll.
    * @param direction Die Richtung, in die der Spieler schauen soll.
    */
    private void OnSpawn(Vector2 position, string direction){

        // Spielerposition auf die übergebene Position setzen
        if (direction == "right")
        {
            // Update the x value by adding 50 to it, keep the original y value
            Sprite.FlipH = false;
            position = position with { X = position.X + 25 };
        }
        else if (direction == "left")
        {
            // Update the x value by subtracting 50 from it, keep the original y value
            Sprite.FlipH = true;
            position = position with { X = position.X - 25 };
        }
        Position = position;

    }


    /** 
     * @brief Aktualisiert die Animationen des Spielers.
     */
    private void UpdateAnimations() {
        if (Input.IsActionJustPressed("light_attack") && !IsDashing && !IsAttacking()) {
            if (UseStamina(10)){
                LastAttack = 1;
                AnimationPlayer.Play("light_attack");
            }
        } else if (Input.IsActionJustPressed("heavy_attack") && !IsDashing && !IsAttacking()) {
            if (UseStamina(25)){
                LastAttack = 2;
                AnimationPlayer.Play("heavy_attack");
            }
        }
        if (Input.IsActionPressed("block") && !IsDashing && !IsAttacking() && IsOnFloor()) {
            if (UseStamina(0)){
                AnimationPlayer.Play("block");
                LastAttack = 0;
            }
        }

        if (IsOnFloor() && !IsAttacking() && !IsBlocking()) {
            LastAttack = 0;
            if (Velocity.X == 0) {
                AnimationPlayer.Play("idle");
            } else {
                AnimationPlayer.Play("run");
            }
        } else if (!IsOnFloor() && !IsAttacking() && !IsBlocking()) {
            LastAttack = 0;
            if (Velocity.Y < 0) {
                AnimationPlayer.Play("jump");
            } else if (Velocity.Y > 0) {
                AnimationPlayer.Play("fall");
            }
        }
    }
}
