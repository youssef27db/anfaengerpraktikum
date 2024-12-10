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

        Door door = GetNode<Door>(doorPath);

        // TriggerPlayerSpawn nach deferred ausführen
        NavigationManager.CallDeferred("TriggerPlayerSpawn", door.GlobalPosition, door.SpawnDirection);
    }
}
