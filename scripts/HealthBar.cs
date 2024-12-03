using Godot;

/** 
 * @brief Klasse für die Gesundheitsleiste des Spielers.
 * Synchronisiert die Anzeige der HealthBar mit den Lebenspunkten des Spielers.
 */
public partial class HealthBar : TextureProgressBar {
    // Referenz zum Spieler
    private Player player;

    /** 
     * @brief Initialisiert die HealthBar und verbindet sie mit den Lebenspunkten des Spielers.
     * Lädt den Spieler-Knoten und setzt die maximale und aktuelle Gesundheit in der HealthBar.
     */
    public override void _Ready() {
        // Spieler-Knoten im Baum finden (ersetze "Player" mit dem tatsächlichen Pfad des Spieler-Nodes)
        player = GetNode<Player>("../../Player");

        if (player != null) {
            // Setze die maximale Gesundheit der HealthBar basierend auf der Spieler-MaxHealth
            MaxValue = player.GetMaxHealthPoints();
            Value = player.GetCurrentHealth();
        }
    }

    /** 
     * @brief Aktualisiert die HealthBar in jedem Frame.
     * Synchronisiert die Anzeige der aktuellen Lebenspunkte mit den Werten des Spielers.
     * @param delta Zeit seit dem letzten Frame (wird nicht direkt genutzt).
     */
    public override void _Process(double delta) {
        if (player != null) {
            // Aktualisiere den Wert der HealthBar basierend auf der aktuellen Gesundheit des Spielers
            Value = player.GetCurrentHealth();
        }
    }
}
