using Godot;

/** 
 * @brief Klasse für die Ausdauerleiste des Spielers.
 * Synchronisiert die Anzeige der StaminaBar mit der Ausdauer des Spielers.
 */
public partial class StaminaBar : TextureProgressBar {
    // Referenz zum Spieler
    private Player player;

    /** 
     * @brief Initialisiert die StaminaBar und verbindet sie mit der Ausdauer des Spielers.
     * Lädt den Spieler-Knoten und setzt die maximale und aktuelle Ausdauer in der StaminaBar.
     */
    public override void _Ready() {
        // Spieler-Knoten im Baum finden (ersetze "Player" mit dem tatsächlichen Pfad des Spieler-Nodes)
        player = GetNode<Player>("../../Player");

        if (player != null) {
            // Setze die maximale Ausdauer der StaminaBar basierend auf der Spieler-MaxStamina
            MaxValue = player.GetMaxStamina();
            Value = player.GetStamina();
        }
    }

    /** 
     * @brief Aktualisiert die StaminaBar in jedem Frame.
     * Synchronisiert die Anzeige der aktuellen Ausdauer mit den Werten des Spielers.
     * @param delta Zeit seit dem letzten Frame (wird nicht direkt genutzt).
     */
    public override void _Process(double delta) {
        if (player != null) {
            // Aktualisiere den Wert der StaminaBar basierend auf der aktuellen Ausdauer des Spielers
            Value = player.GetStamina();
        }
    }
}
