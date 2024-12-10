using Godot;

public partial class NavigationManager : Node
{
    // Deklarieren der vorab geladenen Szenen
    private static readonly PackedScene sceneIntro = (PackedScene)GD.Load("res://Scenes/intro.tscn");
    private static readonly PackedScene sceneLevel1 = (PackedScene)GD.Load("res://Scenes/level1.tscn");

    // Die Spawn-Tag-Variable
    private string spawnDoorTag;

    // Die Methode, die die Level-Wechsel-Logik verarbeitet
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
            spawnDoorTag = destinationTag;
            // Verwendung der ChangeSceneTo()-Methode in Godot 4
            GetTree().ChangeSceneToPacked(sceneToLoad);
        }
    }
}
