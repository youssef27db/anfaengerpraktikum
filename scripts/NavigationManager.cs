using Godot;

/**
 * Der NavigationManager ist für das Laden von Leveln und das Spawnen des Spielers verantwortlich.
 * 
 * Der NavigationManager ist ein Singleton, der in der Haupt-Szene platziert wird und von anderen Skripten verwendet wird, um Level zu laden und den Spieler zu spawnen.
 */

public partial class NavigationManager : Node
{
    public static NavigationManager Instance { get; private set; }
    // Deklarieren der vorab geladenen Szenen
    private static readonly PackedScene SceneMainMenu = (PackedScene)GD.Load("res://Scenes/main_menu.tscn");
    private static readonly PackedScene SceneIntro = (PackedScene)GD.Load("res://Scenes/intro.tscn");
    private static readonly PackedScene SceneLevel1 = (PackedScene)GD.Load("res://Scenes/level1.tscn");
    private static readonly PackedScene SceneBoss = (PackedScene)GD.Load("res://Scenes/bossRoom.tscn");
    private static readonly PackedScene SceneLevelOne = (PackedScene)GD.Load("res://Scenes/level_one.tscn");
    private static readonly PackedScene SceneLevelTwo = (PackedScene)GD.Load("res://Scenes/level_two.tscn");

    // Die Spawn-Tag-Variable
    public string SpawnDoorTag { get; private set; }

    /**
     * Das Signal, das ausgelöst wird, wenn der Spieler spawnen soll.
     * 
     * @param Position Die Position, an der der Spieler spawnen soll.
     * @param Direction Die Richtung, in die der Spieler schauen soll.
     */

    [Signal]
    public delegate void OnTriggerPlayerSpawnEventHandler(Vector2 Position, string Direction);

    /**
     * Initialisiert den NavigationManager und setzt ihn als Singleton.
     */

    public override void _Ready(){
        Instance = this;
    }

    /**
     * Lädt das angegebene Level und setzt das Ziel-Tag für den Spieler-Spawn.
     * 
     * @param LevelTag Das Tag des Levels, das geladen werden soll.
     * @param DestinationTag Das Tag der Tür, an der der Spieler spawnen soll.
     */
    public void GoToLevel(string LevelTag, string DestinationTag)
    {
        PackedScene SceneToLoad = null;

        // Bestimmen, welches Level geladen werden soll
        switch (LevelTag)
        {
            case "main_menu":
                SceneToLoad = SceneMainMenu;
                break;
            case "intro":
                SceneToLoad = SceneIntro;
                break;
            case "level1":
                SceneToLoad = SceneLevel1;
                break;
            case "bossRoom":
                SceneToLoad = SceneBoss;
                break;
            case "level_one":
                SceneToLoad = SceneLevelOne;
                break;
            case "level_two":
                SceneToLoad = SceneLevelTwo;
                break;
        }

        // Überprüfen, ob eine Szene ausgewählt wurde und diese dann laden
        if (SceneToLoad != null){
            if(SceneToLoad != SceneMainMenu){
                PlayerStats.Instance.SetCurrentLevelTag(LevelTag);
                SpawnDoorTag = DestinationTag;
            }
            // Verwendung der ChangeSceneToPacked-Methode in Godot 4
            CallDeferred(nameof(DeferredChangeScene), SceneToLoad);
        } 
    }

    /**
     * Diese Methode wird aufgerufen, um die Szene zu wechseln.
     * 
     * @param SceneToLoad Die Szene, die geladen werden soll.
     */

    private void DeferredChangeScene(PackedScene SceneToLoad)
    {
        GetTree().ChangeSceneToPacked(SceneToLoad);
    }

    /**
     * Lädt das angegebene Level und setzt das Ziel-Tag für den Spieler-Spawn.
     * 
     * @param LevelTag Das Tag des Levels, das geladen werden soll.
     */
    public void TriggerPlayerSpawn(Vector2 Position, string Direction)
    {
        EmitSignal(SignalName.OnTriggerPlayerSpawn, Position, Direction);
    }
}
