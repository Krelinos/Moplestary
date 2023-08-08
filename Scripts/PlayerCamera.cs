using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
	private CharacterBody2D PlyCharacter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlyCharacter = GetNode("/root/Game/PlayerCharacter") as CharacterBody2D;
		Position = PlyCharacter.Position;
		ResetSmoothing();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 camDistToChar = PlyCharacter.Position - Position;
		
		if (camDistToChar != Vector2.Zero )
		{
			var curPos = Position;
			Vector2 newPos = curPos + camDistToChar;

			Position = newPos;
		}
	}
}
