using Godot;
using System;

public partial class Background : Node2D
{
	// Distance required to offset Parallax2 by Parallax2CycleOffset.
	// Example: when Parallax2CycleLength is 512 and Parallax2CycleOffset is 64,
	// once the camera's global position is (512, 256), Parallax2 will be offsetted
	// by (-64, -32).
	[Export] public float Parallax2CycleLength = 0b1000000000000000;	// 160384 I think
	[Export] public float Parallax2CycleOffset = 320;

	[Export] public float Parallax3CycleLength = 0b0100010000000000;
	[Export] public float Parallax3CycleOffset = 320;

	private Node2D Parallax1;
	private Node2D Parallax2;
	private Node2D Parallax3;

	private Vector2 Parallax2OriginalPosition;
	private Vector2 Parallax3OriginalPosition;

	private Camera2D Camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parallax1 = GetNode<Node2D>("Parallax1");
		Parallax2 = GetNode<Node2D>("Parallax2");
		Parallax3 = GetNode<Node2D>("Parallax3");

		Parallax2OriginalPosition = Parallax2.Position;
		Parallax3OriginalPosition = Parallax3.Position;
		
		Camera = GetNode("/root/Game/PlayerCamera") as Camera2D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = Camera.GetScreenCenterPosition();

		Parallax2.Position = Parallax2OriginalPosition - Parallax2CycleOffset * ( GlobalPosition / Parallax2CycleLength );
		Parallax3.Position = Parallax3OriginalPosition - Parallax3CycleOffset * ( GlobalPosition / Parallax3CycleLength );
	}
}
