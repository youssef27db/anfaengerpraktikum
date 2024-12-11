using Godot;
using System;

public partial class Checkpoint : Node2D
{

	// Variable für Player
	private Player player;
	public override void _Ready()
	{
		// Zugriff auf Player Node

		player = GetNode<Player>("../Player");
	}

	public override void _Process(double delta)
	{
	}
	
	private void OnPlayerBodyEntered(Node body)
	{
		
		/**
		 * Prüfen ob der Körper, der den Checkpoint betritt, ein Player ist
		 * Wenn ja, dann wird der Checkpoint als Spawnpoint gesetzt
		 */

		if (body is Player player)
		{
			// Setzen des Spawnpoints
			player.SetSpawnPoint(this.GlobalPosition);
			player.MaxHeal();
			player.SetStamina(player.GetMaxStamina());
        	GD.Print("Spawnpoint des Players gesetzt auf: ", this.GlobalPosition);

			PlayerStats playerStats = GetNode<PlayerStats>("/root/PlayerStats");
			playerStats.SetRespawnLevelTag(GetParent().Name);
			GD.Print("RespawnLevelTag des Players gesetzt auf: ", GetParent().Name);
		}

	}
}
