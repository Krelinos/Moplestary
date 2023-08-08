using Godot;
using System;

public partial class Projectile : Area2D
{
	// Max X-axis distance before projectile expires
	[Export] public int MaxDistance = 512;
	[Export] public int ProjectileSpeed = 32;	// per second

	private double distanceTravelled;

	private CollisionShape2D Hitbox;
	private AnimatedSprite2D Sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		distanceTravelled = 0;

		Hitbox = GetNode<CollisionShape2D>("CollisionShape2D");

		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Sprite.Play("default");
	}

	/*
		Use _physics_process()(_PhysicsProcess in C#) when one needs a framerate-independent
		delta time between frames. If code needs consistent updates over time, regardless of
		how fast or slow time advances, this is the right place. Recurring kinematic and
		object transform operations should execute here.

		https://docs.godotengine.org/en/stable/tutorials/best_practices/godot_notifications.html#process-vs-physics-process-vs-input

		'delta' is the elapsed time since the previous frame.
	*/
	public override void _PhysicsProcess(double delta)
	{
		if( distanceTravelled >= MaxDistance )
			QueueFree();
		else
			distanceTravelled += ProjectileSpeed * delta;

		Translate( Vector2.Right.Rotated( Rotation ) * ProjectileSpeed * (float)delta );
	}

	public void _on_body_entered( Node2D body )
	{
		ProjectileSpeed = 0;

		Hitbox.QueueFree();

		Sprite.Play("hit");
		Sprite.AnimationFinished += HitEffectFinished;
	}

	private void HitEffectFinished()
	{
		QueueFree();
	}
}
