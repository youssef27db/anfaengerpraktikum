using Godot;

/**
 * Der NavigationManager ist für das Laden von Leveln und das Spawnen des Spielers verantwortlich.
 * 
 * Der NavigationManager ist ein Singleton, der in der Haupt-Szene platziert wird und von anderen Skripten verwendet wird, um Level zu laden und den Spieler zu spawnen.
 */

public partial class NavigationManager : Node
{
    // Deklarieren der vorab geladenen Szenen
    private static readonly PackedScene sceneIntro = (PackedScene)GD.Load("res://Scenes/intro.tscn");
    private static readonly PackedScene sceneLevel1 = (PackedScene)GD.Load("res://Scenes/level1.tscn");

    // Die Spawn-Tag-Variable
    public string SpawnDoorTag { get; private set; }

    /**
     * Das Signal, das ausgelöst wird, wenn der Spieler spawnen soll.
     * 
     * @param position Die Position, an der der Spieler spawnen soll.
     * @param direction Die Richtung, in die der Spieler schauen soll.
     */

    [Signal]
    public delegate void OnTriggerPlayerSpawnEventHandler(Vector2 position, string direction);

    /**
     * Lädt das angegebene Level und setzt das Ziel-Tag für den Spieler-Spawn.
     * 
     * @param levelTag Das Tag des Levels, das geladen werden soll.
     * @param destinationTag Das Tag der Tür, an der der Spieler spawnen soll.
     */
    public void GoToLevel(string levelTag, string destinationTag)
    {
        PackedScene sceneToLoad = null;

        // Bestimmen, welches Level geladen werden soll
        switch (levelTag)
        {
            case "intro":
                sceneToLoad = sceneIntro;
                break;
            case "level1":
                sceneToLoad = sceneLevel1;
                break;
        }

        // Überprüfen, ob eine Szene ausgewählt wurde und diese dann laden
        if (sceneToLoad != null)
        {
            SpawnDoorTag = destinationTag;
            // Verwendung der ChangeSceneToPacked-Methode in Godot 4
            GetTree().ChangeSceneToPacked(sceneToLoad);
        }
    }

    /**
     * Lädt das angegebene Level und setzt das Ziel-Tag für den Spieler-Spawn.
     * 
     * @param levelTag Das Tag des Levels, das geladen werden soll.
     */
    public void TriggerPlayerSpawn(Vector2 position, string direction)
    {
        EmitSignal(SignalName.OnTriggerPlayerSpawn, position, direction);
    }
}
