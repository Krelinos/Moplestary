using Godot;
using System;

public abstract partial class Mob : CharacterBody2D
{
    [Export] public float MoveSpeed = 160f;
	[Export] public float JumpPower = -700f;

	protected float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	protected AnimatedSprite2D Sprite;
	protected CollisionShape2D CollisionShape;
    protected StateMachine StateMachine;

    // The Terrain TileMap of the region this Mob is on.
	protected TileMap _Terrain;

    // The mob's position on TILE_MAP's grid.
	protected Vector2I GridPosition
	{
		get { return _Terrain.LocalToMap( _Terrain.ToLocal( GlobalPosition ) ); }
		// set {  }
	}

    // -1 | Mob is moving left. 0 | Mob is still in the x-axis. 1 | Mob is moving right.
	protected int movementDirection;

    // When dropping down a platform, the Mob ignores _Terrain collision.
    // Once the Mob's Global Y-Axis becomes greater than this number,
    // The Mob will react to _Terrain collision once more.
    protected float dropdownThreshold;

	protected bool facingLeft;
    protected Vector2 velocity;

	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_Terrain = GetNode<TileMap>("/root/Game/Terrain");

        movementDirection = 0;
	}

    public override void _PhysicsProcess(double delta)
	{
		velocity = Velocity;

        // Add the gravity.
		if ( !IsOnFloor() )
			velocity.Y += GRAVITY * (float)delta;
        
        // Check dropdown threshold.
        if ( CollisionShape.Disabled == true )
			if ( GlobalPosition.Y > dropdownThreshold )
				CollisionShape.Disabled = false;
        
        Move();

        // Update sprite stuff.
        SpriteStuff();

        Velocity = velocity;
        MoveAndSlide();
    }

    protected virtual void Move()
    {
        if( movementDirection == 0 )
        {
            // For mobs, conserve X-axis velocity until on ground.
            // For players, automatically try to reach 0 velocity in X-axis when airbone.
            if( IsOnFloor() )   // Keep for mobs, remove for players
			    velocity.X = Mathf.MoveToward(Velocity.X, 0, MoveSpeed);
        }
        else
            velocity.X = movementDirection * MoveSpeed;
    }

    protected virtual void SpriteStuff()
    {
        // facingLeft should also be used for anything else that needs to be mirrored
        // such as weapons for player characters.
        if ( velocity.X < 0 )
            facingLeft = true;
        else
            if ( velocity.X > 0 )
                facingLeft = false;

        Sprite.FlipH = facingLeft;

        if ( !IsOnFloor() )
            Sprite.Play("jump");
        else
            if ( velocity.X != 0f )
                Sprite.Play("moving");
            else
                Sprite.Play("idle");
    }

    protected virtual void Jump()
    {
        if ( IsOnFloor() )
			velocity.Y = JumpPower;
    }

    protected virtual void DropDown()
    {
        // Check if DropDown-able
        Vector2 hitboxBottom = GlobalPosition + ( new Vector2(0, 8) );

        // Tile position directly 8 units below the Mob.
        var tilePos = _Terrain.LocalToMap( _Terrain.ToLocal( hitboxBottom ) );

        // Check if the cell to be DropDown-ed from is a "No Dropdown" zone.
        // Currently, that means checking for an occupied cell in layer 4 in _Terrain.
        // Hardcoded. Might regret this.
        TileData tileData = _Terrain.GetCellTileData( 4, tilePos );
        if( tileData == null )
        {
            velocity.Y = -100;
            CollisionShape.Disabled = true;
            dropdownThreshold = GlobalPosition.Y + 8;
        }
    }

    public void TakeDamage( int amount, Node2D attacker, Node2D inflicter )
	{
		GD.Print( Name + " took " + amount + " damage from " + attacker.Name + " using a " + inflicter.Name );
	}

}