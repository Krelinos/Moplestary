using Godot;
using System;

public partial class Background : Node2D
{

	[Export] public float Parallax2CycleLength = 512;
	[Export] public float Parallax2CycleOffset = 320;
	[Export] public float Parallax3CycleLength = 1024;
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
		Parallax2OriginalPosition = Parallax3.Position;
		
		Camera = GetNode("/root/Game/PlayerCamera") as Camera2D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = Camera.GetScreenCenterPosition();
		Parallax2.Position = Parallax2OriginalPosition + Parallax2CycleOffset * ( GlobalPosition / Parallax2CycleLength );
	}
}
