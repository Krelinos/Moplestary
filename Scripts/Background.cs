using Godot;
using System;

public partial class Background : Node2D
{

	private Node2D parallax1;
	private Node2D parallax2;
	private Node2D parallax3;

	private Camera2D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parallax1 = GetNode<Node2D>("Parallax1");
		parallax2 = GetNode<Node2D>("Parallax2");
		parallax3 = GetNode<Node2D>("Parallax3");
		
		camera = GetNode("/root/Game/PlayerCamera") as Camera2D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = camera.GetScreenCenterPosition();
	}
}
