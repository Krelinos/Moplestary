using Godot;
using System;

partial class PlayerCharacter : Mob
{
    [Export] public float ClimbSpeed = 150f;

	public Node2D WeaponHolder;
	private Area2D RangedAttackTargetingArea;

    private bool isOnLadder;

    public override void _Ready()
    {
        base._Ready();

		WeaponHolder = GetNode<Node2D>("WeaponHolder");
		RangedAttackTargetingArea = GetNode<Area2D>("WeaponHolder/RangedAttackTargetingArea");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // WeaponHolder.Scale =  facingLeft ? new Vector2( -1, 1 ) : Vector2.One;
    }

    protected override void Move()
    {
        // Handle Jump.
		if ( Input.IsActionPressed("Jump") )
		{
			if( IsOnFloor() )
			{
				if( Input.IsActionPressed("MoveDown") )
					DropDown();
				else
					velocity.Y = JumpPower;
			}
			else
                if( isOnLadder && Input.IsActionJustPressed("Jump") )
                {
                    isOnLadder = false;
                    velocity.Y = -200;
                }
		}

        // Ladder movement
        if ( isOnLadder )
		{
			velocity.Y = Input.GetAxis("MoveUp", "MoveDown") * ClimbSpeed;

            // Return to normal movement if there is no Ladder tile in the middle
            // or bottom of the PlayerCharacter, or if the player is on the ground.
			Vector2 upperThreshold = GlobalPosition + new Vector2( 0, -12 );
			Vector2 lowerTheshold = GlobalPosition;

			var tileUpper = _Terrain.LocalToMap( _Terrain.ToLocal( upperThreshold ) );
			var tileLower = _Terrain.LocalToMap( _Terrain.ToLocal( lowerTheshold ) );

			if( ( _Terrain.GetCellTileData( 2, tileUpper ) == null
                        && _Terrain.GetCellTileData( 2, tileLower ) == null )
                    || IsOnFloor() )
				isOnLadder = false;
            
            return;
		}
        else
        {
            // Horizontal movement
            float direction = Input.GetAxis("MoveLeft", "MoveRight");
            if ( direction != 0 )
                // velocity.X = Mathf.MoveToward( velocity.X, direction * MoveSpeed, MoveSpeed );
                velocity.X = direction * MoveSpeed;
            else
                velocity.X = Mathf.MoveToward(Velocity.X, 0, MoveSpeed);
            
            // Grab ladder
            if ( Input.IsActionJustPressed("MoveUp") )
                GrabLadder( new Vector2( 0, -8 ) );
            if ( Input.IsActionJustPressed("MoveDown") )
                GrabLadder( new Vector2( 0, 4 ) );
        }
    }

    protected void GrabLadder( Vector2 offset )
    {
        var tilePos = _Terrain.LocalToMap( _Terrain.ToLocal( GlobalPosition + offset ) );

        // Check if a cell on the Ladder layer exists
        // Assumes the Ladder layer in _Terrain is 2. Hardcoded nastiness.
        TileData tileData = _Terrain.GetCellTileData( 2, tilePos );	
        if( tileData != null )
        {
            isOnLadder = true;
            velocity = Vector2.Zero;

            // -1 on Y-axis so the character is technically off the floor.
            GlobalPosition = new Vector2( _Terrain.ToGlobal( _Terrain.MapToLocal(tilePos) ).X, GlobalPosition.Y + offset.Y );
        }
    }
}