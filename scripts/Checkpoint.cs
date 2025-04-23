using Godot;
using System;

public partial class Checkpoint : Node2D
{

	// Variable für Player
	private Player player;

	/*
	 * @brief Intitalisierung der Node player
	 */

	public override void _Ready()
	{
		// Zugriff auf Player Node

		player = GetNode<Player>("../Player");
	}
	
	/*
	 * @brief Diese Funktion wird aufgerufen, wenn der Player den Checkpoint betritt
	 * @param body Der Körper, der den Checkpoint betritt
	 */
	private void OnPlayerBodyEntered(Node body)
	{
		
		/**
		 * Prüfen ob der Körper, der den Checkpoint betritt, ein Player ist
		 * Wenn ja, dann wird der Checkpoint als Spawnpoint gesetzt
		 */

		if (body is Player player)
		{
			// Setzen des Spawnpoints
			PlayerStats.Instance.SetSpawnPoint(this.GlobalPosition);
			player.MaxHeal();
			PlayerStats.Instance.SetStamina(PlayerStats.Instance.GetMaxStamina());
			player.GetBloodVials().ResetUses();
        	GD.Print("Spawnpoint des Players gesetzt auf: ", this.GlobalPosition);

			PlayerStats playerStats = GetNode<PlayerStats>("/root/PlayerStats");
			playerStats.SetRespawnLevelTag(GetParent().Name);
			GD.Print("RespawnLevelTag des Players gesetzt auf: ", GetParent().Name);
			GD.Print(PlayerStats.Instance.GetRespawnLevelTag());
		}

	}
}
