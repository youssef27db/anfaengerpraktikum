using Godot;
using System;

/**
 * @brief Klasse für die Tür
 * @details Die Klasse ist für den Wechsel zwischen den Levels zuständig.
 */
public partial class Door : Area2D
{
    public Node Spawn;

    [Export]
    public string DestinationLevelTag { get; set; }

    [Export]
    public string DestinationDoorTag { get; set; }

    [Export]
    public string SpawnDirection { get; set; } = "up";

    

    /**
     * @brief Initialisierung der Node Spawn
     */
    public override void _Ready()
    {
        Spawn = GetNode("Spawn");
    }


    /**
     * @brief Diese Funktion wird aufgerufen, wenn der Player die Tür betritt
     * @param body Der Körper, der die Tür betritt
     */
    private void OnPlayerBodyEntered(Node body)
    {
        if (body is Player player)
        {
            var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");
            NavigationManager.GoToLevel(DestinationLevelTag, DestinationDoorTag);
        }
    }
}
