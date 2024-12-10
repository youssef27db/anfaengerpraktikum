using Godot;
using System;

public partial class spike : Area2D
{
	// Variable für Player
	private Player player;


	public override void _Ready()
	{
		// Zugriff auf Player Node

		player = GetNode<Player>("../Player");
 	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPlayerBodyEntered(Node body)
	{
		
		/**
		 * Prüfen ob der Körper, der den Checkpoint betritt, ein Player ist
		 * Wenn ja, dann 
		 */

		if (body is Player)
		{
			player = (Player)body; // Instanzvariable setzen				
			player.TakeDamage(GetDamage());
			player.SlowPlayer(0.5f);
			GetNode<Timer>("Timer").Start();
			GD.Print("Player entered spike");
		}


	}

	private void OnPlayerBodyExited(Node body)
	{
		if (body is Player)
		{
			player = null; // Instanzvariable zurücksetzen

			GetNode<Timer>("Timer").Stop();
		}
	}

	private void _on_timer_timeout()
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

    return new Damage(PhysicalDamage, TrueDamage, Push);
}
}
