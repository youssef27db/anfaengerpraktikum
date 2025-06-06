using Godot;

/**
 * @brief Klasse für den LevelManager
 * Diese Klasse verwaltet den Levelwechsel und die Spielerpositionierung.
 */
public partial class LevelManager : Node2D
{
    /**
     * @brief Initialisierung der Referenzen.
     * Findet die relevanten Knoten in der Szene und weist sie zu.
     */
    public override void _Ready()
    {
        var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");

        /**
         * Wenn ein Spawn-Tag gesetzt ist, wird der Spieler an die entsprechende Tür gesetzt.
         * Dies wird verwendet, um den Spieler an eine bestimmte Tür zu setzen, wenn er von einem anderen Level aus spawnt.
         */
        if (NavigationManager.SpawnDoorTag != null)
        {
            OnLevelSpawn(NavigationManager.SpawnDoorTag);
        }
        else
        {
            NavigationManager.CallDeferred("TriggerPlayerSpawn", PlayerStats.Instance.GetPosition(), "");
        }

    }

    /**
     * @brief Wird aufgerufen, wenn ein neues Level geladen wird.
     * @param DestinationTag Das Tag der Tür, an der der Spieler spawnen soll.
     */
    private void OnLevelSpawn(string DestinationTag)
    {
        var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");
        // Pfad zur Tür basierend auf dem Ziel-Tag erstellen
        string DoorPath = "Doors/Door_" + DestinationTag;

        Door door = GetNode<Door>(DoorPath);

        // TriggerPlayerSpawn nach deferred ausführen
        NavigationManager.CallDeferred("TriggerPlayerSpawn", door.GlobalPosition, door.SpawnDirection);
    }
}
