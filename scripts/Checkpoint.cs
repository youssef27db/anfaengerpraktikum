using Godot;
using System;

public partial class Checkpoint : Node2D
{

	// Variable für Player
	private Player Player;

	/*
	 * @brief Intitalisierung der Node Player
	 */
	public override void _Ready()
	{
		// Zugriff auf Player Node
		Player = GetNode<Player>("../Player");
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

		if (body is Player Player)
		{
			// Setzen des Spawnpoints
			PlayerStats PlayerStats = GetNode<PlayerStats>("/root/PlayerStats");
			PlayerStats.Instance.SetSpawnPoint(this.GlobalPosition);
			Player.MaxHeal();
			PlayerStats.Instance.SetStamina(PlayerStats.Instance.GetMaxStamina());
			Player.GetBloodVials().ResetUses();
        	GD.Print("Spawnpoint des Players gesetzt auf: ", this.GlobalPosition);

			PlayerStats.SetRespawnLevelTag(GetParent().Name);
			GD.Print("RespawnLevelTag des Players gesetzt auf: ", GetParent().Name);
			GD.Print(PlayerStats.Instance.GetRespawnLevelTag());
		}

	}
}
