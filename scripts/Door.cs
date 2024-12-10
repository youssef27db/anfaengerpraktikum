using Godot;
using System;

public partial class Door : Area2D
{
    [Export]
    public string DestinationLevelTag { get; set; }

    [Export]
    public string DestinationDoorTag { get; set; }

    [Export]
    public string SpawnDirection { get; set; } = "up";

    public Node spawn;

    public override void _Ready()
    {
        spawn = GetNode("Spawn");
    }

	private void OnPlayerBodyEntered(Node body){
		if (body is Player player)
		{
            var NavigationManager = GetNode<NavigationManager>("/root/NavigationManager");
            NavigationManager.GoToLevel(DestinationLevelTag, DestinationDoorTag);
		}
	}
}
