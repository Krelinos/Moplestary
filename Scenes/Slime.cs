using Godot;
using System;

public partial class Slime : CharacterBody2D
{
	[Export] public const float Speed = 150.0f;
	[Export] public const float JumpVelocity = -700.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public AnimatedSprite2D sprite;

	private int currentMovementDirection;

	private RandomNumberGenerator RNG;
	
	private bool shouldFaceLeft;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		currentMovementDirection = 0;
		RNG = new RandomNumberGenerator();
		RNG.Randomize();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Horizontal movement
		if ( RNG.RandiRange(0, 200) == 0 )
		{
			if ( currentMovementDirection == 0 )
				currentMovementDirection = RNG.RandiRange(-1, 1);
			else
				currentMovementDirection = 0;
		}

		if (currentMovementDirection != 0)
		{
			velocity.X = currentMovementDirection * Speed;

			if ( IsOnFloor() )
				sprite.Play("moving");
			else
				sprite.Play("jump");
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			if ( IsOnFloor() )
				sprite.Play("idle");
			else
				sprite.Play("jump");
		}

		sprite.FlipH = shouldFaceLeft;

		Velocity = velocity;
		MoveAndSlide();
	}
}
