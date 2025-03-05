using Godot;

/** 
 * @brief Klasse für die Ausdauerleiste des Spielers.
 * Synchronisiert die Anzeige der StaminaBar mit der Ausdauer des Spielers.
 */
public partial class StaminaBar : TextureProgressBar {

    /** 
     * @brief Initialisiert die StaminaBar und verbindet sie mit der Ausdauer des Spielers.
     * Lädt den Spieler-Knoten und setzt die maximale und aktuelle Ausdauer in der StaminaBar.
     */
    public override void _Ready() {
        // Setze die maximale Ausdauer der StaminaBar basierend auf der Spieler-MaxStamina
        MaxValue = PlayerStats.Instance.GetMaxStamina();
        Value = PlayerStats.Instance.GetStamina();
    }

    /** 
     * @brief Aktualisiert die StaminaBar in jedem Frame.
     * Synchronisiert die Anzeige der aktuellen Ausdauer mit den Werten des Spielers.
     * @param delta Zeit seit dem letzten Frame (wird nicht direkt genutzt).
     */
    public override void _Process(double DeltaTime) {
        // Aktualisiere den Wert der StaminaBar basierend auf der aktuellen Ausdauer des Spielers
        Value = PlayerStats.Instance.GetStamina();
    }
}
