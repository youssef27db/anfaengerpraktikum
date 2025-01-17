using Godot;
using System;

/**
* @brief Klasse für einen stärkeren Boss-Gegner, der von BaseEnemy erbt
*/
public partial class Boss1 : BaseEnemy{

    private bool EnemiesRevived = false;
    private float RegenCooldown = 5.0f; // Zeit, nach der Regeneration beginnt, wenn kein Schaden genommen wurde
    private float RegenTimer = 0.0f; // Timer für die Zeit seit dem letzten Angriff
    private float RegenAmount = 10.0f; // Menge an Gesundheit, die pro Tick regeneriert wird


    /**
    * @brief Überschreibt die _Ready-Methode von BaseEnemy
    */

    public override void _Ready(){

        MaxHealthPoints = 400f;
        Damage = 50f;
        Armor = 30f;
        Speed = 10f;
        SinAmount = 100; // Bonuspunkte für Spieler beim Besiegen des Bosses

        base._Ready();

        CurrentHealthPoints = MaxHealthPoints;
        HealthBar.Value = 100f * CurrentHealthPoints / MaxHealthPoints;
    }

    /**
    * @brief Überschreibt die _Process-Methode von BaseEnemy
    * @param DeltaTime Die Zeit, die seit dem letzten Frame vergangen ist
    */
    public override void _Process(double DeltaTime){
        base._Process(DeltaTime);

        if (CurrentHealthPoints <= MaxHealthPoints / 2 && !EnemiesRevived){
            StartGlowing();
            ReviveEnemies();
            EnemiesRevived = true;
            Armor = 60f; // Rüstung erhöhen
        }

        HandleRegeneration(DeltaTime);
    }

    /**
    * @brief Regeneriert die Gesundheit des Bosses, wenn er keinen Schaden nimmt
    * @param DeltaTime Die Zeit, die seit dem letzten Frame vergangen ist
    */
    private void HandleRegeneration(double DeltaTime){
        if (CurrentHealthPoints < MaxHealthPoints){
            RegenTimer += (float)DeltaTime;

            if (RegenTimer >= RegenCooldown){
                CurrentHealthPoints = Math.Min(CurrentHealthPoints + RegenAmount, MaxHealthPoints);
                HealthBar.Value = 100f * CurrentHealthPoints / MaxHealthPoints;
                RegenTimer = 0.0f; // Timer zurücksetzen
            }
        }
    }

    /**
    * @brief Startet einen Leuchteffekt, wenn der Boss Schaden nimmt
    */
    private void StartGlowing(){
        // Ändere die Modulationsfarbe des Sprites, um ein Leuchten zu simulieren
        if (Sprite != null){
            ShowPopupMessage("AHHHH!!!");
            Sprite.Modulate = new Color(1.0f, 0.84f, 0.0f, 1.0f); // Ein goldliche Leuchteffekt
        }
    }

    /**
    * @brief Zeigt eine Popup-Nachricht an
    * @param message Die Nachricht, die angezeigt werden soll
    */
    private void ShowPopupMessage(string message){
        Label popup = new Label();
        popup.Text = message;
        popup.AddThemeColorOverride("font_color", new Color(1, 0, 0)); // Rot
        popup.Modulate = new Color(1, 1, 1, 0); // Start transparent
        popup.AutowrapMode = TextServer.AutowrapMode.Word;
        popup.SizeFlagsHorizontal = (Control.SizeFlags)(int)Control.SizeFlags.ExpandFill;
        popup.SizeFlagsVertical = (Control.SizeFlags)(int)Control.SizeFlags.ShrinkCenter;
        popup.HorizontalAlignment = HorizontalAlignment.Center;
        popup.VerticalAlignment = VerticalAlignment.Center;


        Vector2 bossGlobalPosition = GetGlobalTransformWithCanvas().Origin;
        popup.GlobalPosition = bossGlobalPosition + new Vector2(0, -100);

        CanvasLayer canvas = new CanvasLayer();
        AddChild(canvas);
        canvas.AddChild(popup);

        var tween = CreateTween();
        tween.TweenProperty(popup, "modulate:a", 1, 0.5f).From(0); // Einblenden
        tween.TweenProperty(popup, "modulate:a", 0, 0.5f).From(1).SetDelay(1.0f); // Ausblenden nach 1 Sekunde
        tween.Connect("finished", new Callable(popup, "queue_free"));
    }

    /**
    * @brief Lässt alle toten Feinde im Raum des Bosses wiederbeleben
    */
    private void ReviveEnemies()
    {
        // Hole den Elternknoten (bossRoom)
        Node BossRoom = GetParent();

        // Iteriere durch alle Kinder von bossRoom
        foreach (Node Child in BossRoom.GetChildren()){
            if (Child is BaseEnemy BaseEnemy && BaseEnemy.IsDead()){
                BaseEnemy.Respawn();
            }
        }
    }
}
