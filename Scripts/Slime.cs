using Godot;
using System;

public partial class Slime : CharacterBody2D
{
	[Export] public float Speed = 150.0f;
	[Export] public float JumpVelocity = -700.0f;

	// (x,y) offsets from the mob that will be considered when checking for possible jumps.
	// Only consider positive x-coord jumps; the code will handle negative x-coords.
	// Units are based on TILE_MAP grid size, so (3, 0) is a jump to reach 3 tiles right of the mob.
	[Export] public Godot.Collections.Array<Vector2I> PotentialJumpCoords { get; set; } = new Godot.Collections.Array<Vector2I>
			{
				new Vector2I( 2, -1 ),
				new Vector2I( 1, -2 ),
				// new Vector2I( 3, 0 )
			};

	// Get the global gravity from the project settings to be synced with RigidBody nodes.
	private float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Visual sprite of this mob.
	private AnimatedSprite2D SPRITE;

	// The main TileMap of the Scene this mob is on.
	private TileMap TILE_MAP;

	private RandomNumberGenerator RNG;
	
	// The mob's position on TILE_MAP's grid.
	private Vector2I GridPosition
	{
		get { return TILE_MAP.LocalToMap( TILE_MAP.ToLocal( GlobalPosition ) ); }
		// set {  }
	}

	// The character this mob is fighting.
	private Node2D target;
	// -1 | Mob is moving left. 0 | Mob is still in the x-axis. 1 | Mob is moving right.
	private int movementDirection;
	// True | Mob is facing left. False | Mob is facing right.
	private bool facingDirection;
	// Only change state (moving/idle) once this is < 0. Decreases by 1 every second.
	private double wanderTime;

	public override void _Ready()
	{
		SPRITE = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		TILE_MAP = GetNode<TileMap>("/root/Game/Terrain");

		RNG = new RandomNumberGenerator();
		RNG.Randomize();

		movementDirection = 0;
		facingDirection = true;
		wanderTime = RNG.RandiRange( 2, 6 );
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		wanderTime -= delta;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += GRAVITY * (float)delta;
		
		// Wander if no target
		if( target == null )
		{
			if( wanderTime < 0 )
			{
				// Change state from idle to moving
				if( movementDirection == 0 )
				{
					if( RNG.Randf() < 0.5 )	// .Randf() returns a float between 0.0 and 1.0 (inclusive)
					{
						movementDirection = -1;
						facingDirection = true;
					}
					else
					{
						movementDirection = 1;
						facingDirection = false;
					}
				}
				// Change state from moving to idle
				else
					movementDirection = 0;
				
				wanderTime = RNG.RandiRange( 2, 6 );
			}

			if( movementDirection == 0 )
			{
				 velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

				if ( IsOnFloor() )
					SPRITE.Play("idle");
				else
					SPRITE.Play("jump");
			}
			else
			{
				velocity.X = movementDirection * Speed;

				if( IsOnFloor() )
				{
					if( shouldMobJump() )
						velocity.Y = JumpVelocity;
				
					if( IsAboutToFallOffLedge() )
					{
						if( RNG.Randf() > 0.3f )
						{
							movementDirection *= -1;
							facingDirection = !facingDirection;
						}
						else
							velocity.Y = JumpVelocity;
					}
				}

				if ( IsOnFloor() )
					SPRITE.Play("moving");
				else
					SPRITE.Play("jump");
			}

		}
		else
		{
			var targetGridPos = TILE_MAP.LocalToMap( TILE_MAP.ToLocal( target.GlobalPosition ) );
			var mobGridPos = TILE_MAP.LocalToMap( TILE_MAP.ToLocal( GlobalPosition ) );

			// How many TILE_MAP tiles the target is from the mob.
			var targetOffset = targetGridPos - mobGridPos;
		}
		
		// Handle Jump.
		// if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		// 	velocity.Y = JumpVelocity;

		// Horizontal movement
		// if ( RNG.RandiRange(0, 200) == 0 )
		// {
		// 	if ( movementDirection == 0 )
		// 		movementDirection = RNG.RandiRange(-1, 1);
		// 	else
		// 		movementDirection = 0;
		// }

		// if (movementDirection != 0)
		// {
		// 	velocity.X = movementDirection * Speed;

		// 	if ( IsOnFloor() )
		// 		SPRITE.Play("moving");
		// 	else
		// 		SPRITE.Play("jump");
		// }
		// else
		// {
		// 	velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

		// 	if ( IsOnFloor() )
		// 		SPRITE.Play("idle");
		// 	else
		// 		SPRITE.Play("jump");
		// }

		SPRITE.FlipH = facingDirection;

		Velocity = velocity;
		MoveAndSlide();
	}

	//
	private bool shouldMobJump()
	{
		foreach( Vector2I offset in PotentialJumpCoords )
		{
			// The tile below the target tile position.
			// offset is multiplied in order to mirror the coords about the y-axis when facing left.
			Vector2I landingTile = GridPosition + new Vector2I(0, 1) + ( offset * ( new Vector2I( movementDirection, 1 ) ) );

			if( TILE_MAP.GetCellSourceId( 1, landingTile ) == -1 )			// 1 should be the main terrain layer (this is an assumption).
			{																// A return of -1 means the landing tile does not exist.
				;	// Nothing
			}
			else
			{
				// Currently, if a mob detects it can jump somewhere, it will 100% of the time.
				// TODO: Change this behavior.
				return true;
			}
		}

		return false;
	}

	private bool IsAboutToFallOffLedge()
	{
		if( TILE_MAP.GetCellSourceId( 1, GridPosition + (new Vector2I(movementDirection, 1)) ) == -1 )
			return true;
		return false;
	}
}
