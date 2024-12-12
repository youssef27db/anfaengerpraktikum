using Godot;
using System;

public partial class Spike : Area2D
{
	// Variable für Player
	private Player player;


	public override void _Ready()
	{
		// Zugriff auf Player Node

		player = GetNode<Player>("../Player");
 	}

	/**
		 * Prüfen ob der Körper den Spike betritt falls ja wird der Timer gestartet und der Spieler nimmt Schaden
		 */

	private void OnPlayerBodyEntered(Node body)
	{

		if (body is Player)
		{
			player = (Player)body; // Instanzvariable setzen				
			player.TakeDamage(GetDamage());
			player.SlowPlayer(0.5f);
			GetNode<Timer>("Timer").Start();
			GD.Print("Player entered spike");
		}


	}

	/**
	 * Prüfen ob der Körper den Spike verlässt, falls ja wird der Timer gestoppt und der Spieler nimmt keinen Schaden mehr
	 */

	private void OnPlayerBodyExited(Node body)
	{
		if (body is Player)
		{
			player = null; // Instanzvariable zurücksetzen

			GetNode<Timer>("Timer").Stop();
		}
	}

	// Timer wird gestartet und der Spieler nimmt Schaden
	private void OnTimerTimeout()
	{	
		GD.Print("Timer timeout");
		player.TakeDamage(GetDamage());
		GetNode<Timer>("Timer").Start();
	}

	public Damage GetDamage(){
    // Beispielwerte für den Schaden (anpassbar)
		float PhysicalDamage = 0f; 
		float TrueDamage = 10f;     
		Vector2 Push = new Vector2(0, 0);

		return new Damage(PhysicalDamage, TrueDamage, Push, this);
	}
}
