using Godot;
using System;

public partial class spike : Area2D
{
	// Called when the node enters the scene tree for the first time.

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

		if (body is Player player)
		{
			
		}

	}
}
