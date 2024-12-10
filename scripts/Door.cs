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

    private Node spawn;

    public override void _Ready()
    {
        spawn = GetNode("Spawn");
    }

	private void OnPlayerBodyEntered(Node body){
		if (body is Player player)
		{
            var navigationManager = GetNode<NavigationManager>("/root/NavigationManager");
            navigationManager.GoToLevel(DestinationLevelTag, DestinationDoorTag);
		}
	}
}
