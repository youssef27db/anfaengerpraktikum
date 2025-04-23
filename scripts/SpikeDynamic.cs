using Godot;
using System;

public partial class SpikeDynamic : Node2D
{
	// Variable für Player
	private Player player;

	[Export]
	private float Damage = 10f;

	/**
	 * @brief Initialisierung der Node Player
	 * @details Hier wird der Player Node gefunden
	 */

	public override void _Ready()
	{
		// Zugriff auf Player Node

		player = GetNode<Player>("../../../Player");
 	}

	/**
	* @brief Prüfen ob der Körper den Spike betritt falls ja wird der Timer gestartet und der Spieler nimmt Schaden
	*/

	private void OnPlayerBodyEntered(Node body)
	{

		if (body is Player)
		{
			player = (Player)body; // Instanzvariable setzen				
			player.TakeDamage(GetDamage());
			player.SlowPlayer(0.5f);
			GetNode<Timer>("StaticBody2D/Area2D/Timer").Start();
			GD.Print("Player entered spike");
		}


	}

	/**
	 * @brief Prüfen ob der Körper den Spike verlässt, falls ja wird der Timer gestoppt und der Spieler nimmt keinen Schaden mehr
	 */

	private void OnPlayerBodyExited(Node body)
	{
		if (body is Player)
		{
			player = null; // Instanzvariable zurücksetzen
			GetNode<Timer>("StaticBody2D/Area2D/Timer").Stop();
		}
	}

	/**
	 * @brief Timer Timeout Methode, die den Schaden an den Spieler übergibt
	 */
	private void OnTimerTimeout()
	{	
		GD.Print("Timer timeout");
		player.TakeDamage(GetDamage());
		GetNode<Timer>("StaticBody2D/Area2D/Timer").Start();
	}

	/**
	 * @brief Gibt ein Damage Objekt zurück.
	 * @return Damage Objekt
	 */
	public Damage GetDamage(){
		return new Damage(0, Damage, Vector2.Zero, this);
	}
}
