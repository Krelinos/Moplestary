using Godot;
using System;

public partial class MyCharacter : CharacterBody2D
{
	[Export] public float Speed = 200.0f;
	[Export] public float JumpVelocity = -700.0f;
	[Export] public float ClimbSpeed = 200f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public AnimatedSprite2D sprite;
	public CollisionShape2D hitbox;
	public Node2D WeaponHolder;

	public TileMap tileMap;
	private PackedScene _sceneFirebolt;

	private bool shouldFaceLeft;
	private bool isOnLadder;
	private float dropdownHeight;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		hitbox = GetNode<CollisionShape2D>("Hitbox");
		WeaponHolder = GetNode<Node2D>("WeaponHolder");
		tileMap = GetNode<TileMap>("/root/Game/Terrain");

		_sceneFirebolt = (PackedScene)ResourceLoader.Load<PackedScene>("res://scenes/firebolt.tscn");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Cast firebolt
		if ( Input.IsActionJustPressed("Shoot") )
		{
			var aFirebolt = _sceneFirebolt.Instantiate() as Area2D;
			GetNode("/root/Game").AddChild( aFirebolt );
			aFirebolt.GlobalPosition = WeaponHolder.GetNode<Node2D>("Staff/Head").GlobalPosition;
			if( shouldFaceLeft )
				aFirebolt.LookAt( aFirebolt.ToGlobal( new Vector2(-1, 0) ) );
		}

		// Handle ladder climbing
		if ( Input.IsActionJustPressed("MoveUp") )
		{

			Vector2 hitboxCenter = hitbox.GlobalPosition;
			var tilePos = tileMap.LocalToMap( tileMap.ToLocal( hitboxCenter ) );
			TileData tileData = tileMap.GetCellTileData( 2, tilePos );	// Check if a cell on the Ladder layer exists

			if( tileData != null )
			{
				isOnLadder = true;
				velocity = new Vector2(0f, 0f);
				GlobalPosition = new Vector2( tileMap.ToGlobal( tileMap.MapToLocal(tilePos) ).X, GlobalPosition.Y );
				// this.MotionMode = (MotionModeEnum)1;
			}
		}

		// Grab ladder below character
		if ( Input.IsActionJustPressed("MoveDown") )
		{

			Vector2 hitboxBottom = GlobalPosition + ( new Vector2(0, 2) );
			var tilePos = tileMap.LocalToMap( tileMap.ToLocal( hitboxBottom ) );
			TileData tileData = tileMap.GetCellTileData( 2, tilePos );	// Check if a cell on the Ladder layer exists

			if( tileData != null )
			{
				isOnLadder = true;
				velocity = new Vector2(0f, 0f);
				GlobalPosition = new Vector2( tileMap.ToGlobal( tileMap.MapToLocal(tilePos) ).X, GlobalPosition.Y+2 );
				// this.MotionMode = (MotionModeEnum)1;
			}
		}

		if( isOnLadder )
		{
			velocity.Y = Input.GetAxis("MoveUp", "MoveDown") * ClimbSpeed;

			Vector2 hitboxBottom = GlobalPosition;
			Vector2 hitboxCenter = hitbox.GlobalPosition;

			var tileBottom = tileMap.LocalToMap( tileMap.ToLocal( hitboxBottom ) );
			var tileCenter = tileMap.LocalToMap( tileMap.ToLocal( hitboxCenter ) );

			if( tileMap.GetCellTileData( 2, tileBottom ) == null && tileMap.GetCellTileData( 2, tileCenter ) == null )
				isOnLadder = false;
		}

		// Add the gravity.
		if (!IsOnFloor() && !isOnLadder )
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if ( Input.IsActionPressed("Jump") )
		{
			if( IsOnFloor() )
			{
				if( Input.IsActionPressed("MoveDown") )	// Drop down
				{
					// Check if dropdown-able
					Vector2 hitboxBottom = GlobalPosition + ( new Vector2(0, 8) );
					var tilePos = tileMap.LocalToMap( tileMap.ToLocal( hitboxBottom ) );
					TileData tileData = tileMap.GetCellTileData( 4, tilePos );	// Check if the cell to be dropdowned from is a "No Dropdown" zone

					if( tileData == null )
					{
						velocity.Y = -20;
						hitbox.Disabled = true;
						dropdownHeight = GlobalPosition.Y;
					}
				}
				else
					velocity.Y = JumpVelocity;
			}
			else if( isOnLadder )
			{
				isOnLadder = false;
				velocity.Y = JumpVelocity * 0.5f;
			}
		}

		if ( hitbox.Disabled == true )
			if ( GlobalPosition.Y > dropdownHeight+8 )
				hitbox.Disabled = false;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = new Vector2( Input.GetAxis("MoveLeft", "MoveRight"), 0f ); //, "ui_up", "ui_down");
		if (direction != Vector2.Zero && !isOnLadder )	// At least one movement key is pressed
		{
			velocity.X = direction.X * Speed;

			if ( IsOnFloor() )
				sprite.Play("moving");
			else
				sprite.Play("jump");
		
			if ( direction.X < 0 )
				shouldFaceLeft = true;
			else if ( direction.X > 0 )
				shouldFaceLeft = false;
		
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
		int scale = shouldFaceLeft ? -1 : 1;

		WeaponHolder.Scale = new Vector2( scale, 1 );

		Velocity = velocity;
		MoveAndSlide();
	}

	public void _OnProjectileTargetAreaAreaEntered( Area2D area )
	{
	}
}
