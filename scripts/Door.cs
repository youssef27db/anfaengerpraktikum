using Godot;
using System;

public partial class Door : Area2D
{

    /**
     * @brief Tag für das Ziellevel
     * @details Das Ziellevel wird über den Tag gefunden
     */

    [Export]
    public string DestinationLevelTag { get; set; }

    /**
     * @brief Tag für die Zieltür
     * @details Die Zieltür wird über den Tag gefunden
     */

    [Export]
    public string DestinationDoorTag { get; set; }

    /**
     * @brief Spawnposition
     * @details Die Spawnposition wird über die Richtung bestimmt
     */

    [Export]
    public string SpawnDirection { get; set; } = "up";

    public Node Spawn;

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
	private void OnPlayerBodyEntered(Node body){
		if (body is Player player)
		{
            var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");
            NavigationManager.GoToLevel(DestinationLevelTag, DestinationDoorTag);
		}
	}
}
