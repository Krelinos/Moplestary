using Godot;
using System;

public partial class PlayerCamera : Camera2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CharacterBody2D plyCharacter = GetNode("/root/Game/PlayerCharacter") as CharacterBody2D;

		Vector2 camDistToChar = plyCharacter.Position - Position;
		
		if (camDistToChar != Vector2.Zero )
		{
			var curPos = Position;
			Vector2 newPos = curPos + camDistToChar;

			Position = newPos;
		}
	}
}
