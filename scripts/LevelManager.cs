using Godot;

public partial class LevelManager : Node2D
{
    public override void _Ready()
    {
        var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");

        // Überprüfen, ob der spawn_door_tag gesetzt ist
        if (NavigationManager.SpawnDoorTag != null)
        {
            OnLevelSpawn(NavigationManager.SpawnDoorTag);
        }
    }

    private void OnLevelSpawn(string destinationTag)
    {
        var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");
        // Pfad zur Tür basierend auf dem Ziel-Tag erstellen
        string doorPath = "Doors/Door_" + destinationTag;

        // Die Tür-Node laden
        Door door = GetNode<Door>(doorPath);
        Node2D marker = door.GetNode<Node2D>("Marker");
        Vector2 markerPosition = door.GetNode<Node2D>("Marker").GlobalPosition;

        // Trigger für das Spawnen des Spielers auslösen
//        Vector2 spawnPosition = door.GlobalPosition;  // Beispiel: Position der Tür als Spawn-Position
//        string spawnDirection = "up"; // Beispiel: Richtung (z.B. 'up', 'down', etc.)

        // TriggerPlayerSpawn mit den richtigen Parametern aufrufen
        NavigationManager.TriggerPlayerSpawn(markerPosition, door.SpawnDirection);
    }
}
